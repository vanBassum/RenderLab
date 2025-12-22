namespace Engine2D.Tiles.Abstractions
{
    public interface ITileSource
    {
        ITileImage? GetTile(TileKey tileKey);
    }



}
