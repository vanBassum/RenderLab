using Engine2D.Rendering.Camera;
using Engine2D.Rendering.Graphics;

namespace Engine2D.Rendering.Pipeline
{
    public readonly struct RenderContext2D
    {
        public Camera2D Camera { get; }
        public IViewport2D Viewport { get; }
        public IGraphics2D Graphics { get; }

        public RenderContext2D(Camera2D camera, IViewport2D viewport, IGraphics2D graphics)
        {
            Camera = camera;
            Viewport = viewport;
            Graphics = graphics;
        }
    }
}
