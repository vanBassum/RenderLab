namespace Engine2D.Tiles.Abstractions
{
    public interface ITileMetrics
    {
        void TileRequest(string key);
        void TileHit(string key);
        void TileMiss(string key);
        void TileEvicted(string key);
        void TileDownloaded(string key);
    }

}



