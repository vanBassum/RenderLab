using Engine2D.Calc;
using System.Numerics;

namespace Engine2D.Rendering.Camera
{
    public sealed class CenteredViewport2D : IViewport2D
    {
        public ScreenVector ScreenSize { get; set; }

        public CenteredViewport2D(ScreenVector screenSize)
        {
            ScreenSize = screenSize;
        }

        public ScreenVector WorldToScreen(Vector2 world, Camera2D camera)
        {
            Vector2 relative = world - camera.Position;
            Vector2 scaled = relative * camera.Zoom;

            float x = scaled.X + ScreenSize.X * 0.5f;
            float y = scaled.Y + ScreenSize.Y * 0.5f;

            return new ScreenVector(
                (int)MathF.Round(x),
                (int)MathF.Round(y));
        }


        public Vector2 ScreenToWorld(ScreenVector screen, Camera2D camera)
        {
            // 1. Convert screen pixel to centered pixel space (still integer)
            ScreenVector halfScreen = ScreenSize / 2;
            ScreenVector centered = screen - halfScreen;

            // 2. Convert to float world units
            Vector2 worldOffset = new Vector2(
                centered.X / camera.Zoom,
                centered.Y / camera.Zoom);

            // 3. Translate by camera position
            return worldOffset + camera.Position;
        }

    }
}
