using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Providers;
using Engine2D.Tiles.Scaling;
using System;

namespace Engine2D.Tiles.Rendering
{
    public sealed class TileRenderStage : IRenderer2D
    {
        private readonly ITileSource _tileSource;

        public TileRenderStage(ITileSource tileSource)
        {
            _tileSource = tileSource;
        }

        public void Render(in RenderContext2D context)
        {
            ITileGridProvider _gridProvider = new TileGridProvider(context.Camera);
            TileCoverageProvider coverage = new TileCoverageProvider(context.Camera);

            coverage.GetWorldCoverage(context.Camera.ViewportSize, out var worldMin, out var worldMax);

            foreach (var tile in _gridProvider.GetTiles(worldMin, worldMax))
            {
                RenderTile(tile, context);
            }
        }

        private void RenderTile(TileRenderItem tile, in RenderContext2D context)
        {
            var camera = context.Camera;
            var tileImage = _tileSource.GetTile(tile.TileKey);

            if (tileImage == null)
                return;

            // For now no scaling!
            context.Graphics.DrawImage(tileImage, tile.ScreenPosition);
        }
    }
}




