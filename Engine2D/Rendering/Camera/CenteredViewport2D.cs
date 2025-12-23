using System.Numerics;

namespace Engine2D.Rendering.Camera
{
    public sealed class CenteredViewport2D : IViewport2D
    {
        public Vector2 ScreenSize { get; }

        public CenteredViewport2D(Vector2 screenSize)
        {
            ScreenSize = screenSize;
        }

        public Vector2 WorldToScreen(Vector2 world, Camera2D camera)
        {
            Vector2 relative = world - camera.Position;
            Vector2 scaled = relative * camera.Zoom;

            return scaled + ScreenSize * 0.5f;
        }

        public Vector2 ScreenToWorld(Vector2 screen, Camera2D camera)
        {
            Vector2 centered = screen - ScreenSize * 0.5f;
            Vector2 unscaled = centered / camera.Zoom;
            return unscaled + camera.Position;
        }
    }
}
