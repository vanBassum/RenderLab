using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using System.Diagnostics;
using System.Numerics;

namespace Engine2D.Rendering.Stages
{
    public sealed class FpsCounterStage : IRenderer2D
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private int _frameCount;
        private double _fps;

        private long _lastTimestamp;

        public void Render(in RenderContext2D context)
        {
            Measure();

            context.Graphics.DrawText(
                new Vector2(10, 10),
                $"FPS: {_fps:F1}",
                ColorRgba.Lime);
        }

        private void Measure()
        {
            _frameCount++;

            var now = _stopwatch.ElapsedMilliseconds;
            var elapsed = now - _lastTimestamp;

            if (elapsed >= 1000)
            {
                _fps = _frameCount * 1000.0 / elapsed;
                _frameCount = 0;
                _lastTimestamp = now;
            }
        }
    }
}
