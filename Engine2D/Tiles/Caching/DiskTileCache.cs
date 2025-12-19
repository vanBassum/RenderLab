using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Images;
using Engine2D.Tiles.Metrics;

namespace Engine2D.Tiles.Caching
{
    public sealed class DiskTileCache : ITileSource
    {
        private readonly string _cacheDirectory;
        private readonly ITileSource _inner;
        private readonly ITileMetrics _metrics;

        public DiskTileCache(string directory, ITileSource inner, ITileMetrics metrics)
        {
            _cacheDirectory = directory ?? throw new ArgumentNullException(nameof(directory));
            _inner = inner;
            _metrics = metrics ?? NullTileMetrics.Instance;
            Directory.CreateDirectory(directory);
        }

        public async ValueTask<TileImage?> GetTileAsync(int x, int y, int zoom)
        {
            _metrics.TileRequest(nameof(DiskTileCache));
            var path = GetTilePath(x, y, zoom);

            if (File.Exists(path))
            {
                _metrics.TileHit(nameof(DiskTileCache));
                using var bmp = new Bitmap(path);
                return new TileImage(new Bitmap(bmp));
            }

            _metrics.TileMiss(nameof(DiskTileCache));

            var tile = await _inner.GetTileAsync(x, y, zoom);
            if (tile != null)
                tile.Bitmap.Save(path);

            return tile;
        }

        private string GetTilePath(int x, int y, int zoom)
            => Path.Combine(_cacheDirectory, $"{zoom}_{x}_{y}.png");
    }


}



