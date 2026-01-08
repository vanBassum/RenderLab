using Engine2D.Calc;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using Engine2D.Tiles.Abstractions;

namespace Engine2D.Tiles.Rendering
{
    public sealed class TileRenderStage : IRenderer2D
    {
        private readonly ITileProvider _tiles;
        private readonly TilePyramidSpec _spec;

        public TileRenderStage(ITileProvider tiles, TilePyramidSpec spec)
        {
            _tiles = tiles;
            _spec = spec;
        }

        public void Render(in RenderContext2D context)
        {
            var worldMin = context.Viewport.GetWorldMin(context.Camera);
            var worldMax = context.Viewport.GetWorldMax(context.Camera);

            int tileLevel = ResolveTileLevel(context);
            float tileWorldSize = GetTileWorldSize(tileLevel);

            int x0 = WorldToTileIndex(worldMin.X, _spec.Origin.X, tileWorldSize);
            int y0 = WorldToTileIndex(worldMin.Y, _spec.Origin.Y, tileWorldSize);
            int x1 = WorldToTileIndex(worldMax.X, _spec.Origin.X, tileWorldSize);
            int y1 = WorldToTileIndex(worldMax.Y, _spec.Origin.Y, tileWorldSize);

            int maxIndex = (1 << tileLevel) - 1;
            x0 = Clamp(x0, 0, maxIndex);
            y0 = Clamp(y0, 0, maxIndex);
            x1 = Clamp(x1, 0, maxIndex);
            y1 = Clamp(y1, 0, maxIndex);

            for (int ty = y0; ty <= y1; ty++)
            {
                for (int tx = x0; tx <= x1; tx++)
                {
                    RenderTile(context, tx, ty, tileLevel, tileWorldSize);
                }
            }
        }

        private void RenderTile(in RenderContext2D context, int tx, int ty, int tileLevel, float tileWorldSize)
        {
            var tileId = new TileId(tx, ty, tileLevel);

            // World-space top-left and bottom-right of this tile
            var worldTopLeft = new WorldVector(
                _spec.Origin.X + tx * tileWorldSize,
                _spec.Origin.Y + ty * tileWorldSize);

            var worldBottomRight = new WorldVector(
                worldTopLeft.X + tileWorldSize,
                worldTopLeft.Y + tileWorldSize);

            var screenTopLeft = context.Viewport.WorldToScreen(worldTopLeft, context.Camera);
            var screenBottomRight = context.Viewport.WorldToScreen(worldBottomRight, context.Camera);
            var screenSize = new ScreenVector(
                screenBottomRight.X - screenTopLeft.X,
                screenBottomRight.Y - screenTopLeft.Y);

            int targetW = Math.Max(1, (int)MathF.Ceiling(MathF.Abs(screenSize.X)));
            int targetH = Math.Max(1, (int)MathF.Ceiling(MathF.Abs(screenSize.Y)));

            // Request the canonical tile pixel size from the provider.
            var query = new TileQuery
            {
                Tile = tileId,
                PixelWidth = targetW,
                PixelHeight = targetH
            };

            TileFetchResult result;
            try
            {
                result = _tiles.FetchAsync(query, default).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                DrawTilePlaceholder(
                    context,
                    screenTopLeft,
                    screenSize,
                    $"EXCEPTION\nL{tileLevel} X{tx} Y{ty}");
                return;
            }

            if (result.Image is not null)
            {
                context.Graphics.DrawImage(result.Image, screenTopLeft);
            }
            else
            {
                DrawTilePlaceholder(
                    context,
                    screenTopLeft,
                    screenSize,
                    $"LOADING / MISSING\nL{tileLevel} X{tx} Y{ty}");
            }
        }

        private static void DrawTilePlaceholder(in RenderContext2D context, ScreenVector screenTopLeft, ScreenVector screenSize, string text)
        {
            // Optional background
            context.Graphics.FillRect(screenTopLeft, screenSize, ColorRgba.DarkPink);

            // Border
            context.Graphics.DrawRect(screenTopLeft, screenSize, ColorRgba.Red);

            // Crude centering (debug-only, no font metrics)
            var lines = text.Split('\n');
            const float lineHeight = 14f;

            float totalTextHeight = lines.Length * lineHeight;
            float startY = screenTopLeft.Y + (screenSize.Y - totalTextHeight) / 2f;

            for (int i = 0; i < lines.Length; i++)
            {
                var pos = new ScreenVector(
                    screenTopLeft.X + screenSize.X / 2 - 40, // rough horizontal centering
                    (int)(startY + i * lineHeight));

                context.Graphics.DrawText(pos, lines[i], ColorRgba.White);
            }
        }

        private int ResolveTileLevel(in RenderContext2D ctx)
        {
            var w0 = ctx.Viewport.ScreenToWorld(ScreenVector.Zero, ctx.Camera);
            var w1 = ctx.Viewport.ScreenToWorld(new ScreenVector(1, 0), ctx.Camera);
            float worldPerPixel = MathF.Abs(w1.X - w0.X);

            float ideal = _spec.Level0WorldSize / (worldPerPixel * _spec.TilePixelSize);
            if (ideal <= 1f) return 0;

            int level = (int)MathF.Round(MathF.Log2(ideal));
            return Math.Max(level, 0);
        }

        private float GetTileWorldSize(int tileLevel)
        {
            return _spec.Level0WorldSize / (1 << tileLevel);
        }

        private static int WorldToTileIndex(float world, float origin, float tileWorldSize)
        {
            return (int)MathF.Floor((world - origin) / tileWorldSize);
        }

        private static int Clamp(int v, int min, int max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}
