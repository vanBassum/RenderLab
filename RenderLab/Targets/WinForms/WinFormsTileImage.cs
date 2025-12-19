using Engine2D.Tiles.Images;

namespace RenderLab.Targets.WinForms
{
    public sealed class WinFormsTileImage : ITileImage, IDisposable
    {
        public Bitmap Bitmap { get; }

        public int Width => Bitmap.Width;
        public int Height => Bitmap.Height;

        public WinFormsTileImage(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }

        public void Dispose()
        {
            Bitmap.Dispose();
        }
    }
}
