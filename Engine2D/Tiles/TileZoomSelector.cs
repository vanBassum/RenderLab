namespace Engine2D.Tiles
{
    public sealed class TileZoomSelector
    {
        public int SelectTileZoom(float cameraZoom)
        {
            return (int)MathF.Floor(MathF.Log2(cameraZoom));
        }
    }

}
