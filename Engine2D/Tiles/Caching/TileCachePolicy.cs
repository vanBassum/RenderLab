using Engine2D.Tiles.Images;
using Microsoft.Extensions.Caching.Memory;

namespace Engine2D.Tiles.Caching
{
    public static class TileCachePolicy
    {
        public static MemoryCacheEntryOptions Create(TileImage tile, Action? onEvicted = null)
        {
            return new MemoryCacheEntryOptions
            {
                Size = EstimateSize(tile),
                PostEvictionCallbacks =
                {
                    new PostEvictionCallbackRegistration
                    {
                        EvictionCallback = (_, value, _, _) =>
                        {
                            if (value is TileImage t)
                                t.Dispose();

                            onEvicted?.Invoke();
                        }
                    }
                }
            };
        }

        private static long EstimateSize(TileImage tile)
            => (long)tile.Bitmap.Width * tile.Bitmap.Height * 4;
    }


}



