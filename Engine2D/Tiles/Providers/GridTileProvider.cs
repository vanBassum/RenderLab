using Engine2D.Rendering.Camera;
using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Rendering;
using Engine2D.Tiles.Scaling;
using System.Numerics;

namespace Engine2D.Tiles.Providers
{
    public sealed class GridTileProvider : ITileProvider
    {
        private readonly ITileSource _source;
        private readonly TileZoomSelector _zoomSelector;
        private readonly Camera2D _camera;
        private readonly float _baseTileWorldSize;

        public GridTileProvider(
            ITileSource source,
            TileZoomSelector zoomSelector,
            Camera2D camera,
            float baseTileWorldSize = 256f)
        {
            _source = source;
            _zoomSelector = zoomSelector;
            _camera = camera;
            _baseTileWorldSize = baseTileWorldSize;
        }

        public IEnumerable<TileRenderItem> GetTiles(Vector2 worldMin, Vector2 worldMax)
        {
            int z = _zoomSelector.SelectTileZoom(_camera.Zoom);

            // IMPORTANT: higher zoom => smaller world area per tile
            float tileWorldSize = _baseTileWorldSize / MathF.Pow(2f, z);
            var tileWorldSizeVec = new Vector2(tileWorldSize, tileWorldSize);

            int minX = (int)MathF.Floor(worldMin.X / tileWorldSize);
            int minY = (int)MathF.Floor(worldMin.Y / tileWorldSize);

            // IMPORTANT: include partially visible edge tiles
            int maxX = (int)MathF.Ceiling(worldMax.X / tileWorldSize) - 1;
            int maxY = (int)MathF.Ceiling(worldMax.Y / tileWorldSize) - 1;

            for (int y = minY; y <= maxY; y++)
                for (int x = minX; x <= maxX; x++)
                {
                    var image = _source.GetTile(x, y, z);

                    if (image == null)
                        continue;

                    yield return new TileRenderItem(
                        image,
                        worldPosition: new Vector2(x * tileWorldSize, y * tileWorldSize),
                        worldSize: tileWorldSizeVec,
                        tileX: x,
                        tileY: y,
                        tileZoom: z);
                }
        }
    }
}
