using Engine2D.Tiles.Abstractions;
using System.Diagnostics;
using System.Numerics;

namespace Engine2D.Tiles.Caching
{

    public readonly record struct TileCacheKey(        TileId Tile,        int PixelWidth,        int PixelHeight )
    {
        public static TileCacheKey FromQuery(TileQuery q)
            => new(q.Tile, q.PixelWidth, q.PixelHeight);
    }



}
