using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;

namespace Engine2D.Tiles
{
    public sealed class TileRenderStage : IRenderer2D
    {
        private readonly ITileProvider _provider;
        private readonly TileCoverageProvider _coverage;

        public TileRenderStage(ITileProvider provider, TileCoverageProvider coverage)
        {
            _provider = provider;
            _coverage = coverage;
        }

        public void Render(in RenderContext2D context)
        {
            _coverage.GetWorldCoverage(context.Camera.ViewportSize, out var worldMin, out var worldMax);

            foreach (var tile in _provider.GetTiles(worldMin, worldMax))
            {
                RenderTile(tile, context);
            }
        }

        private static void RenderTile(TileRenderItem tile, in RenderContext2D context)
        {
            var camera = context.Camera;

            var screenPos = camera.WorldToScreen(tile.WorldPosition);

            // NO SIZE PASSED → no scaling in graphics
            context.Graphics.DrawImage(tile.Image, screenPos);

            // Optional debug outline
            var screenSize = tile.WorldSize * camera.Zoom;
            context.Graphics.DrawRect(screenPos, screenSize, ColorRgba.Pink);
        }

    }

}
