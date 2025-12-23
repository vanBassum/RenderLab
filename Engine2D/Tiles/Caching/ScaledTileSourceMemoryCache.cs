using Engine2D.Tiles.Abstractions;
using System.Diagnostics;

namespace Engine2D.Tiles.Caching
{
    public sealed class ScaledTileSourceMemoryCache : IScaledTileSource
    {
        private readonly IScaledTileSource _source;
        private readonly MemoryLruCache<string, ITileImage> _cache;

        public ScaledTileSourceMemoryCache(IScaledTileSource source, int maxEntries)
        {
            _source = source;
            _cache = new (maxEntries, (k, v) => { 
                v.Dispose();
            });
        }

        public ITileImage? GetTile(TileKey tileKey, int screenSize)
        {
            var key = tileKey.ToString() + screenSize.ToString();

            if (_cache.TryGet(key, out var cached))
                return cached;


            var image = _source.GetTile(tileKey, screenSize);
            if (image != null)
            {
                Debug.WriteLine($"Memory cache miss for scaled tile {tileKey} at size {screenSize}.");
                _cache.Add(key, image);
            }

            return image;
        }
    }
}


