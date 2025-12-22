using System.Numerics;
// This can be the conversion between world coordinates and tile grid coordinates
// So it returns a list of tiles that need to be drawn at a screen location
public interface ITileGridProvider
{
    // This thing is supplied with a camera and a viewport size.
    // This is the only place where the power of 2 conversion is done.

    IEnumerable<TileRenderItem> GetTiles(Vector2 worldMin, Vector2 worldMax);
}


