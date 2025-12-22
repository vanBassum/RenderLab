using Engine2D.Tiles.Abstractions;
using System.Numerics;

public class TileRenderItem
{
    public required TileKey TileKey { get; set; }
    public Vector2 ScreenPosition { get; set; }
    public Vector2 ScreenSize { get; set; }
}


