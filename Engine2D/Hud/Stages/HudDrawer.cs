using Engine2D.Calc;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;

namespace Engine2D.Hud.Stages
{
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


