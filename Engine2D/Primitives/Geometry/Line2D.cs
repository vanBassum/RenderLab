using Engine2D.Calc;
using Engine2D.Primitives.Abstractions;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using System.Numerics;

namespace Engine2D.Primitives.Geometry
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

        public void Render(RenderContext2D context)
        {
            var a = context.Viewport.WorldToScreen(Start, context.Camera);
            var b = context.Viewport.WorldToScreen(End, context.Camera);
            context.Graphics.DrawLine(a, b, ColorRgba.Lime, 1f);
        }
    }

}


