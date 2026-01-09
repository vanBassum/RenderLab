using Engine2D.Tiles.Models;

namespace Engine2D.Tiles.Abstractions
{
    public interface ITileProvider
    {
        ValueTask<TileFetchResult> FetchAsync(TileQuery query, CancellationToken token);
    }

}


