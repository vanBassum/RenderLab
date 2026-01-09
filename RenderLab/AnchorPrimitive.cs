using Engine2D.Primitives.Abstractions;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using System.Numerics;

namespace RenderLab
{
    public class AnchorPrimitive : IPrimitive2D
    {
        public Vector2 Map;                 // The place the user clicked on the map
        public Vector2 CalculatedMap;       // The calculated map position based on game coords

        public AnchorPrimitive(Vector2 map, Vector2 calculatedMap)
        {
            Map = map;
            CalculatedMap = calculatedMap;
        }

        public void Render(RenderContext2D context)
        {
            var s1 = context.Viewport.WorldToScreen(Map, context.Camera);
            context.Graphics.DrawCircle(s1, 7f, ColorRgba.DarkPink, 1);

            var s2 = context.Viewport.WorldToScreen(CalculatedMap, context.Camera);
            context.Graphics.DrawCircle(s2, 5f, ColorRgba.Pink, 1);
        }
    }
}







