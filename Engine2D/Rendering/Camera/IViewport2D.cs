using System.Numerics;

namespace Engine2D.Rendering.Camera
{
    public interface IViewport2D
    {
        Vector2 ScreenSize { get; }

        Vector2 WorldToScreen(Vector2 world, Camera2D camera);
        Vector2 ScreenToWorld(Vector2 screen, Camera2D camera);
    }
}
