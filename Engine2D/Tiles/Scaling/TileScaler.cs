using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Images;

namespace Engine2D.Tiles.Scaling
{
    public sealed class TileScaler : ITileProvider
    {
        private readonly ITileSource _source;
        private readonly int _tileSize;

        public TileScaler(ITileSource source, int tileSize = 256)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _tileSize = tileSize;
        }

        public async ValueTask<TileImage?> GetTileAsync(int x, int y, float zoom)
        {
            int baseLevel = (int)MathF.Floor(zoom);
            int higherLevel = baseLevel + 1;

            // Prefer scaling down from higher resolution
            var tile = await TryGetScaledAsync(x, y, zoom, higherLevel).ConfigureAwait(false);
            if (tile != null)
                return tile;

            return await TryGetScaledAsync(x, y, zoom, baseLevel).ConfigureAwait(false);
        }

        private async ValueTask<TileImage?> TryGetScaledAsync(int x, int y, float requestedLevel, int sourceLevel)
        {
            if (sourceLevel < 1)
                return null;

            int levelDiff = sourceLevel - (int)MathF.Floor(requestedLevel);
            if (levelDiff < 0)
                return null;

            // Map tile coordinates across scale levels
            int factor = 1 << levelDiff;
            int srcX = x / factor;
            int srcY = y / factor;

            var baseTile = await _source
                .GetTileAsync(srcX, srcY, sourceLevel)
                .ConfigureAwait(false);

            if (baseTile == null)
                return null;

            // Compute relative scale between levels
            float relativeScale = MathF.Pow(2, requestedLevel - sourceLevel);

            int targetSize = (int)(_tileSize * relativeScale);
            if (targetSize <= 0)
                return null;

            var resized = new Bitmap(
                baseTile.Bitmap,
                new Size(targetSize, targetSize));

            return new TileImage(resized);
        }
    }


}



