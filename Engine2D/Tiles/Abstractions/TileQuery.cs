namespace Engine2D.Tiles.Abstractions
{
    public sealed record TileQuery
    {
        public required TileId Tile { get; init; }
        public required int PixelWidth { get; init; }
        public required int PixelHeight { get; init; }
    }
}


