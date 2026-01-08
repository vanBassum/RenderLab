using Engine2D.Tiles.Abstractions;

namespace Engine2D.Tiles.Caching
{
    public sealed class TileMemoryCache : ITileProvider
    {
        private readonly ITileProvider _inner;
        private readonly MemoryLruCache<TileCacheKey, ITileImage> _cache;

        public TileMemoryCache(ITileProvider inner, int maxEntries)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));

            _cache = new MemoryLruCache<TileCacheKey, ITileImage>(
                maxEntries,
                onEvict: (k, v) =>
                {
                    v.Dispose();
                    // Debug.WriteLine($"TileProviderMemoryCache: Evict {k}");
                });
        }

        public async ValueTask<TileFetchResult> FetchAsync(TileQuery query, CancellationToken token)
        {
            var key = TileCacheKey.FromQuery(query);

            if (_cache.TryGet(key, out var cached))
            {
                return new TileFetchResult { Image = cached };
            }

            var result = await _inner.FetchAsync(query, token).ConfigureAwait(false);

            // Do not cache null results (in-flight / miss / failure).
            if (result.Image is not null)
            {
                _cache.Add(key, result.Image);
            }

            return result;
        }
    }



}
