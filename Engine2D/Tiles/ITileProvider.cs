using System.Numerics;

namespace Engine2D.Tiles
{
    // =========================
    // Tile Providers (World)
    // =========================

    public interface ITileProvider
    {
        IEnumerable<TileRenderItem> GetTiles(Vector2 worldMin, Vector2 worldMax);
    }

}
