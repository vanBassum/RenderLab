using System.Numerics;

namespace Engine2D.Tiles.Abstractions
{
    public interface IScaledTileSource
    {
        ITileImage? GetTile(TileKey tileKey, Vector2 ScreenSize);
    }


}
