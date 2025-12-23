using Engine2D.Calc;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;

namespace Engine2D.Hud.Stages
{
    // =========================
    // Simple label element
    // =========================
    public sealed class HudLabel2D : IHudElement2D
    {
        public ScreenVector Position { get; set; }
        public string Text { get; set; }
        public ScreenVector Padding { get; set; } = new(6, 4);
        public ScreenVector Size { get; set; }
        public ColorRgba Background { get; set; } = new(0, 0, 0, 160);
        public ColorRgba Foreground { get; set; } = ColorRgba.Lime;

        public HudLabel2D(ScreenVector position, ScreenVector size, string text)
        {
            Position = position;
            Size = size;
            Text = text;
        }

    }
}


