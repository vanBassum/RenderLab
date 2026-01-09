using Engine2D.Primitives.Abstractions;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using System.Numerics;

namespace Engine2D.Primitives.Geometry
{
    public sealed class PlayerPrimitive : IPrimitive2D
    {
        public Vector2 Position { get; set; }

        public PlayerPrimitive(Vector2 position)
        {
            Position = position;
        }

        public void Render(RenderContext2D context)
        {
            var screenPos = context.Viewport.WorldToScreen(Position, context.Camera);
            context.Graphics.DrawCircle(screenPos, 5f, ColorRgba.Red, 3);
        }
    }
}
