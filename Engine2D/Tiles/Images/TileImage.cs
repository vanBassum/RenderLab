namespace Engine2D.Tiles.Images
{
    public sealed class TileImage : IDisposable
    {
        public Bitmap Bitmap { get; }

        public TileImage(Bitmap bitmap)
        {
            Bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
        }

        public void Dispose()
        {
            Bitmap.Dispose();
        }
    }

}



