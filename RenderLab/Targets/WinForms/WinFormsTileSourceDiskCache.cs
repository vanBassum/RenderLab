using Engine2D.Tiles.Abstractions;
using RenderLab.Targets.WinForms;
using System.Diagnostics;

namespace Engine2D.Tiles.Caching
{
    public sealed class WinFormsTileSourceDiskCache : ITileSource
    {
        private readonly ITileSource _inner;
        private readonly string _rootDirectory;

        public WinFormsTileSourceDiskCache(ITileSource inner, string rootDirectory)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _rootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));

            Directory.CreateDirectory(_rootDirectory);
        }

        public ITileImage? GetTile(TileKey tileKey)
        {
            var path = GetTilePath(tileKey.X, tileKey.Y, tileKey.Z);

            if (File.Exists(path))
            {
                using var fs = File.OpenRead(path);
                var bitmap = new Bitmap(fs);
                return new WinFormsTileImage(bitmap);
            }

            // Cache miss ? load from inner source
            var tile = _inner.GetTile(tileKey);
            if (tile != null)
            {
                SaveTile(tile, path);
                //Debug.WriteLine($"Disk cache miss for tile {tileKey}. Saved to {path}");
            }

            return tile;
        }

        private string GetTilePath(int x, int y, int z)
        {
            // /root/z/x_y.png
            return Path.Combine(_rootDirectory, z.ToString(), $"{x}_{y}.png");
        }

        private static void SaveTile(ITileImage tile, string path)
        {
            var directory = Path.GetDirectoryName(path)!;
            Directory.CreateDirectory(directory);

            // Assumes WinFormsTileImage – explicit by design
            if (tile is not WinFormsTileImage wf)
                throw new InvalidOperationException("DiskTileSourceCache currently requires WinFormsTileImage");

            wf.Bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        }

    }
}

