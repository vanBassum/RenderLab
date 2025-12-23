using Engine2D.Rendering.Camera;
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
        void DrawLine(ScreenVector a, ScreenVector b, ColorRgba color, float thickness = 1.0f);
        void DrawText(ScreenVector position, string text, ColorRgba color);
        void DrawImage(ITileImage image, ScreenVector topLeft);
        void DrawImage(ITileImage image, ScreenVector topLeft, ScreenVector size);
        void FillRect(ScreenVector position, ScreenVector size, ColorRgba color);
        void DrawRect(ScreenVector screenPos, ScreenVector screenSize, ColorRgba red);
    }
}
