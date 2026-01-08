namespace Engine2D.Tiles.Abstractions
{
    public interface ITileScaler
    {
        ITileImage Scale(ITileImage source, int targetWidth, int targetHeight);
    }
}


