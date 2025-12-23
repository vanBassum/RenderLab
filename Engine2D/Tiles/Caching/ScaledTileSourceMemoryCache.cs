using Engine2D.Tiles.Abstractions;
using System.Numerics;

namespace Engine2D.Tiles.Caching
{
    public sealed class ScaledTileSourceMemoryCache : IScaledTileSource
    {
        private readonly IScaledTileSource _source;
        private readonly MemoryLruCache<(TileKey, Vector2), ITileImage> _cache;

        public ScaledTileSourceMemoryCache(IScaledTileSource source, int maxEntries)
        {
            _source = source;
            _cache = new MemoryLruCache<(TileKey, Vector2), ITileImage>(maxEntries, (k, v) => { 
                v.Dispose();
            });
        }

        public ITileImage? GetTile(TileKey tileKey, Vector2 screenSize)
        {
            var key = (tileKey, Quantize(screenSize));

            if (_cache.TryGet(key, out var cached))
                return cached;

            var image = _source.GetTile(tileKey, screenSize);
            if (image != null)
                _cache.Add(key, image);

            return image;
        }

        private static Vector2 Quantize(Vector2 size)
        {
            // Avoid caching infinite variants due to float noise
            return new Vector2(MathF.Round(size.X), MathF.Round(size.Y));
        }
    }
}


