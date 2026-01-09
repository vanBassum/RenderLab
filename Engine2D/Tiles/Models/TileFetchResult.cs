using Engine2D.Tiles.Abstractions;

namespace Engine2D.Tiles.Models
{
    public sealed class TileFetchResult
    {
        public ITileImage? Image { get; init; }

        // False = exact tile (cache it)
        // True  = fallback (do not cache; allow next frame to fetch real tile)
        public bool IsProvisional { get; init; }
    }

}


