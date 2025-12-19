namespace Engine2D.Tiles
{
    public sealed class TileSourceMemoryCache : ITileSource
    {
        private readonly ITileSource _source;
        private readonly MemoryLruCache<TileKey, ITileImage> _cache;

        public TileSourceMemoryCache(ITileSource source, int maxEntries)
        {
            _source = source;
            _cache = new MemoryLruCache<TileKey, ITileImage>(maxEntries);
        }

        public ITileImage GetTile(int x, int y, int zoom)
        {
            var key = new TileKey(x, y, zoom);

            if (_cache.TryGet(key, out var image))
                return image;

            image = _source.GetTile(x, y, zoom);
            _cache.Add(key, image);
            return image;
        }
        public readonly record struct TileKey(int X, int Y, int Z);
    }
}
