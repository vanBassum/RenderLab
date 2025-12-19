using Engine2D.Rendering.Camera;
using System.Numerics;

namespace Engine2D.Tiles
{
    public sealed class TileScaler : ITileProvider
    {
        private readonly ITileProvider _source;
        private readonly Camera2D _camera;
        private readonly ITileImageScaler _imageScaler;
        private readonly int _baseTilePixelSize;

        public TileScaler(ITileProvider source, Camera2D camera, ITileImageScaler imageScaler, int baseTilePixelSize = 256)
        {
            _source = source;
            _camera = camera;
            _imageScaler = imageScaler;
            _baseTilePixelSize = baseTilePixelSize;
        }

        public IEnumerable<TileRenderItem> GetTiles(Vector2 worldMin, Vector2 worldMax)
        {
            foreach (var tile in _source.GetTiles(worldMin, worldMax))
            {
                yield return Scale(tile);
            }
        }

        private TileRenderItem Scale(TileRenderItem tile)
        {
            // Discrete zoom already handled by tile selection
            float discreteScale = MathF.Pow(2f, tile.TileZoom);

            // Only scale for the fractional part
            float fractionalZoom = _camera.Zoom / discreteScale;

            int targetPixelSize =
                (int)MathF.Round(_baseTilePixelSize * fractionalZoom);

            if (targetPixelSize <= 0)
                targetPixelSize = 1;

            var scaledImage = _imageScaler.Scale(
                tile.Image,
                targetPixelSize);

            return new TileRenderItem(
                scaledImage,
                tile.WorldPosition,
                tile.WorldSize,
                tile.TileX,
                tile.TileY,
                tile.TileZoom);
        }

    }
}
