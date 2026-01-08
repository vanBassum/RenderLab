using Engine2D.Calc;
using System.Numerics;

namespace Engine2D.Rendering.Camera
{
    public interface IViewport2D
    {
       ScreenVector ScreenSize { get; }
        ScreenVector WorldToScreen(WorldVector world, Camera2D camera);
        WorldVector ScreenToWorld(ScreenVector screen, Camera2D camera);

        public WorldVector GetWorldMin(Camera2D camera)
        {
            var a = ScreenToWorld(ScreenVector.Zero, camera);
            var b = ScreenToWorld(ScreenSize, camera);
            return new WorldVector(
                MathF.Min(a.X, b.X),
                MathF.Min(a.Y, b.Y));
        }

        public WorldVector GetWorldMax(Camera2D camera)
        {
            var a = ScreenToWorld(ScreenVector.Zero, camera);
            var b = ScreenToWorld(ScreenSize, camera);
            return new WorldVector(
                MathF.Max(a.X, b.X),
                MathF.Max(a.Y, b.Y));
        }
    }
}


