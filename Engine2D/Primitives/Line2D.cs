using System.Numerics;

namespace Engine2D.Primitives
{
    public sealed class Line2D : IPrimitive2D
    {
        public Vector2 Start { get; }
        public Vector2 End { get; }

        public Line2D(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
    }
}
