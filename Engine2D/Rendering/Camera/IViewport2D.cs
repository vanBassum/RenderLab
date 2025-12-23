using System.Numerics;

namespace Engine2D.Rendering.Camera
{
    public interface IViewport2D
    {
        ScreenVector ScreenSize { get; }
        ScreenVector WorldToScreen(Vector2 world, Camera2D camera);
        Vector2 ScreenToWorld(ScreenVector screen, Camera2D camera);

    }
}
