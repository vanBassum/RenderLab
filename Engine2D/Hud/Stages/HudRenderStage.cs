using Engine2D.Primitives.Abstractions;
using Engine2D.Primitives.Geometry;
using Engine2D.Rendering.Pipeline;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace Engine2D.Hud.Stages
{

    // =========================
    // HUD render stage
    // =========================
    public sealed class HudRenderStage : IRenderer2D
    {
        private readonly Func<IEnumerable<IHudElement2D>> _getElements;

        public HudRenderStage(Func<IEnumerable<IHudElement2D>> getElements)
        {
            _getElements = getElements;
        }

        public void Render(in RenderContext2D context)
        {
            foreach (var element in _getElements())
            {
                RenderElement(context, element);
            }
        }

        private void RenderElement(in RenderContext2D context, IHudElement2D element)
        {
            switch (element)
            {
                case HudLabel2D label:
                    RenderLabel(context, label);
                    break;
                default:
                    throw new Exception($"Type not supported {element.GetType()}");
            }

        }


        private void RenderLabel(in RenderContext2D context, HudLabel2D label) 
        {
            context.Graphics.FillRect(label.Position, label.Size, label.Background);
            context.Graphics.DrawText(label.Position + label.Padding, label.Text, label.Foreground);
        }
    }
}


