namespace Engine2D.Tiles.Abstractions
{
    public interface ITileSource
    {
        ITileImage? GetTile(int x, int y, int zoom);
    }


}
