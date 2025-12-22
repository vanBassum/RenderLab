using Engine2D.Tiles.Abstractions;
using RenderLab.Targets.WinForms;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace RenderLab
{
    public sealed class HttpTileSource : ITileSource, IDisposable
    {
        private readonly string _urlTemplate;
        private readonly HttpClient _httpClient;

        // URLs that previously failed → never retry
        private readonly ConcurrentDictionary<string, byte> _failedUrls = new();

        public HttpTileSource(string urlTemplate, HttpClient? httpClient = null)
        {
            _urlTemplate = urlTemplate
                ?? throw new ArgumentNullException(nameof(urlTemplate));

            _httpClient = httpClient ?? new HttpClient();
        }

        public ITileImage? GetTile(TileKey tileKey)
        {
            var url = BuildUrl(tileKey.X, tileKey.Y, tileKey.Z);

            // Do not retry known failures
            if (_failedUrls.ContainsKey(url))
                return null;

            try
            {
                using var response = _httpClient
                    .GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                    .GetAwaiter()
                    .GetResult();

                if (!response.IsSuccessStatusCode)
                {
                    MarkFailed(url, $"HTTP {(int)response.StatusCode}");
                    return null;
                }

                using var stream = response.Content
                    .ReadAsStreamAsync()
                    .GetAwaiter()
                    .GetResult();

                var bitmap = new Bitmap(stream);
                return new WinFormsTileImage(bitmap);
            }
            catch (Exception ex)
            {
                MarkFailed(url, ex.Message);
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

