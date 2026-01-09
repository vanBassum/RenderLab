using Engine2D.Calc;
using System.Numerics;

namespace Engine2D.Tiles.Models
{
    public sealed record TilePyramidSpec
    {
        public required int TilePixelSize { get; init; }      = 256;    // tile size in pixels (assumed square)
        public required float Level0WorldSize { get; init; }  = 256f;   // world units covered by 1 tile at level 0
        public Vector2 Origin { get; init; } = Vector2.Zero;    // optional, default (0,0)
    }
}


