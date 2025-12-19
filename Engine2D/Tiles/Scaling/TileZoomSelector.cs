namespace Engine2D.Tiles.Scaling
{
    public sealed class TileZoomSelector
    {
        public int SelectTileZoom(float cameraZoom)
        {
            return (int)MathF.Floor(MathF.Log2(cameraZoom));
        }
    }
}
