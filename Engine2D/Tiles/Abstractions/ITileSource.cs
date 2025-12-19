using Engine2D.Tiles.Images;

namespace Engine2D.Tiles.Abstractions
{
    public interface ITileSource
    {
        ValueTask<TileImage?> GetTileAsync(int x, int y, int zoom);
    }
}



