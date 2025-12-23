using Engine2D.Calc;
using Engine2D.Primitives.Abstractions;
using System.Numerics;

namespace Engine2D.Primitives.Geometry
{
    public sealed class Line2D : IPrimitive2D
    {
        public WorldVector Start { get; }
        public WorldVector End { get; }

        public Line2D(WorldVector start, WorldVector end)
        {
            Start = start;
            End = end;
        }
    }
}
