using System.Numerics;

namespace Engine2D.Rendering.Camera
{
    public sealed class Camera2D
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Zoom { get; set; } = 1.0f;
    }
}
