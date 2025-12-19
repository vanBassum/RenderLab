using Engine2D.Tiles.Abstractions;
using System.Collections.Concurrent;

namespace Engine2D.Tiles.Metrics
{
    public sealed class TileMetricsCounter : ITileMetrics
    {
        private readonly ConcurrentDictionary<string, long> _requests = new();
        private readonly ConcurrentDictionary<string, long> _hits = new();
        private readonly ConcurrentDictionary<string, long> _misses = new();
        private readonly ConcurrentDictionary<string, long> _evictions = new();
        private readonly ConcurrentDictionary<string, long> _downloads = new();

        public void TileRequest(string key) => Increment(_requests, key);
        public void TileHit(string key) => Increment(_hits, key);
        public void TileMiss(string key) => Increment(_misses, key);
        public void TileEvicted(string key) => Increment(_evictions, key);
        public void TileDownloaded(string key) => Increment(_downloads, key);

        // -------- Query API --------

        public IReadOnlyDictionary<string, long> Requests => _requests;
        public IReadOnlyDictionary<string, long> Hits => _hits;
        public IReadOnlyDictionary<string, long> Misses => _misses;
        public IReadOnlyDictionary<string, long> Evictions => _evictions;
        public IReadOnlyDictionary<string, long> Downloads => _downloads;

        private static void Increment(ConcurrentDictionary<string, long> dict, string key)
        {
            dict.AddOrUpdate(key, 1, static (_, v) => v + 1);
        }
    }

}
