using Engine2D.Tiles.Abstractions;
using System.Numerics;

namespace Engine2D.Tiles.Rendering
{
    public sealed class ScaledTileSource : IScaledTileSource
    {
        private readonly ITileSource _source;
        private readonly ITileScaler _scaler;

        public ScaledTileSource(ITileSource source, ITileScaler scaler)
        {
            _source = source;
            _scaler = scaler;
        }

        public ITileImage? GetTile(TileKey tileKey, Vector2 screenSize)
        {
            var baseTile = _source.GetTile(tileKey);
            if (baseTile == null)
                return null;

            int targetWidth = (int)MathF.Round(screenSize.X);
            int targetHeight = (int)MathF.Round(screenSize.Y);

            // No scaling needed → return original
            if (targetWidth == tileKey.PixelSize &&
                targetHeight == tileKey.PixelSize)
            {
                return baseTile;
            }

            // Scaling needed → delegate
            return _scaler.Scale(baseTile, targetWidth, targetHeight);
        }
    }


}
