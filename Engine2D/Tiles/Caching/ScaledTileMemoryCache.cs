using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Images;
using Engine2D.Tiles.Metrics;
using Microsoft.Extensions.Caching.Memory;

namespace Engine2D.Tiles.Caching
{
    public sealed class ScaledTileMemoryCache : ITileProvider
    {
        private readonly ITileProvider _inner;
        private readonly IMemoryCache _cache;
        private readonly ITileMetrics _metrics;

        public ScaledTileMemoryCache(ITileProvider inner, IMemoryCache cache, ITileMetrics metrics)
        {
            _inner = inner;
            _cache = cache;
            _metrics = metrics ?? NullTileMetrics.Instance;
        }

        public async ValueTask<TileImage?> GetTileAsync(int x, int y, float zoom)
        {
            _metrics.TileRequest(nameof(ScaledTileMemoryCache));

            zoom = MathF.Round(zoom * 16f) / 16f;
            var key = $"scaled:{x}:{y}:{zoom}";

            if (_cache.TryGetValue(key, out TileImage cached))
            {
                _metrics.TileHit(nameof(ScaledTileMemoryCache));
                return cached;
            }

            _metrics.TileMiss(nameof(ScaledTileMemoryCache));

            var tile = await _inner.GetTileAsync(x, y, zoom).ConfigureAwait(false);
            if (tile == null)
                return null;

            _cache.Set(
                key,
                tile,
                TileCachePolicy.Create(
                    tile,
                    onEvicted: () => _metrics.TileEvicted(nameof(ScaledTileMemoryCache))));

            return tile;
        }
    }


}


