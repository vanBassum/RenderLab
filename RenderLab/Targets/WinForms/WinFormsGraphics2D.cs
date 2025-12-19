using Engine2D.Rendering.Graphics;
using Engine2D.Tiles.Images;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

namespace RenderLab.Targets.WinForms
{
    // =========================
    // WinForms backend adapter
    // =========================
    public sealed class WinFormsGraphics2D : IGraphics2D
    {
        private readonly Graphics _graphics;
        private readonly Font _font = new Font("Consolas", 10);

        public WinFormsGraphics2D(Graphics graphics)
        {
            _graphics = graphics;
            //_graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
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


        public void DrawImage(ITileImage image, Vector2 topLeft, Vector2 size)
        {
            switch (image)
            {
                // Fast path: cached bitmap
                case WinFormsTileImage wf:
                    _graphics.DrawImage(wf.Bitmap, topLeft.X, topLeft.Y, size.X, size.Y);
                    break;

                // Slow path: procedural RGBA buffer
                case ProceduralTileImage proc:
                    DrawProceduralImage(proc, topLeft, size);
                    break;
                default:
                    throw new NotSupportedException("Unsupported tile image type.");
            }
        }

        private void DrawProceduralImage(ProceduralTileImage image, Vector2 topLeft, Vector2 size)
        {
            using var bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

            var rect = new Rectangle(0, 0, image.Width, image.Height);
            var data = bitmap.LockBits(rect, ImageLockMode.WriteOnly, bitmap.PixelFormat);

            try
            {
                // Convert RGBA -> BGRA
                var bgra = new byte[image.Width * image.Height * 4];
                var src = image.PixelData;

                int srcIndex = 0;
                int dstIndex = 0;

                for (int i = 0; i < image.Width * image.Height; i++)
                {
                    bgra[dstIndex + 0] = src[srcIndex + 2]; // B
                    bgra[dstIndex + 1] = src[srcIndex + 1]; // G
                    bgra[dstIndex + 2] = src[srcIndex + 0]; // R
                    bgra[dstIndex + 3] = src[srcIndex + 3]; // A

                    srcIndex += 4;
                    dstIndex += 4;
                }

                Marshal.Copy(bgra, 0, data.Scan0, bgra.Length);
            }
            finally
            {
                bitmap.UnlockBits(data);
            }

            _graphics.DrawImage(bitmap, topLeft.X, topLeft.Y, size.X, size.Y);
        }

        private static Color ToColor(ColorRgba c)
            => Color.FromArgb(c.A, c.R, c.G, c.B);
    }
}
