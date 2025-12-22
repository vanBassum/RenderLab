using Engine2D.Tiles.Abstractions;

namespace RenderLab.Targets.WinForms
{
    public sealed class WinFormsTileImage : ITileImage
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



