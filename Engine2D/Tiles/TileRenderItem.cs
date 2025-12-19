using System.Numerics;

namespace Engine2D.Tiles
{
    public sealed class TileRenderItem
    {
        public ITileImage Image { get; }
        public Vector2 WorldPosition { get; }
        public Vector2 WorldSize { get; }

        // Debug / identity
        public int TileX { get; }
        public int TileY { get; }
        public int TileZoom { get; }

        public TileRenderItem(ITileImage image, Vector2 worldPosition, Vector2 worldSize, int tileX, int tileY, int tileZoom)
        {
            Image = image;
            WorldPosition = worldPosition;
            WorldSize = worldSize;
            TileX = tileX;
            TileY = tileY;
            TileZoom = tileZoom;
        }
    }


}
