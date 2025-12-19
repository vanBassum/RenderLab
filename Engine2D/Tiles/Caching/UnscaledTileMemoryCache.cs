using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Images;
using Engine2D.Tiles.Metrics;
using Microsoft.Extensions.Caching.Memory;

namespace Engine2D.Tiles.Caching
{
    public sealed class UnscaledTileMemoryCache : ITileSource
    {
        private readonly ITileSource _inner;
        private readonly IMemoryCache _cache;
        private readonly ITileMetrics _metrics;

        public UnscaledTileMemoryCache(ITileSource inner, IMemoryCache cache, ITileMetrics metrics)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _metrics = metrics ?? NullTileMetrics.Instance;
        }

        public async ValueTask<TileImage?> GetTileAsync(int x, int y, int zoom)
        {
            _metrics.TileRequest(nameof(UnscaledTileMemoryCache));
            var key = $"unscaled:{zoom}:{x}:{y}";

            if (_cache.TryGetValue(key, out TileImage? cached))
            {
                _metrics.TileHit(nameof(UnscaledTileMemoryCache));
                return cached;
            }

            _metrics.TileMiss(nameof(UnscaledTileMemoryCache));

            var tile = await _inner.GetTileAsync(x, y, zoom).ConfigureAwait(false);
            if (tile == null)
                return null;

            _cache.Set(
                key,
                tile,
                TileCachePolicy.Create(
                    tile,
                    onEvicted: () =>
                        _metrics.TileEvicted(nameof(UnscaledTileMemoryCache))));

            return tile;
        }
    }
}
