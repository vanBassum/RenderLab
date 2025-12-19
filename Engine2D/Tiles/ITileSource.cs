namespace Engine2D.Tiles
{
    public interface ITileSource
    {
        ITileImage GetTile(int x, int y, int zoom);
    }


}
