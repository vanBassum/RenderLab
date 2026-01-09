using System.Diagnostics;
using System.Threading;

namespace RenderLab
{
    public record GameLoopContext
    {
        public double DeltaTime { get; init; }
        public double TotalTime { get; init; }
    }

    public sealed class GameLoop
    {
        public Func<GameLoopContext, Task>? UpdateAsyncCallback { get; set; }
        public Func<GameLoopContext, Task>? RenderAsyncCallback { get; set; }

        public int FrameRateLimit { get; set; } = 30;
        public int AverageFrameRate { get; private set; }

        public async Task Start(CancellationToken token = default)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            TimeSpan startTime = stopwatch.Elapsed;
            TimeSpan previousFrameStartTime = startTime;
            TimeSpan nextFrameTargetTime = startTime;

            TimeSpan previousStatisticsUpdateTime = startTime;
            int frameCountSinceLastStatisticsUpdate = 0;

            TimeSpan frameInterval = FrameRateLimit > 0
                ? TimeSpan.FromSeconds(1.0 / FrameRateLimit)
                : TimeSpan.Zero;

            while (!token.IsCancellationRequested)
            {
                TimeSpan frameStartTime = stopwatch.Elapsed;

                double deltaTimeSeconds =
                    (frameStartTime - previousFrameStartTime).TotalSeconds;

                double totalTimeSeconds =
                    (frameStartTime - startTime).TotalSeconds;

                GameLoopContext context = new GameLoopContext
                {
                    DeltaTime = deltaTimeSeconds,
                    TotalTime = totalTimeSeconds
                };

                await (UpdateAsyncCallback?.Invoke(context) ?? Task.CompletedTask);
                await (RenderAsyncCallback?.Invoke(context) ?? Task.CompletedTask);

                frameCountSinceLastStatisticsUpdate++;

                TimeSpan timeSinceLastStatisticsUpdate =
                    stopwatch.Elapsed - previousStatisticsUpdateTime;

                if (timeSinceLastStatisticsUpdate >= TimeSpan.FromMilliseconds(500))
                {
                    AverageFrameRate = (int)Math.Round(
                        frameCountSinceLastStatisticsUpdate /
                        timeSinceLastStatisticsUpdate.TotalSeconds);

                    frameCountSinceLastStatisticsUpdate = 0;
                    previousStatisticsUpdateTime = stopwatch.Elapsed;
                }

                // Frame pacing (stable, low drift)
                if (FrameRateLimit > 0)
                {
                    nextFrameTargetTime += frameInterval;

                    TimeSpan now = stopwatch.Elapsed;

                    // Prevent runaway catch-up if we fall far behind
                    if (now > nextFrameTargetTime + frameInterval)
                    {
                        nextFrameTargetTime = now;
                    }

                    TimeSpan remainingTime = nextFrameTargetTime - now;

                    // Coarse delay
                    if (remainingTime > TimeSpan.Zero)
                    {
                        await Task.Delay(remainingTime, token);
                    }
                }

                previousFrameStartTime = frameStartTime;
            }
        }
    }
}
