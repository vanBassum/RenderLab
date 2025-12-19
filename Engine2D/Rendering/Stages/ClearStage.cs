using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;

namespace Engine2D.Rendering.Stages
{
    public sealed class ClearStage : IRenderer2D
    {
        private readonly ColorRgba _color;

        public ClearStage(ColorRgba color)
        {
            _color = color;
        }

        public void Render(in RenderContext2D context)
        {
            context.Graphics.Clear(_color);
        }
    }
}
