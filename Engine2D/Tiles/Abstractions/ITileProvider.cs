namespace Engine2D.Tiles.Abstractions
{
    public interface ITileProvider
    {
        ValueTask<TileFetchResult> FetchAsync(TileQuery query, CancellationToken token);
    }

}


