using System.Numerics;

namespace Engine2D.Tiles
{
    public sealed class TileProviderMemoryCache : ITileProvider
    {
        private readonly ITileProvider _provider;
        private readonly MemoryLruCache<TileRenderKey, TileRenderItem> _cache;

        public TileProviderMemoryCache(ITileProvider provider, int maxEntries)
        {
            _provider = provider;
            _cache = new MemoryLruCache<TileRenderKey, TileRenderItem>(maxEntries);
        }

        public IEnumerable<TileRenderItem> GetTiles(Vector2 worldMin, Vector2 worldMax)
        {
            foreach (var tile in _provider.GetTiles(worldMin, worldMax))
            {
                var key = new TileRenderKey(
                    tile.TileX,
                    tile.TileY,
                    tile.TileZoom,
                    tile.Image.Width,
                    tile.Image.Height);

                if (_cache.TryGet(key, out var cached))
                {
                    yield return cached;
                }
                else
                {
                    _cache.Add(key, tile);
                    yield return tile;
                }
            }
        }

        public readonly record struct TileRenderKey(
            int X,
            int Y,
            int Z,
            int PixelWidth,
            int PixelHeight);
    }

}
