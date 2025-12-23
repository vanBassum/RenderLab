using Engine2D.Calc;
using Engine2D.Rendering.Graphics;
using Engine2D.Tiles.Abstractions;

namespace RenderLab.Targets.WinForms
{
    public sealed class WinFormsGraphics2D : IGraphics2D, IDisposable
    {
        private readonly Graphics _graphics;
        private readonly WinFormsTextDrawer _text;

        public WinFormsGraphics2D(Graphics graphics, WinFormsTextDrawer text)
        {
            _graphics = graphics;
            _text = text;
        }

        public void Clear(ColorRgba color)
        {
            _graphics.Clear(ToColor(color));
        }

        public void DrawLine(ScreenVector a, ScreenVector b, ColorRgba color, float thickness)
        {
            using var pen = new Pen(ToColor(color), thickness);
            _graphics.DrawLine(pen, a.X, a.Y, b.X, b.Y);
        }

        public void DrawText(ScreenVector position, string text, ColorRgba color)
        {
            if (string.IsNullOrEmpty(text))
                return;

            _text.Draw(_graphics, position.X, position.Y, text, ToColor(color));
        }

        public void FillRect(ScreenVector position, ScreenVector size, ColorRgba color)
        {
            using var brush = new SolidBrush(ToColor(color));
            _graphics.FillRectangle(brush, position.X, position.Y, size.X, size.Y);
        }


        public void DrawImage(ITileImage image, ScreenVector topLeft)
        {
            if (image is not WinFormsTileImage wf)
                throw new NotSupportedException($"Tile image type {image.GetType().Name} not supported.");

            _graphics.DrawImage(wf.Bitmap, topLeft.X, topLeft.Y);
        }

        public void DrawImage(ITileImage image, ScreenVector topLeft, ScreenVector size)
        {
            if (image is not WinFormsTileImage wf)
                throw new NotSupportedException($"Tile image type {image.GetType().Name} not supported.");

            _graphics.DrawImage(wf.Bitmap, topLeft.X, topLeft.Y, size.X, size.Y);
        }

        public void DrawRect(ScreenVector screenPos, ScreenVector screenSize, ColorRgba color)
        {
            using var pen = new Pen(ToColor(color));
            _graphics.DrawRectangle(pen, screenPos.X, screenPos.Y, screenSize.X, screenSize.Y);
        }

        public void Dispose()
        {
        }

        private static Color ToColor(ColorRgba c) => Color.FromArgb(c.A, c.R, c.G, c.B);
    }
}


