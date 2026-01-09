using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Models;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace RenderLab.Targets.WinForms
{
    public sealed class TileDiskCache : ITileProvider
    {
        private readonly ITileProvider _inner;
        private readonly string _rootDirectory;

        public TileDiskCache(ITileProvider inner, string rootDirectory)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _rootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));

            Directory.CreateDirectory(_rootDirectory);
        }

        public async ValueTask<TileFetchResult> FetchAsync(TileQuery query, CancellationToken token)
        {
            var tile = query.Tile;

            var path = GetTilePath(tile.X, tile.Y, tile.TileLevel);

            // 1) Disk hit
            if (File.Exists(path))
            {
                try
                {
                    var bitmap = LoadBitmapDetached(path);
                    return new TileFetchResult { Image = new WinFormsTileImage(bitmap) };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Disk cache read failed for {path}: {ex.Message}");
                    // Treat as miss; try inner source.
                }
            }

            // 2) Disk miss -> try inner
            var innerResult = await _inner.FetchAsync(query, token).ConfigureAwait(false);
            if (innerResult.Image is null)
                return innerResult; // still miss / in-flight / not found

            // 3) Write-through cache
            try
            {
                SaveTilePng(innerResult.Image, path);
                // Debug.WriteLine($"Disk cache miss for tile {tile}. Saved to {path}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Disk cache write failed for {path}: {ex.Message}");
                // Ignore disk write failure; still return the image.
            }

            return innerResult;
        }

        private string GetTilePath(int x, int y, int tileLevel)
        {
            // /root/<level>/<x>_<y>.png
            return Path.Combine(_rootDirectory, tileLevel.ToString(), $"{x}_{y}.png");
        }

        private static Bitmap LoadBitmapDetached(string path)
        {
            // Ensure the file stream can close immediately by cloning.
            using var fs = File.OpenRead(path);
            using var tmp = new Bitmap(fs);
            return new Bitmap(tmp);
        }

        private static void SaveTilePng(ITileImage tile, string path)
        {
            var directory = Path.GetDirectoryName(path)!;
            Directory.CreateDirectory(directory);

            // Explicitly WinForms-only by design:
            if (tile is not WinFormsTileImage wf)
                throw new InvalidOperationException(
                    $"Disk cache requires {nameof(WinFormsTileImage)} but got {tile.GetType().Name}");

            wf.Bitmap.Save(path, ImageFormat.Png);
        }
    }
}
