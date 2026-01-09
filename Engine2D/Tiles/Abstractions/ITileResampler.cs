using Engine2D.Tiles.Models;

namespace Engine2D.Tiles.Abstractions
{
    public interface ITileResampler
    {
        ITileImage Resample(ITileImage source, PixelRect sourceRect, int targetWidth, int targetHeight);
    }
}




