using Engine2D.Rendering.Graphics;
using System.Numerics;

namespace RenderLab.Targets.WinForms
{
    // =========================
    // WinForms backend adapter
    // =========================

    public sealed class WinFormsGraphics2D : IGraphics2D
    {
        private readonly Graphics _g;
        private readonly Font _font = new Font("Consolas", 10);

        public WinFormsGraphics2D(Graphics g)
        {
            _g = g;
            _g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        public void Clear(ColorRgba color)
        {
            _g.Clear(ToColor(color));
        }

        public void DrawLine(Vector2 a, Vector2 b, ColorRgba color, float thickness)
        {
            using var pen = new Pen(ToColor(color), thickness);
            _g.DrawLine(pen, a.X, a.Y, b.X, b.Y);
        }
        public void DrawText(Vector2 position, string text, ColorRgba color)
        {
            using var brush = new SolidBrush(ToColor(color));
            _g.DrawString(text, _font, brush, position.X, position.Y);
        }

        private static Color ToColor(ColorRgba c)
            => Color.FromArgb(c.A, c.R, c.G, c.B);
    }
}
