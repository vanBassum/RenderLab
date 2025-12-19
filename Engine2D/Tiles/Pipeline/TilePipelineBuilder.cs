using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Metrics;
using Engine2D.Tiles.Caching;
using Engine2D.Tiles.Scaling;
using Engine2D.Tiles.Sources;
using Microsoft.Extensions.Caching.Memory;


namespace Engine2D.Tiles.Pipeline
{
    public sealed class TilePipelineBuilder
    {
        private string? _tileServerUrl;
        private string? _diskCacheDirectory;

        private bool _useUnscaledMemoryCache = true;
        private bool _useScaledMemoryCache = true;

        private int _tileSize = 256;

        private long _unscaledMemoryLimitBytes = 512L * 1024 * 1024; // 512 MB
        private long _scaledMemoryLimitBytes = 128L * 1024 * 1024; // 128 MB

        private HttpClient? _httpClient;
        private ITileMetrics _metrics = NullTileMetrics.Instance;

        // ---------------- Fluent configuration ----------------

        public TilePipelineBuilder WithTileServer(string url)
        {
            _tileServerUrl = url;
            return this;
        }

        public TilePipelineBuilder WithHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            return this;
        }

        public TilePipelineBuilder WithDiskCache(string directory)
        {
            _diskCacheDirectory = directory;
            return this;
        }

        public TilePipelineBuilder EnableUnscaledMemoryCache(bool enabled = true)
        {
            _useUnscaledMemoryCache = enabled;
            return this;
        }

        public TilePipelineBuilder EnableScaledMemoryCache(bool enabled = true)
        {
            _useScaledMemoryCache = enabled;
            return this;
        }

        public TilePipelineBuilder WithUnscaledMemoryLimit(long bytes)
        {
            _unscaledMemoryLimitBytes = bytes;
            return this;
        }

        public TilePipelineBuilder WithScaledMemoryLimit(long bytes)
        {
            _scaledMemoryLimitBytes = bytes;
            return this;
        }

        public TilePipelineBuilder WithTileSize(int tileSize)
        {
            _tileSize = tileSize;
            return this;
        }

        public TilePipelineBuilder WithMetrics(ITileMetrics metrics)
        {
            _metrics = metrics ?? NullTileMetrics.Instance;
            return this;
        }

        // ---------------- Build ----------------

        public ITileProvider Build()
        {
            if (string.IsNullOrWhiteSpace(_tileServerUrl))
                throw new InvalidOperationException("Tile server URL must be specified.");

            ITileMetrics metrics = _metrics ?? NullTileMetrics.Instance;

            // ---- Base tile source ----
            ITileSource source =
                new HttpTileSource(_tileServerUrl!, _httpClient);

            // ---- Disk cache ----
            if (!string.IsNullOrWhiteSpace(_diskCacheDirectory))
            {
                source = new DiskTileCache(
                    _diskCacheDirectory!,
                    source,
                    metrics);
            }

            // ---- Unscaled memory cache ----
            if (_useUnscaledMemoryCache)
            {
                var unscaledCache = new MemoryCache(new MemoryCacheOptions
                {
                    SizeLimit = _unscaledMemoryLimitBytes
                });

                source = new UnscaledTileMemoryCache(
                    source,
                    unscaledCache,
                    metrics);
            }

            // ---- Scaling layer ----
            ITileProvider provider =
                new TileScaler(source, _tileSize);

            // ---- Scaled memory cache ----
            if (_useScaledMemoryCache)
            {
                var scaledCache = new MemoryCache(new MemoryCacheOptions
                {
                    SizeLimit = _scaledMemoryLimitBytes
                });

                provider = new ScaledTileMemoryCache(
                    provider,
                    scaledCache,
                    metrics);
            }

            return provider;
        }
    }
}
