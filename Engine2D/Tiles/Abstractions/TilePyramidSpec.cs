using Engine2D.Calc;

namespace Engine2D.Tiles.Abstractions
{
    public sealed record TilePyramidSpec
    {
        public required int TilePixelSize { get; init; }      = 256;    // tile size in pixels (assumed square)
        public required float Level0WorldSize { get; init; }  = 256f;   // world units covered by 1 tile at level 0
        public WorldVector Origin { get; init; } = WorldVector.Zero;    // optional, default (0,0)
    }
}


