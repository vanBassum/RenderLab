using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Rendering;
using System.Diagnostics;
using System.Numerics;

namespace Engine2D.Tiles.Caching
{
    public sealed class TileProviderMemoryCache : ITileProvider
    {
        private readonly ITileProvider _provider;
        private readonly MemoryLruCache<TileRenderKey, TileRenderItem> _cache;

        public TileProviderMemoryCache(ITileProvider provider, int maxEntries)
        {
            _provider = provider;
            _cache = new MemoryLruCache<TileRenderKey, TileRenderItem>(maxEntries, (k, v) => {
                //Debug.WriteLine($"TileProviderMemoryCache: Evicting tile X={k.X} Y={k.Y} Z={k.Z} Size={k.PixelWidth}x{k.PixelHeight}");
            });
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
                    //Debug.WriteLine($"TileProviderMemoryCache: Caching tile X={tile.TileX} Y={tile.TileY} Z={tile.TileZoom} Size={tile.Image.Width}x{tile.Image.Height}");
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
