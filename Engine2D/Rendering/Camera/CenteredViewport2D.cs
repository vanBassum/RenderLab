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

        public ScreenVector WorldToScreen(WorldVector world, Camera2D camera)
        {
            WorldVector relative = world - camera.Position;
            WorldVector scaled = relative * camera.Zoom;

            float x = scaled.X + ScreenSize.X * 0.5f;
            float y = scaled.Y + ScreenSize.Y * 0.5f;

            return new ScreenVector(
                (int)MathF.Round(x),
                (int)MathF.Round(y));
        }


        public WorldVector ScreenToWorld(ScreenVector screen, Camera2D camera)
        {
            // 1. Convert screen pixel to centered pixel space (still integer)
            ScreenVector halfScreen = ScreenSize / 2;
            ScreenVector centered = screen - halfScreen;

            // 2. Convert to float world units
            WorldVector worldOffset = new WorldVector(
                centered.X / camera.Zoom,
                centered.Y / camera.Zoom);

            // 3. Translate by camera position
            return worldOffset + camera.Position;
        }

    }
}
