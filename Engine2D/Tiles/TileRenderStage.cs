using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using System.Numerics;

namespace Engine2D.Tiles
{
    public sealed class TileRenderStage : IRenderer2D
    {
        private readonly ITileProvider _provider;

        public TileRenderStage(ITileProvider provider)
        {
            _provider = provider;
        }

        public void Render(in RenderContext2D context)
        {
            var camera = context.Camera;

            // Viewport size comes from camera
            var viewport = camera.ViewportSize;

            var halfW = viewport.X * 0.5f / camera.Zoom;
            var halfH = viewport.Y * 0.5f / camera.Zoom;

            var worldMin = camera.Position - new Vector2(halfW, halfH);
            var worldMax = camera.Position + new Vector2(halfW, halfH);

            foreach (var tile in _provider.GetTiles(worldMin, worldMax))
            {
                RenderTile(tile, context);
            }
        }

        private static void RenderTile(TileRenderItem tile, in RenderContext2D context)
        {
            var camera = context.Camera;

            var screenPos = camera.WorldToScreen(tile.WorldPosition);
            var screenSize = tile.WorldSize * camera.Zoom;

            context.Graphics.DrawImage(tile.Image, screenPos, screenSize);
            context.Graphics.DrawRect(screenPos, screenSize, ColorRgba.Red);
        }
    }

}
