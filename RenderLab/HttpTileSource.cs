using Engine2D.Tiles.Abstractions;
using RenderLab.Targets.WinForms;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace RenderLab
{
    public sealed class HttpTileSource : ITileSource, IDisposable
    {
        public int MinZ { get; } = 0;
        public int MaxZ { get; } = 5;

        private readonly string _urlTemplate;
        private readonly HttpClient _httpClient;

        private readonly ConcurrentDictionary<string, byte> _failedUrls = new();
        private readonly ConcurrentDictionary<string, Task<ITileImage?>> _inFlight = new();

        public HttpTileSource(string urlTemplate, HttpClient? httpClient = null)
        {
            _urlTemplate = urlTemplate
                ?? throw new ArgumentNullException(nameof(urlTemplate));

            _httpClient = httpClient ?? new HttpClient();
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

            // Start download if not already in progress
            var task = _inFlight.GetOrAdd(url, _ => DownloadAsync(url));

            // If still downloading → return null
            if (!task.IsCompleted)
                return null;

            // Download finished
            _inFlight.TryRemove(url, out _);

            if (task.IsFaulted || task.Result == null)
            {
                MarkFailed(url, task.Exception?.GetBaseException().Message ?? "Unknown error");
                return null;
            }

            return task.Result;
        }

        private async Task<ITileImage?> DownloadAsync(string url)
        {
            try
            {
                using var response = await _httpClient.GetAsync(
                    url,
                    HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return null;

                await using var stream = await response.Content
                    .ReadAsStreamAsync()
                    .ConfigureAwait(false);

                var bitmap = new Bitmap(stream);
                bitmap.SetResolution(96f, 96f);
                return new WinFormsTileImage(bitmap);
            }
            catch
            {
                return null;
            }
        }

        private void MarkFailed(string url, string reason)
        {
            _failedUrls.TryAdd(url, 0);
            Debug.WriteLine($"HttpTileSource: Blacklisted {url} ({reason})");
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

