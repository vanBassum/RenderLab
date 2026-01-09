using Engine2D.Calc;
using Engine2D.Primitives.Abstractions;
using Engine2D.Primitives.Geometry;
using Engine2D.Rendering.Camera;
using System.Numerics;

namespace Engine2D.Primitives.Filtering
{
    public sealed class WorldCoverageLineFilter
    {
        public bool IsVisible(IPrimitive2D primitive, Camera2D camera, IViewport2D viewport, Vector2 worldMin, Vector2 worldMax)
        {
            if (primitive is not Line2D line)
                return true; // unknown primitives pass through

            return LineIntersectsAabb(line.Start, line.End, worldMin, worldMax);
        }

        private static bool LineIntersectsAabb(Vector2 a, Vector2 b, Vector2 min, Vector2 max)
        {
            float minX = MathF.Min(a.X, b.X);
            float maxX = MathF.Max(a.X, b.X);
            float minY = MathF.Min(a.Y, b.Y);
            float maxY = MathF.Max(a.Y, b.Y);

            if (maxX < min.X || minX > max.X)
                return false;

            if (maxY < min.Y || minY > max.Y)
                return false;

            return true;
        }
    }

}
