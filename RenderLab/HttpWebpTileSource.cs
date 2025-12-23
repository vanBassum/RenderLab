using Engine2D.Tiles.Abstractions;
using RenderLab.Targets.WinForms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RenderLab
{
    public sealed class HttpWebpTileSource : ITileSource, IDisposable
    {
        public int MinZ { get; } = 0;
        public int MaxZ { get; } = 6;

        private readonly string _urlTemplate;
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _downloadLimiter;

        private readonly ConcurrentDictionary<string, byte> _failedUrls = new();
        private readonly ConcurrentDictionary<string, Task<ITileImage?>> _inFlight = new();

        public HttpWebpTileSource(string urlTemplate, int maxConcurrentDownloads = 4, HttpClient? httpClient = null)
        {
            _urlTemplate = urlTemplate
                ?? throw new ArgumentNullException(nameof(urlTemplate));

            _httpClient = httpClient ?? new HttpClient();
            _downloadLimiter = new SemaphoreSlim(maxConcurrentDownloads);
        }

        public ITileImage? GetTile(TileKey tileKey)
        {
            if (tileKey.Z < MinZ || tileKey.Z > MaxZ)
                return null;

            if (tileKey.X < 0 || tileKey.Y < 0)
                return null;

            var url = BuildUrl(tileKey.X, tileKey.Y, tileKey.Z);

            if (_failedUrls.ContainsKey(url))
                return null;

            var task = _inFlight.GetOrAdd(url, _ => DownloadAsync(url));

            if (!task.IsCompleted)
                return null;

            _inFlight.TryRemove(url, out _);

            if (task.IsFaulted || task.Result == null)
            {
                MarkFailed(
                    url,
                    task.Exception?.GetBaseException().Message ?? "404");
                return null;
            }

            return task.Result;
        }

        private async Task<ITileImage?> DownloadAsync(string url)
        {
            await _downloadLimiter.WaitAsync().ConfigureAwait(false);
            try
            {
                using var response = await _httpClient.GetAsync(
                    url,
                    HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return null;

                await using var stream = await response.Content
                    .ReadAsStreamAsync()
                    .ConfigureAwait(false);

                using var image = await SixLabors.ImageSharp.Image.LoadAsync<Rgba32>(stream)
                    .ConfigureAwait(false);

                var bitmap = ConvertToBitmap(image);
                return new WinFormsTileImage(bitmap);
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

        private static System.Drawing.Bitmap ConvertToBitmap(Image<Rgba32> image)
        {
            var bitmap = new System.Drawing.Bitmap(
                image.Width,
                image.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            bitmap.SetResolution(96f, 96f);

            var rect = new System.Drawing.Rectangle(
                0, 0, bitmap.Width, bitmap.Height);

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

        private string BuildUrl(int x, int y, int z)
        {
            return _urlTemplate
                .Replace("{x}", x.ToString())
                .Replace("{y}", y.ToString())
                .Replace("{z}", z.ToString());
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
