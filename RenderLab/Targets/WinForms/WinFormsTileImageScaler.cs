using Engine2D.Tiles.Abstractions;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace RenderLab.Targets.WinForms
{
    public sealed class WinFormsTileImageScaler : ITileScaler
    {
        public ITileImage Scale(ITileImage source, int targetWidth, int targetHeight)
        {
            if (source is not WinFormsTileImage wf)
                throw new NotSupportedException(
                    $"Tile image type {source.GetType().Name} not supported.");

            var src = wf.Bitmap;

            var dst = CreateDestinationBitmap(src, targetWidth, targetHeight);

            float scaleX = (float)targetWidth / src.Width;
            float scaleY = (float)targetHeight / src.Height;

            using (var g = Graphics.FromImage(dst))
            {
                g.Clear(Color.Transparent);
                ConfigureGraphics(g, scaleX, scaleY);
                DrawScaled(g, src, targetWidth, targetHeight);
            }

            return new WinFormsTileImage(dst);
        }

        // -------------------------
        // Helpers
        // -------------------------

        private static Bitmap CreateDestinationBitmap(Bitmap src, int targetWidth, int targetHeight)
        {
            var dst = new Bitmap(targetWidth, targetHeight, src.PixelFormat);
            dst.SetResolution(96f, 96f);
            return dst;
        }

        private static void ConfigureGraphics(Graphics g, float scaleX, float scaleY)
        {
            g.SmoothingMode = SmoothingMode.None;

            if (IsDownscale(scaleX, scaleY))
            {
                ConfigureForDownscale(g);
            }
            else if (IsIntegerUpscale(scaleX, scaleY))
            {
                ConfigureForIntegerUpscale(g);
            }
            else
            {
                ConfigureForFractionalUpscale(g);
            }
        }

        private static void ConfigureForDownscale(Graphics g)
        {
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        private static void ConfigureForIntegerUpscale(Graphics g)
        {
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
        }

        private static void ConfigureForFractionalUpscale(Graphics g)
        {
            g.InterpolationMode = InterpolationMode.Bilinear;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        private static bool IsDownscale(float scaleX, float scaleY)
            => scaleX < 1.0f || scaleY < 1.0f;

        private static bool IsIntegerUpscale(float scaleX, float scaleY)
            => scaleX >= 1.0f &&
               NearlyInteger(scaleX) &&
               NearlyInteger(scaleY);

        private static bool NearlyInteger(float value)
            => MathF.Abs(value - MathF.Round(value)) < 0.001f;

        private static void DrawScaled(Graphics g, Bitmap src, int targetWidth, int targetHeight)
        {
            using var attrs = new ImageAttributes();

            attrs.SetWrapMode(WrapMode.TileFlipXY);

            g.DrawImage(
                src,
                new Rectangle(0, 0, targetWidth, targetHeight),
                0,
                0,
                src.Width,
                src.Height,
                GraphicsUnit.Pixel,
                attrs);
        }
    }
}
