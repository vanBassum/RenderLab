using Engine2D.Primitives;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;

namespace Engine2D.Rendering.Stages
{
    public sealed class PrimitiveRenderStage : IRenderer2D
    {
        private readonly Func<IEnumerable<IPrimitive2D>> _getPrimitives;

        public PrimitiveRenderStage(Func<IEnumerable<IPrimitive2D>> getPrimitives)
        {
            _getPrimitives = getPrimitives;
        }

        public void Render(in RenderContext2D context)
        {
            foreach (var primitive in _getPrimitives())
            {
                RenderPrimitive(primitive, context);
            }
        }

        private static void RenderPrimitive(IPrimitive2D primitive, in RenderContext2D context)
        {
            switch (primitive)
            {
                case Line2D line:
                    RenderLine(line, context);
                    break;

                default:
                    throw new NotSupportedException($"Primitive {primitive.GetType().Name} not supported.");
            }
        }

        private static void RenderLine(Line2D line, in RenderContext2D context)
        {
            var a = context.Viewport.WorldToScreen(line.Start, context.Camera);
            var b = context.Viewport.WorldToScreen(line.End, context.Camera);

            context.Graphics.DrawLine(a, b, ColorRgba.Lime, 1f);
        }
    }
}
