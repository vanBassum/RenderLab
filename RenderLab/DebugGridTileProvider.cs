using Engine2D.Tiles;
using System.Drawing.Imaging;
using System.Numerics;

namespace RenderLab
{
    public sealed class DebugTileSource : ITileSource
    {
        private readonly Dictionary<(int x, int y, int z), WinFormsTileImage> _cache
            = new();

        private readonly int _pixelSize;

        public DebugTileSource(int pixelSize = 256)
        {
            _pixelSize = pixelSize;
        }

        public ITileImage GetTile(int x, int y, int z)
        {
            var key = (x, y, z);

            if (_cache.TryGetValue(key, out var cached))
                return cached;

            var created = CreateTile(x, y, z);
            _cache[key] = created;
            return created;
        }

        private WinFormsTileImage CreateTile(int x, int y, int z)
        {
            var bmp = new Bitmap(_pixelSize, _pixelSize, PixelFormat.Format32bppArgb);

            using var g = Graphics.FromImage(bmp);

            // Checker-ish background based on position
            var baseColor =
                ((x + y) & 1) == 0
                ? Color.FromArgb(40, 40, 40)
                : Color.FromArgb(60, 60, 60);

            g.Clear(baseColor);

            // Red border
            using (var pen = new Pen(Color.Red, 2))
            {
                g.DrawRectangle(pen, 0, 0, _pixelSize - 1, _pixelSize - 1);
            }

            // Centered text: x,y,z
            using var font = new Font("Consolas", 14, FontStyle.Bold);
            using var brush = new SolidBrush(Color.White);

            string text = $"{x},{y},{z}";
            var size = g.MeasureString(text, font);

            g.DrawString(
                text,
                font,
                brush,
                (_pixelSize - size.Width) * 0.5f,
                (_pixelSize - size.Height) * 0.5f);

            return new WinFormsTileImage(bmp);
        }
    }
}
