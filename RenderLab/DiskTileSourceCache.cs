using Engine2D.Tiles.Abstractions;
using RenderLab.Targets.WinForms;

namespace RenderLab
{
    public sealed class DiskTileSourceCache : ITileSource
    {
        private readonly ITileSource _inner;
        private readonly string _rootDirectory;

        public DiskTileSourceCache(
            ITileSource inner,
            string rootDirectory)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _rootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));

            Directory.CreateDirectory(_rootDirectory);
        }

        public ITileImage? GetTile(int x, int y, int z)
        {
            var path = GetTilePath(x, y, z);

            if (File.Exists(path))
            {
                using var fs = File.OpenRead(path);
                var bitmap = new Bitmap(fs);
                return new WinFormsTileImage(bitmap);
            }

            // Cache miss → load from inner source
            var tile = _inner.GetTile(x, y, z);

            if(tile != null)
                SaveTile(tile, path);

            return tile;
        }

        private string GetTilePath(int x, int y, int z)
        {
            // /root/z/x/y.png
            return Path.Combine(
                _rootDirectory,
                z.ToString(),
                x.ToString(),
                $"{y}.png");
        }

        private static void SaveTile(ITileImage tile, string path)
        {
            var directory = Path.GetDirectoryName(path)!;
            Directory.CreateDirectory(directory);

            // Assumes WinFormsTileImage – explicit by design
            if (tile is not WinFormsTileImage wf)
                throw new InvalidOperationException(
                    "DiskTileSourceCache currently requires WinFormsTileImage");

            wf.Bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}

