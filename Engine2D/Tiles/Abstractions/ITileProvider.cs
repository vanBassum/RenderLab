using Engine2D.Tiles.Images;

namespace Engine2D.Tiles.Abstractions
{
    public interface ITileProvider
    {
        bool TryGetTile(TileId id, out ITileImage image);
    }
}
