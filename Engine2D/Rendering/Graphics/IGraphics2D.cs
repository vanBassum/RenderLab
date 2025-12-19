using System.Numerics;

namespace Engine2D.Rendering.Graphics
{
    // =========================
    // Backend-agnostic graphics
    // =========================

    public interface IGraphics2D
    {
        void Clear(ColorRgba color);

        void DrawLine(Vector2 a, Vector2 b, ColorRgba color, float thickness = 1.0f);

        void DrawText(            Vector2 position,            string text,            ColorRgba color        );
    }
}
