using Engine2D.Rendering.Pipeline;
using Engine2D.Tiles.Abstractions;
using System.Numerics;

namespace Engine2D.Tiles.Stages
{
    public sealed class TileRenderStage : IRenderer2D
    {
        private readonly ITileProvider _tiles;
        private readonly int _tileSize;

        public TileRenderStage(ITileProvider tiles, int tileSize = 256)
        {
            _tiles = tiles;
            _tileSize = tileSize;
        }

        public void Render(in RenderContext2D context)
        {
            var camera = context.Camera;

            // Compute visible tile range (simplified)
            int zoom = (int)MathF.Round(MathF.Log2(camera.Zoom));
            zoom = Math.Clamp(zoom, 0, 20);

            var halfW = camera.ViewportSize.X / 2f;
            var halfH = camera.ViewportSize.Y / 2f;

            var topLeftWorld = camera.Position - new Vector2(halfW, halfH) / camera.Zoom;
            var bottomRightWorld = camera.Position + new Vector2(halfW, halfH) / camera.Zoom;

            int minX = (int)MathF.Floor(topLeftWorld.X / _tileSize);
            int maxX = (int)MathF.Ceiling(bottomRightWorld.X / _tileSize);
            int minY = (int)MathF.Floor(topLeftWorld.Y / _tileSize);
            int maxY = (int)MathF.Ceiling(bottomRightWorld.Y / _tileSize);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    var id = new TileId(zoom, x, y);

                    if (!_tiles.TryGetTile(id, out var image))
                        continue;

                    var worldPos = new Vector2(x * _tileSize, y * _tileSize);
                    var screenPos = camera.WorldToScreen(worldPos);
                    var size = new Vector2(_tileSize * camera.Zoom);

                    context.Graphics.DrawImage(image, screenPos, size);
                }
            }
        }
    }
}
