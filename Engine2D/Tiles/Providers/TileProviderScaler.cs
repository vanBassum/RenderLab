using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Models;

namespace Engine2D.Tiles.Providers
{
    public sealed class TileProviderScaler : ITileProvider
    {
        private readonly ITileProvider _inner;
        private readonly ITileScaler _scaler;

        public TileProviderScaler(ITileProvider inner, ITileScaler scaler)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _scaler = scaler ?? throw new ArgumentNullException(nameof(scaler));
        }

        public async ValueTask<TileFetchResult> FetchAsync(TileQuery query, CancellationToken token)
        {
            var result = await _inner.FetchAsync(query, token).ConfigureAwait(false);
            var image = result.Image;

            if (image is null)
                return result;

            // If already correct size, no-op
            if (image.Width == query.PixelWidth && image.Height == query.PixelHeight)
                return result;

            // Scale and dispose old image:
            // NOTE: This is safe only if ownership semantics are "result owns the image".
            // In your current pipeline, caches keep images; scaler should be above caches,
            // so images coming from below are not shared by anyone else.
            var scaled = _scaler.Scale(image, query.PixelWidth, query.PixelHeight);

            image.Dispose();

            return new TileFetchResult { Image = scaled };
        }
    }
}


