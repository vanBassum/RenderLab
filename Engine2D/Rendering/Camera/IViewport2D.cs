using Engine2D.Calc;
using System.Numerics;

namespace Engine2D.Rendering.Camera
{
    public interface IViewport2D
    {
        ScreenVector ScreenSize { get; }
        ScreenVector WorldToScreen(WorldVector world, Camera2D camera);
        WorldVector ScreenToWorld(ScreenVector screen, Camera2D camera);

        public void GetWorldCoverage(Camera2D camera, out WorldVector worldMin, out WorldVector worldMax)
        {
            var a = ScreenToWorld(ScreenVector.Zero, camera);
            var b = ScreenToWorld(ScreenSize, camera);

            worldMin = new WorldVector(
                MathF.Min(a.X, b.X),
                MathF.Min(a.Y, b.Y));

            worldMax = new WorldVector(
                MathF.Max(a.X, b.X),
                MathF.Max(a.Y, b.Y));
        }
    }
}
