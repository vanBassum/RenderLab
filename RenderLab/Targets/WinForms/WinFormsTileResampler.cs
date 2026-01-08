using Engine2D.Tiles.Abstractions;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace RenderLab.Targets.WinForms
{
    public sealed class WinFormsTileResampler : ITileResampler
    {
        private readonly ITileScaler _scaler;

        public WinFormsTileResampler(ITileScaler scaler)
        {
            _scaler = scaler ?? throw new ArgumentNullException(nameof(scaler));
        }

        public ITileImage Resample(ITileImage source, PixelRect srcRect, int targetWidth, int targetHeight)
        {
            // 1) Crop
            var cropped = Crop(source, srcRect);

            try
            {
                // 2) Scale using existing scaler
                return _scaler.Scale(cropped, targetWidth, targetHeight);
            }
            finally
            {
                cropped.Dispose();
            }
        }

        private static ITileImage Crop(ITileImage source, PixelRect srcRect)
        {
            if (source is not WinFormsTileImage wf)
                throw new NotSupportedException(
                    $"Tile image type {source.GetType().Name} not supported.");

            if (srcRect.Width <= 0 || srcRect.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(srcRect), "Crop rectangle must be positive.");

            var src = wf.Bitmap;

            // Clamp crop rect to source bounds to avoid GDI+ exceptions.
            int x = Math.Max(0, Math.Min(srcRect.X, src.Width - 1));
            int y = Math.Max(0, Math.Min(srcRect.Y, src.Height - 1));
            int w = Math.Max(1, Math.Min(srcRect.Width, src.Width - x));
            int h = Math.Max(1, Math.Min(srcRect.Height, src.Height - y));

            var dst = new Bitmap(w, h, src.PixelFormat);
            dst.SetResolution(96f, 96f);

            using (var g = Graphics.FromImage(dst))
            {
                g.Clear(Color.Transparent);
                g.SmoothingMode = SmoothingMode.None;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;

                // Copy pixels 1:1 from src region into dst
                g.DrawImage(
                    src,
                    new Rectangle(0, 0, w, h),      // destination
                    new Rectangle(x, y, w, h),      // source
                    GraphicsUnit.Pixel);
            }

            return new WinFormsTileImage(dst);
        }
    }
}



