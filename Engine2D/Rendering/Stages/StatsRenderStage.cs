using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace Engine2D.Rendering.Stages
{
    public sealed class StatsRenderStage : IRenderer2D
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private int _frameCount;
        private double _fps;
        private long _lastTimestamp;

        private Vector2 _cursor = new(6, 6);
        private readonly Vector2 _boxSize = new(140, 20);

        public void Render(in RenderContext2D context)
        {
            Measure();

            _cursor = new Vector2(6, 6);
            DrawStatistic(context, $"FPS:  {_fps:F1}");
            DrawStatistic(context, $"Zoom: {context.Camera.Zoom:F2}");
        }

        private void DrawStatistic(in RenderContext2D context, string text)
        {
            context.Graphics.FillRect(
                _cursor,
                _boxSize,
                new ColorRgba(0, 0, 0, 160));

            context.Graphics.DrawText(
                _cursor + new Vector2(6, 4),
                text,
                ColorRgba.Lime);

            _cursor.Y += _boxSize.Y + 4;
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
