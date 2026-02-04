using Engine2D.Calc;
using System.Numerics;

namespace Engine2D.Rendering.Camera
{
    public interface IViewport2D
    {
        ScreenVector ScreenSize { get; }
        ScreenVector WorldToScreen(Vector2 world, Camera2D camera);
        Vector2 ScreenToWorld(ScreenVector screen, Camera2D camera);

        float WorldToScreenLength(float worldLength, Camera2D camera) => worldLength * camera.Zoom;
        float ScreenToWorldLength(float screenLength, Camera2D camera) => screenLength / camera.Zoom;

        public Vector2 GetWorldMin(Camera2D camera)
        {
            var a = ScreenToWorld(ScreenVector.Zero, camera);
            var b = ScreenToWorld(ScreenSize, camera);
            return new Vector2(
                MathF.Min(a.X, b.X),
                MathF.Min(a.Y, b.Y));
        }

        public Vector2 GetWorldMax(Camera2D camera)
        {
            var a = ScreenToWorld(ScreenVector.Zero, camera);
            var b = ScreenToWorld(ScreenSize, camera);
            return new Vector2(
                MathF.Max(a.X, b.X),
                MathF.Max(a.Y, b.Y));
        }
    }
}


