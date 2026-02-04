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
        public ColorRgba Color { get; set; } = ColorRgba.Red;

        public Line2D(Vector2 start, Vector2 end, ColorRgba color)
        {
            Start = start;
            End = end;
            Color = color;
        }

        public void Render(RenderContext2D context)
        {
            var a = context.Viewport.WorldToScreen(Start, context.Camera);
            var b = context.Viewport.WorldToScreen(End, context.Camera);
            context.Graphics.DrawLine(a, b, Color, 1f);
        }
    }

}


