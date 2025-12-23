using Engine2D.Rendering.Graphics;
using Engine2D.Tiles.Abstractions;
using System.Numerics;

namespace RenderLab.Targets.WinForms
{
    // =========================
    // WinForms backend adapter
    // =========================
    public sealed class WinFormsGraphics2D : IGraphics2D, IDisposable
    {
        private readonly Graphics _graphics;
        private readonly Font _font;

        public WinFormsGraphics2D(Graphics graphics)
        {
            _graphics = graphics;
            _font = new Font("Consolas", 10);
        }

        public void Dispose()
        {
            _font.Dispose();
        }

        public void Clear(ColorRgba color)
        {
            _graphics.Clear(ToColor(color));
        }

        public void DrawLine(Vector2 a, Vector2 b, ColorRgba color, float thickness)
        {
            using var pen = new Pen(ToColor(color), thickness);
            _graphics.DrawLine(pen, a.X, a.Y, b.X, b.Y);
        }

        public void DrawText(Vector2 position, string text, ColorRgba color)
        {
            using var brush = new SolidBrush(ToColor(color));
            _graphics.DrawString(text, _font, brush, position.X, position.Y);
        }

        public void FillRect(Vector2 position, Vector2 size, ColorRgba color)
        {
            using var brush = new SolidBrush(ToColor(color));
            _graphics.FillRectangle(brush, position.X, position.Y, size.X, size.Y);
        }

        private static Color ToColor(ColorRgba c)
            => Color.FromArgb(c.A, c.R, c.G, c.B);

        public void DrawImage(ITileImage image, Vector2 topLeft)
        {
            if (image is not WinFormsTileImage wf)
                throw new NotSupportedException(
                    $"Tile image type {image.GetType().Name} not supported.");

            _graphics.DrawImage(wf.Bitmap, topLeft.X, topLeft.Y);
        }

        public void DrawImage(ITileImage image, Vector2 topLeft, Vector2 size)
        {
            if (image is not WinFormsTileImage wf)
                throw new NotSupportedException(
                    $"Tile image type {image.GetType().Name} not supported.");

            _graphics.DrawImage(wf.Bitmap, topLeft.X, topLeft.Y, size.X, size.Y);
        }

        public void DrawRect(Vector2 screenPos, Vector2 screenSize, ColorRgba color)
        {
            using var pen = new Pen(ToColor(color));
            _graphics.DrawRectangle(
                pen,
                screenPos.X,
                screenPos.Y,
                screenSize.X,
                screenSize.Y);
        }


    }
}
