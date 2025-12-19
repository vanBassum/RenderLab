namespace Engine2D.Tiles.Abstractions
{
    public readonly struct TileId
    {
        public int Zoom { get; }
        public int X { get; }
        public int Y { get; }

        public TileId(int zoom, int x, int y)
        {
            Zoom = zoom;
            X = x;
            Y = y;
        }
    }
}
