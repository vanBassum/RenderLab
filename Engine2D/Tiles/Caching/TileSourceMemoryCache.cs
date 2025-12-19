using Engine2D.Tiles.Abstractions;
using System.Diagnostics;

namespace Engine2D.Tiles.Caching
{
    public sealed class TileSourceMemoryCache : ITileSource
    {
        private readonly ITileSource _source;
        private readonly MemoryLruCache<TileKey, ITileImage> _cache;

        public TileSourceMemoryCache(ITileSource source, int maxEntries)
        {
            _source = source;
            _cache = new MemoryLruCache<TileKey, ITileImage>(maxEntries, (k, v) => { 
                //Debug.WriteLine($"TileSourceMemoryCache: Evicting tile {k.X},{k.Y},{k.Z} from cache");
            });
        }

        public ITileImage? GetTile(int x, int y, int zoom)
        {
            var key = new TileKey(x, y, zoom);

            if (_cache.TryGet(key, out var image))
                return image;

            //Debug.WriteLine($"TileSourceMemoryCache: Cache miss for tile {x},{y},{zoom}");
            image = _source.GetTile(x, y, zoom);

            if (image != null)
                _cache.Add(key, image);

            return image;
        }
        public readonly record struct TileKey(int X, int Y, int Z);
    }
}
