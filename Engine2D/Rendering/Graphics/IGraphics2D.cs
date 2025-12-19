using Engine2D.Tiles.Abstractions;
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
        void DrawText(Vector2 position, string text, ColorRgba color);
        void DrawImage(ITileImage image, Vector2 topLeft);
        void FillRect(Vector2 position, Vector2 size, ColorRgba color);
        void DrawRect(Vector2 screenPos, Vector2 screenSize, ColorRgba red);
    }
}
