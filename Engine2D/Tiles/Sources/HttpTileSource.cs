using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Images;
using Engine2D.Tiles.Metrics;
using System.Globalization;

namespace Engine2D.Tiles.Sources
{
    public sealed class HttpTileSource : ITileSource
    {
        private readonly HttpClient _httpClient;
        private readonly string _urlTemplate;
        private readonly ITileMetrics _metrics;

        public HttpTileSource(string urlTemplate, HttpClient? httpClient = null, ITileMetrics? metrics = null)
        {
            _urlTemplate = urlTemplate ?? throw new ArgumentNullException(nameof(urlTemplate));
            _httpClient = httpClient ?? new HttpClient();
            _metrics = metrics ?? NullTileMetrics.Instance;
        }

        public async ValueTask<TileImage?> GetTileAsync(int x, int y, int zoom)
        {
            _metrics.TileRequest(nameof(HttpTileSource));
            var url = _urlTemplate
                .Replace("{x}", x.ToString(CultureInfo.InvariantCulture))
                .Replace("{y}", y.ToString(CultureInfo.InvariantCulture))
                .Replace("{z}", zoom.ToString(CultureInfo.InvariantCulture));

            try
            {
                using var response = await _httpClient
                    .GetAsync(url, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                    return null;

                await using var stream =
                    await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

                var bitmap = new Bitmap(stream);

                // Successful remote fetch
                _metrics.TileDownloaded(nameof(HttpTileSource));

                return new TileImage(bitmap);
            }
            catch
            {
                return null;
            }
        }
    }
}
