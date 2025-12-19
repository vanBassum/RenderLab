using Engine2D.Tiles.Rendering;
using System.Numerics;

namespace Engine2D.Tiles.Abstractions
{
    // =========================
    // Tile Providers (World)
    // =========================

    public interface ITileProvider
    {
        IEnumerable<TileRenderItem> GetTiles(Vector2 worldMin, Vector2 worldMax);
    }

}
