using System;
using System.Diagnostics;
using Timer = System.Windows.Forms.Timer;

namespace RenderLab
{
    public sealed class GameLoop : IDisposable
    {
        public int FpsLimit
        {
            get => _fpsLimit;
            set
            {
                _fpsLimit = value;
                ReconfigureTimer();
            }
        }
        private int _fpsLimit = 60; // default

        public int CurrentFps { get; private set; }      // windowed FPS (actual callbacks/sec)
        public float AverageLoad { get; private set; }   // 0..1 work / (work + throttled idle)

        public float StatsWindowSeconds
        {
            get => _statsWindowSeconds;
            set => _statsWindowSeconds = Math.Max(0.1f, value);
        }
        private float _statsWindowSeconds = 1.0f;

        public bool IsRunning => _timer.Enabled;

        private readonly Timer _timer;
        private readonly Stopwatch _clock = Stopwatch.StartNew();
        private long _lastTickTicks;

        private Action<float>? _tick;

        // Window accumulators
        private double _winWallSeconds;
        private double _winWorkSeconds;
        private double _winThrottledIdleSeconds;
        private int _winFrames;

        public GameLoop()
        {
            _timer = new Timer();
            _timer.Tick += TimerTick;
            ReconfigureTimer();
        }

        /// <summary>
        /// Start the loop. Tick will be invoked on the UI thread.
        /// </summary>
        public void Start(Action<float> tick)
        {
            _tick = tick ?? throw new ArgumentNullException(nameof(tick));

            _lastTickTicks = _clock.ElapsedTicks;

            // Reset window
            _winWallSeconds = 0;
            _winWorkSeconds = 0;
            _winThrottledIdleSeconds = 0;
            _winFrames = 0;

            ReconfigureTimer();
            _timer.Start();
        }

        public void Stop() => _timer.Stop();

        private void ReconfigureTimer()
        {
            if (_fpsLimit <= 0)
            {
                // "Unlimited" in WinForms: drive as fast as Timer can reasonably fire.
                // 1ms is the minimum; actual resolution depends on the system.
                _timer.Interval = 1;
            }
            else
            {
                // Interval in ms; WinForms timer granularity is limited, but good enough for a basic loop.
                int interval = (int)Math.Round(1000.0 / _fpsLimit);
                _timer.Interval = Math.Max(1, interval);
            }
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            if (_tick is null) return;

            long nowTicks = _clock.ElapsedTicks;
            double dt = (nowTicks - _lastTickTicks) / (double)Stopwatch.Frequency;
            if (dt < 0) dt = 0;
            _lastTickTicks = nowTicks;

            // Measure work
            long workStart = _clock.ElapsedTicks;
            _tick((float)dt);
            long workEnd = _clock.ElapsedTicks;

            double workSeconds = (workEnd - workStart) / (double)Stopwatch.Frequency;
            if (workSeconds < 0) workSeconds = 0;

            // Window: wall time and work
            _winWallSeconds += dt;
            _winWorkSeconds += workSeconds;
            _winFrames++;

            // Throttled idle: only meaningful when we have an FPS cap.
            // Estimate budget per frame and count "unused budget" as throttled idle.
            if (_fpsLimit > 0)
            {
                double budget = 1.0 / _fpsLimit;
                double idleThisFrame = budget - workSeconds;
                if (idleThisFrame > 0)
                    _winThrottledIdleSeconds += idleThisFrame;
            }

            if (_winWallSeconds >= _statsWindowSeconds && _winWallSeconds > 0)
            {
                CurrentFps = (int)Math.Round(_winFrames / _winWallSeconds);

                double denom;
                if (_fpsLimit > 0)
                {
                    // Load = work / (work + intentional throttled idle)
                    denom = _winWorkSeconds + _winThrottledIdleSeconds;
                    AverageLoad = denom > 0 ? (float)(_winWorkSeconds / denom) : 0f;
                }
                else
                {
                    // Uncapped: by definition you are not throttling, so load == 1
                    AverageLoad = 1f;
                }

                // Reset window
                _winWallSeconds = 0;
                _winWorkSeconds = 0;
                _winThrottledIdleSeconds = 0;
                _winFrames = 0;
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Tick -= TimerTick;
            _timer.Dispose();
        }
    }
}
