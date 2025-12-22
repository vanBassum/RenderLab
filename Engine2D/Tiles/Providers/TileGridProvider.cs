using Engine2D.Rendering.Camera;
using Engine2D.Tiles.Abstractions;
using System.Numerics;

public sealed class TileGridProvider : ITileGridProvider
{
    private readonly Camera2D _camera;
    private readonly float _baseTileWorldSize;

    public TileGridProvider(Camera2D camera, float baseTileWorldSize = 256f)
    {
        _camera = camera;
        _baseTileWorldSize = baseTileWorldSize;
    }

    public IEnumerable<TileRenderItem> GetTiles(Vector2 worldMin, Vector2 worldMax)
    {
        // Select discrete zoom level from camera zoom
        // Z = 0 → base resolution
        int z = Math.Max(0, (int)MathF.Floor(MathF.Log2(_camera.Zoom)));

        // World size of one tile at this zoom
        float tileWorldSize = _baseTileWorldSize / (1 << z);

        // Native pixel size of tiles at this zoom
        int pixelSize = (int)(_baseTileWorldSize * (1 << z));

        // Compute inclusive tile bounds
        int minX = (int)MathF.Floor(worldMin.X / tileWorldSize);
        int minY = (int)MathF.Floor(worldMin.Y / tileWorldSize);

        int maxX = (int)MathF.Floor((worldMax.X - float.Epsilon) / tileWorldSize);
        int maxY = (int)MathF.Floor((worldMax.Y - float.Epsilon) / tileWorldSize);

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                // World position of the tile origin
                var worldPos = new Vector2(
                    x * tileWorldSize,
                    y * tileWorldSize);

                // Convert to screen space
                var screenPos = _camera.WorldToScreen(worldPos);

                // Screen size is purely visual
                var screenSize = new Vector2(
                    tileWorldSize * _camera.Zoom,
                    tileWorldSize * _camera.Zoom);

                yield return new TileRenderItem
                {
                    TileKey = new TileKey
                    {
                        X = x,
                        Y = y,
                        Z = z,
                        PixelSize = pixelSize
                    },
                    ScreenPosition = screenPos,
                    ScreenSize = screenSize
                };
            }
        }
    }

}


