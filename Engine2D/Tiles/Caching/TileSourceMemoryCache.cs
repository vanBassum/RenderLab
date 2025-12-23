using Engine2D.Tiles.Abstractions;
using System.Diagnostics;
using System.Numerics;

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
                v.Dispose();
                //Debug.WriteLine($"TileSourceMemoryCache: Evicting tile {k.X},{k.Y},{k.Z} from cache");
            });
        }

        public ITileImage? GetTile(TileKey tileKey)
        {
            if (_cache.TryGet(tileKey, out var image))
                return image;

            image = _source.GetTile(tileKey);

            if (image != null)
                _cache.Add(tileKey, image);

            return image;
        }
    }



}
