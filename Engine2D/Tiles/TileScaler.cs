using Engine2D.Rendering.Camera;
using System.Numerics;

namespace Engine2D.Tiles
{
    public sealed class TileScaler : ITileProvider
    {
        private readonly ITileProvider _source;
        private readonly Camera2D _camera;

        public TileScaler(ITileProvider source, Camera2D camera)
        {
            _source = source;
            _camera = camera;
        }

        public IEnumerable<TileRenderItem> GetTiles(Vector2 worldMin, Vector2 worldMax)
        {
            // For now: pass-through.
            // Later: compose/scale images, stitch 4 tiles into 1, etc.
            return _source.GetTiles(worldMin, worldMax);
        }
    }
}
