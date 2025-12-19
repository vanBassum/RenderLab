using Engine2D.Tiles;
using System.Drawing.Imaging;
using System.Numerics;

namespace RenderLab
{
    public sealed class DebugGridTileProvider : ITileProvider
    {
        private readonly int _tileZoom;
        private readonly float _baseTileSize;
        private readonly int _pixelSize;

        private readonly Dictionary<(int x, int y, int z), WinFormsTileImage> _cache
            = new();

        public DebugGridTileProvider(
            int tileZoom,
            float baseTileSize = 256f,
            int pixelSize = 256)
        {
            _tileZoom = tileZoom;
            _baseTileSize = baseTileSize;
            _pixelSize = pixelSize;
        }

        public IEnumerable<TileRenderItem> GetTiles(
            Vector2 worldMin,
            Vector2 worldMax)
        {
            float tileWorldSize = _baseTileSize * MathF.Pow(2, _tileZoom);
            var tileWorldSizeVec = new Vector2(tileWorldSize);

            int minX = (int)MathF.Floor(worldMin.X / tileWorldSize);
            int maxX = (int)MathF.Floor(worldMax.X / tileWorldSize);
            int minY = (int)MathF.Floor(worldMin.Y / tileWorldSize);
            int maxY = (int)MathF.Floor(worldMax.Y / tileWorldSize);

            for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                {
                    var key = (x, y, _tileZoom);

                    if (!_cache.TryGetValue(key, out var image))
                    {
                        image = CreateDebugTile(x, y, _tileZoom);
                        _cache[key] = image;
                    }

                    yield return new TileRenderItem(
                        image,
                        worldPosition: new Vector2(
                            x * tileWorldSize,
                            y * tileWorldSize),
                        worldSize: tileWorldSizeVec,
                        tileX: x,
                        tileY: y,
                        tileZoom: _tileZoom);
                }
        }

        private WinFormsTileImage CreateDebugTile(int x, int y, int z)
        {
            var bmp = new Bitmap(_pixelSize, _pixelSize, PixelFormat.Format32bppArgb);

            using var g = Graphics.FromImage(bmp);
            g.Clear(((x + y) & 1) == 0 ? Color.FromArgb(40, 40, 40) : Color.FromArgb(60, 60, 60));

            using var pen = new Pen(Color.Red, 2);
            g.DrawRectangle(pen, 0, 0, _pixelSize - 1, _pixelSize - 1);

            using var font = new Font("Consolas", 14, FontStyle.Bold);
            using var brush = new SolidBrush(Color.White);

            string text = $"{x},{y},{z}";
            var size = g.MeasureString(text, font);

            g.DrawString(
                text,
                font,
                brush,
                (_pixelSize - size.Width) / 2,
                (_pixelSize - size.Height) / 2);

            return new WinFormsTileImage(bmp);
        }
    }





}



