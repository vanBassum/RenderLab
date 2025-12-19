namespace Engine2D.Tiles
{
    public interface ITileImageScaler
    {
        ITileImage Scale(ITileImage source, int targetPixelSize);
    }

}
