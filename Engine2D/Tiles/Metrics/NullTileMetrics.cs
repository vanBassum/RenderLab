using Engine2D.Tiles.Abstractions;

namespace Engine2D.Tiles.Metrics
{
    public sealed class NullTileMetrics : ITileMetrics
    {
        public static readonly ITileMetrics Instance = new NullTileMetrics();

        private NullTileMetrics() { }

        public void TileRequest(string key) { }
        public void TileHit(string key) { }
        public void TileMiss(string key) { }
        public void TileEvicted(string key) { }
        public void TileDownloaded(string key) { }
    }
}