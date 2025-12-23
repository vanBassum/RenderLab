using Engine2D.Input;
using Engine2D.Rendering.Pipeline;
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

        public ITileImage? GetTile(TileKey tileKey, int screenSize)
        {
            var baseTile = _source.GetTile(tileKey);
            if (baseTile == null)
                return null;

            // No scaling needed → return original
            if (screenSize == tileKey.PixelSize)
            {
                return baseTile;
            }

            // Scaling needed → delegate
            return _scaler.Scale(baseTile, screenSize, screenSize);
        }
    }


}
