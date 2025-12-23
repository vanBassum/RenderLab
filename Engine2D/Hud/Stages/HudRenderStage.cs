using Engine2D.Calc;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;

namespace Engine2D.Hud.Stages
{

    // =========================
    // HUD render stage
    // =========================
    public sealed class HudRenderStage : IRenderer2D
    {
        private readonly Action<HudDrawer> _Draw;

        public HudRenderStage(Action<HudDrawer> draw)
        {
            _Draw = draw;
        }

        public void Render(in RenderContext2D context)
        {
            HudDrawer drawer = new(context);
            _Draw(drawer);
        }
    }



    public class HudDrawer
    {
        public RenderContext2D Context { get; }

        public HudDrawer(RenderContext2D context)
        {
            Context = context;
        }

        public void DrawLabel(ScreenVector position, ScreenVector size, string text)
        {
            ScreenVector Padding = new(6, 4);
            ColorRgba Background = new(0, 0, 0, 160);
            ColorRgba Foreground = ColorRgba.Lime;


            Context.Graphics.FillRect(position, size, Background);
            Context.Graphics.DrawText(position + Padding, text, Foreground);
        }
    }
}


