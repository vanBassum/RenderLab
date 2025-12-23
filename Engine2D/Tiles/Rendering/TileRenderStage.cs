using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Providers;
using System.Drawing;
using System.Numerics;

namespace Engine2D.Tiles.Rendering
{
    public sealed class TileRenderStage : IRenderer2D
    {
        private readonly IScaledTileSource _tileSource;
        private readonly TileGridProvider _gridProvider = new();

        public TileRenderStage(IScaledTileSource tileSource)
        {
            _tileSource = tileSource;
        }

        public void Render(in RenderContext2D context)
        {
            context.Viewport.GetWorldCoverage(context.Camera, out var worldMin, out var worldMax);

            foreach (var tile in _gridProvider.GetTiles(worldMin, worldMax, context.Camera.Zoom))
            {
                RenderTile(tile, context);
            }
        }

        private void RenderTile(TileRenderItem tileRenderItem, in RenderContext2D context)
        {
            var screenPos = context.Viewport.WorldToScreen(tileRenderItem.WorldPosition, context.Camera);
            var screenSize = tileRenderItem.WorldSize * context.Camera.Zoom;
            int tileSize = (int)MathF.Round(screenSize.X);


            var tileImage = _tileSource.GetTile(tileRenderItem.TileKey, tileSize);
            if (tileImage == null)
                return;

            context.Graphics.DrawImage(tileImage, screenPos);
        }
    }
}
