using Engine2D.Tiles.Abstractions;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace RenderLab.Targets.WinForms
{
 /*   public sealed class WinFormsTileImageScaler : ITileImageScaler
    {
        public ITileImage Scale(ITileImage source, int targetPixelSize)
        {
            if (source is not WinFormsTileImage wf)
                throw new NotSupportedException(
                    $"Tile image type {source.GetType().Name} not supported.");

            var src = wf.Bitmap;

            // Fast path: no scaling needed
            if (src.Width == targetPixelSize &&
                src.Height == targetPixelSize)
            {
                return source;
            }

            var dst = new Bitmap(
                targetPixelSize,
                targetPixelSize,
                PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(dst))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBilinear;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.SmoothingMode = SmoothingMode.None;

                g.DrawImage(
                    src,
                    new Rectangle(0, 0, targetPixelSize, targetPixelSize),
                    new Rectangle(0, 0, src.Width, src.Height),
                    GraphicsUnit.Pixel);
            }

            return new WinFormsTileImage(dst);
        }
    }*/

}



