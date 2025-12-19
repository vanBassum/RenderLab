using Engine2D.Rendering.Camera;
using System.Numerics;

namespace Engine2D.Tiles
{
    public sealed class GridTileProvider : ITileProvider
    {
        private readonly ITileSource _source;
        private readonly TileZoomSelector _zoomSelector;
        private readonly Camera2D _camera;
        private readonly float _baseTileSize;

        public GridTileProvider(
            ITileSource source,
            TileZoomSelector zoomSelector,
            Camera2D camera,
            float baseTileSize = 256f)
        {
            _source = source;
            _zoomSelector = zoomSelector;
            _camera = camera;
            _baseTileSize = baseTileSize;
        }

        public IEnumerable<TileRenderItem> GetTiles(Vector2 worldMin, Vector2 worldMax)
        {
            int tileZoom = _zoomSelector.SelectTileZoom(_camera.Zoom);

            // IMPORTANT: higher zoom => smaller world area per tile
            float tileWorldSize = _baseTileSize / MathF.Pow(2f, tileZoom);
            var tileWorldSizeVec = new Vector2(tileWorldSize, tileWorldSize);

            int minX = (int)MathF.Floor(worldMin.X / tileWorldSize);
            int minY = (int)MathF.Floor(worldMin.Y / tileWorldSize);

            // IMPORTANT: include partially visible tile on max edge
            int maxX = (int)MathF.Ceiling(worldMax.X / tileWorldSize) - 1;
            int maxY = (int)MathF.Ceiling(worldMax.Y / tileWorldSize) - 1;

            for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                {
                    var image = _source.GetTile(x, y, tileZoom);

                    yield return new TileRenderItem(
                        image,
                        worldPosition: new Vector2(x * tileWorldSize, y * tileWorldSize),
                        worldSize: tileWorldSizeVec,
                        tileX: x,
                        tileY: y,
                        tileZoom: tileZoom);
                }
        }
    }
}
