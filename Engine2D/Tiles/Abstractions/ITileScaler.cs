using Engine2D.Tiles.Abstractions;

namespace Engine2D.Tiles.Rendering
{
    public interface ITileScaler
    {
        ITileImage Scale(            ITileImage source,            int targetWidth,            int targetHeight);
    }


}
