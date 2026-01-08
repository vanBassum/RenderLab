namespace Engine2D.Tiles.Abstractions
{
    public sealed class TileProviderLodFallback : ITileProvider
    {
        private readonly ITileProvider _inner;
        private readonly ITileResampler _resampler;
        private readonly TilePyramidSpec _spec;

        public TileProviderLodFallback(ITileProvider inner, ITileResampler resampler, TilePyramidSpec spec)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _resampler = resampler ?? throw new ArgumentNullException(nameof(resampler));
            _spec = spec ?? throw new ArgumentNullException(nameof(spec));
        }

        public async ValueTask<TileFetchResult> FetchAsync(TileQuery query, CancellationToken token)
        {
            // 1) Try exact tile first
            var exact = await _inner.FetchAsync(query, token).ConfigureAwait(false);
            if (exact.Image is not null)
            {
                return new TileFetchResult
                {
                    Image = exact.Image,
                    IsProvisional = false
                };
            }

            // 2) Otherwise walk down levels (parent tiles) until we find something
            var req = query.Tile;

            for (int parentLevel = req.TileLevel - 1; parentLevel >= 0; parentLevel--)
            {
                int delta = req.TileLevel - parentLevel;
                int factor = 1 << delta; // number of child tiles per parent tile axis

                int parentX = req.X >> delta;
                int parentY = req.Y >> delta;

                var parentQuery = new TileQuery
                {
                    Tile = new TileId(parentX, parentY, parentLevel),
                    PixelWidth = _spec.TilePixelSize,
                    PixelHeight = _spec.TilePixelSize,
                };

                var parent = await _inner.FetchAsync(parentQuery, token).ConfigureAwait(false);
                if (parent.Image is null)
                    continue; // still downloading/missing, try even lower

                // Which child within the parent?
                int ix = req.X & (factor - 1);
                int iy = req.Y & (factor - 1);

                // Crop rect in parent image pixels
                int srcW = parent.Image.Width / factor;
                int srcH = parent.Image.Height / factor;

                var srcRect = new PixelRect(ix * srcW, iy * srcH, srcW, srcH);

                // Resample to requested output size
                var outImg = _resampler.Resample(parent.Image, srcRect, query.PixelWidth, query.PixelHeight);

                // We own parent.Image; dispose to avoid leaks
                parent.Image.Dispose();

                return new TileFetchResult
                {
                    Image = outImg,
                    IsProvisional = true
                };
            }

            // Nothing found anywhere
            return new TileFetchResult { Image = null, IsProvisional = true };
        }
    }
}


