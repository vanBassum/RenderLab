using System.Numerics;

namespace Engine2D.Rendering.Camera
{
    // =========================
    // Camera
    // =========================

    public sealed class Camera2D
    {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Zoom { get; set; } = 1.0f;

        public Vector2 ViewportSize { get; set; }

        public Vector2 WorldToScreen(Vector2 world)
        {
            Vector2 relative = world - Position;
            Vector2 scaled = relative * Zoom;

            return scaled + new Vector2(
                ViewportSize.X * 0.5f,
                ViewportSize.Y * 0.5f
            );
        }
    }
}
