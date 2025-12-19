namespace Engine2D.Tiles.Abstractions
{
    public interface ITileImageScaler
    {
        ITileImage Scale(ITileImage source, int targetPixelSize);
    }

}
