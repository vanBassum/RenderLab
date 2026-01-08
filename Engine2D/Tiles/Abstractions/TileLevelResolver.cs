using Engine2D.Calc;
using Engine2D.Rendering.Pipeline;

namespace Engine2D.Tiles.Abstractions
{


    public static class TileLevelResolver
    {
        public static int ResolveTileLevel(in RenderContext2D ctx, TilePyramidSpec spec)
        {
            // Measure world units per screen pixel by sampling two adjacent pixels.
            var w0 = ctx.Viewport.ScreenToWorld(ScreenVector.Zero, ctx.Camera);
            var w1 = ctx.Viewport.ScreenToWorld(new ScreenVector(1, 0), ctx.Camera);
            float worldPerPixel = MathF.Abs(w1.X - w0.X);
    
            // At tile level L: world size covered by one tile = Level0WorldSize / 2^L
            // Therefore world size per tile pixel = (Level0WorldSize / 2^L) / TilePixelSize
            // Choose L so that world-per-screen-pixel ~= world-per-tile-pixel
            // worldPerPixel ~= Level0WorldSize / (2^L * TilePixelSize)
            //
            // Rearranged:
            // 2^L ~= Level0WorldSize / (worldPerPixel * TilePixelSize)
            float ideal = spec.Level0WorldSize / (worldPerPixel * spec.TilePixelSize);
    
            // Prevent log2 issues.
            if (ideal <= 1) return 0;
    
            int level = (int)MathF.Round(MathF.Log2(ideal));
            return Math.Max(level, 0);
        }
    }
}


