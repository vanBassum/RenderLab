using System.Numerics;

namespace Engine2D.Tiles
{
    public sealed class GridTileProvider : ITileProvider
    {
        private readonly ITileImage _image;
        private readonly float _baseTileSize;
        private readonly int _tileZoom;

        public GridTileProvider(
            ITileImage image,
            int tileZoom,
            float baseTileSize = 256f)
        {
            _image = image;
            _tileZoom = tileZoom;
            _baseTileSize = baseTileSize;
        }

        public IEnumerable<TileRenderItem> GetTiles(Vector2 worldMin, Vector2 worldMax)
        {
            // World size of one tile at this zoom level
            float tileWorldSize = _baseTileSize * MathF.Pow(2, _tileZoom);
            var tileSize = new Vector2(tileWorldSize, tileWorldSize);

            int minX = (int)MathF.Floor(worldMin.X / tileWorldSize);
            int maxX = (int)MathF.Floor(worldMax.X / tileWorldSize);

            int minY = (int)MathF.Floor(worldMin.Y / tileWorldSize);
            int maxY = (int)MathF.Floor(worldMax.Y / tileWorldSize);

            for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                {
                    yield return new TileRenderItem(
                        _image,
                        worldPosition: new Vector2(
                            x * tileWorldSize,
                            y * tileWorldSize),
                        worldSize: tileSize,
                        tileX: x,
                        tileY: y,
                        tileZoom: _tileZoom);
                }
        }

    }

}
