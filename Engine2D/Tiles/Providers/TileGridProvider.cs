using Engine2D.Calc;
using Engine2D.Tiles.Abstractions;
using System.Numerics;

namespace Engine2D.Tiles.Providers
{
    public sealed class TileGridProvider : ITileGridProvider
    {
        private readonly float _baseTileWorldSize;
        private readonly int _tilePixelSize;

        public TileGridProvider(float baseTileWorldSize = 256f, int tilePixelSize = 256)
        {
            _baseTileWorldSize = baseTileWorldSize;
            _tilePixelSize = tilePixelSize;
        }

        public IEnumerable<TileRenderItem> GetTiles(WorldVector worldMin, WorldVector worldMax, float zoom)
        {
            // Discrete LOD selection (power-of-two zoom assumed)
            int z = Math.Max(0, (int)MathF.Ceiling(MathF.Log2(zoom)));
            //int z = Math.Max(0, (int)MathF.Floor(MathF.Log2(zoom)));
            float tileWorldSize = _baseTileWorldSize / (1 << z);

            int minX = (int)MathF.Floor(worldMin.X / tileWorldSize);
            int minY = (int)MathF.Floor(worldMin.Y / tileWorldSize);
            int maxX = (int)MathF.Floor(worldMax.X / tileWorldSize);
            int maxY = (int)MathF.Floor(worldMax.Y / tileWorldSize);

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    yield return new TileRenderItem
                    {
                        TileKey = new TileKey
                        {
                            X = x,
                            Y = y,
                            Z = z,
                            PixelSize = _tilePixelSize
                        },
                        WorldPosition = new WorldVector(x * tileWorldSize, y * tileWorldSize),
                        WorldSize = new WorldVector(tileWorldSize, tileWorldSize)
                    };
                }
            }
        }
    }
}
