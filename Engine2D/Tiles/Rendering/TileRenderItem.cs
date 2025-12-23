using Engine2D.Calc;
using Engine2D.Tiles.Abstractions;
using System.Numerics;

public class TileRenderItem
{
    public required TileKey TileKey { get; set; }
    public WorldVector WorldPosition { get; set; }
    public WorldVector WorldSize { get; set; }
}


