using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RenderLab.Targets.WinForms
{
    public sealed class HttpWebpTileProvider : ITileProvider, IDisposable
    {
        public int MinTileLevel { get; } = 0;
        public int MaxTileLevel { get; } = 6;

        private readonly string _urlTemplate;
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _downloadLimiter;

        private readonly ConcurrentDictionary<string, byte> _failedUrls = new();
        private readonly ConcurrentDictionary<string, Task<ITileImage?>> _inFlight = new();

        public HttpWebpTileProvider(string urlTemplate, int maxConcurrentDownloads = 4, HttpClient? httpClient = null)
        {
            _urlTemplate = urlTemplate ?? throw new ArgumentNullException(nameof(urlTemplate));
            _httpClient = httpClient ?? new HttpClient();
            _downloadLimiter = new SemaphoreSlim(maxConcurrentDownloads);
        }

        public ValueTask<TileFetchResult> FetchAsync(TileQuery query, CancellationToken token)
        {
            var tile = query.Tile;

            // Validate tile coordinates / level
            if (tile.TileLevel < MinTileLevel || tile.TileLevel > MaxTileLevel)
                return ValueTask.FromResult(new TileFetchResult { Image = null });

            if (tile.X < 0 || tile.Y < 0)
                return ValueTask.FromResult(new TileFetchResult { Image = null });

            // Optional: reject impossible indices for this level
            int tilesPerAxis = 1 << tile.TileLevel;
            if (tile.X >= tilesPerAxis || tile.Y >= tilesPerAxis)
                return ValueTask.FromResult(new TileFetchResult { Image = null });

            var url = BuildUrl(tile.X, tile.Y, tile.TileLevel);

            if (_failedUrls.ContainsKey(url))
                return ValueTask.FromResult(new TileFetchResult { Image = null });

            // Start download if not already in-flight
            var task = _inFlight.GetOrAdd(url, _ => DownloadAsync(url, token));

            // Non-blocking: if not completed yet, return null image for now
            if (!task.IsCompleted)
                return ValueTask.FromResult(new TileFetchResult { Image = null });

            _inFlight.TryRemove(url, out _);

            // Completed: check result
            if (task.IsFaulted || task.Result is null)
            {
                MarkFailed(url, task.Exception?.GetBaseException().Message ?? "Download failed / 404");
                return ValueTask.FromResult(new TileFetchResult { Image = null });
            }

            return ValueTask.FromResult(new TileFetchResult { Image = task.Result });
        }

        private async Task<ITileImage?> DownloadAsync(string url, CancellationToken token)
        {
            await _downloadLimiter.WaitAsync(token).ConfigureAwait(false);
            try
            {
                using var response = await _httpClient.GetAsync(
                        url,
                        HttpCompletionOption.ResponseHeadersRead,
                        token)
                    .ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return null;

                await using var stream = await response.Content
                    .ReadAsStreamAsync(token)
                    .ConfigureAwait(false);

                using var image = await SixLabors.ImageSharp.Image.LoadAsync<Rgba32>(stream, token)
                    .ConfigureAwait(false);

                var bitmap = ConvertToBitmap(image);
                return new WinFormsTileImage(bitmap);
            }
            catch (OperationCanceledException)
            {
                // Don’t blacklist on cancellation; just treat as not available.
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"HttpWebpTileSource error: {ex}");
                return null;
            }
            finally
            {
                _downloadLimiter.Release();
            }
        }

        private static Bitmap ConvertToBitmap(Image<Rgba32> image)
        {
            var bitmap = new Bitmap(
                image.Width,
                image.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bitmap.SetResolution(96f, 96f);

            var rect = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var data = bitmap.LockBits(
                rect,
                System.Drawing.Imaging.ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                image.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var srcRow = accessor.GetRowSpan(y);
                        var dstPtr = data.Scan0 + y * data.Stride;

                        // Copy row to bitmap
                        // (Keeping your original approach; can be optimized later to avoid ToArray per row.)
                        Marshal.Copy(
                            MemoryMarshal.AsBytes(srcRow).ToArray(),
                            0,
                            dstPtr,
                            srcRow.Length * 4);
                    }
                });
            }
            finally
            {
                bitmap.UnlockBits(data);
            }

            return bitmap;
        }

        private void MarkFailed(string url, string reason)
        {
            _failedUrls.TryAdd(url, 0);
            Debug.WriteLine($"HttpWebpTileSource: Blacklisted {url} ({reason})");
        }

        private string BuildUrl(int x, int y, int tileLevel)
        {
            return _urlTemplate
                .Replace("{x}", x.ToString())
                .Replace("{y}", y.ToString())
                .Replace("{z}", tileLevel.ToString()); // keep template token {z}
        }

        public void Dispose()
        {
            _httpClient.Dispose();
            _downloadLimiter.Dispose();
        }
    }
}
