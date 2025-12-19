using Engine2D.Tiles;

namespace RenderLab
{
    public sealed class WinFormsTileImage : ITileImage
    {
        public Bitmap Bitmap { get; }

        public WinFormsTileImage(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }





}



