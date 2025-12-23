using Engine2D.Calc;
using Engine2D.Input;
using Engine2D.Primitives.Abstractions;
using Engine2D.Primitives.Geometry;
using Engine2D.Primitives.Stages;
using Engine2D.Rendering.Camera;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using Engine2D.Rendering.Stages;
using Engine2D.Tiles.Caching;
using Engine2D.Tiles.Rendering;
using RenderLab.Targets.WinForms;
using System.Diagnostics;

namespace RenderLab
{
    public partial class Form1 : Form
    {
        private RenderPipeline2D _pipeline = null!;
        private List<IPrimitive2D> _primitives = null!;

        private readonly Stopwatch _frameTimer = Stopwatch.StartNew();
        private const double TargetFrameTimeMs = 1000.0 / 60.0;
        private readonly List<ViewportHost> _viewports = new();

        //private readonly string TileUrlTemplate = "https://gtamap.xyz/mapStyles/styleSatelite/{z}/{x}/{y}.jpg";
        //private readonly string TileCacheFolder = "TileCache/gta5";

        private readonly string TileUrlTemplate = "https://storage-cdn.wemod.com/maps/b1c977d8-59dc-4ff7-b0f0-63488f7bfcd6/tiles/{z}/{y}/{x}.webp";
        private readonly string TileCacheFolder = "TileCache/fallout4";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // -----------------------
            // World data
            // -----------------------
            _primitives = GenerateSteeredPath(1000);

            // -----------------------
            // Viewports
            // -----------------------
            _viewports.Add(CreateViewPortHost(pictureBox1));

            // -----------------------
            // Tile system
            // -----------------------
            var httpTileSource = new HttpWebpTileSource(TileUrlTemplate);
            var diskCache = new WinFormsTileSourceDiskCache(httpTileSource, TileCacheFolder);
            //var memoryCache = new TileSourceMemoryCache(diskCache, 100);
            var tileScaler = new WinFormsTileImageScaler();
            var scaler = new ScaledTileSource(diskCache, tileScaler);
            var scaledMemoryCache = new ScaledTileSourceMemoryCache(scaler, 200);

            // -----------------------
            // Render pipeline
            // -----------------------
            _pipeline = new RenderPipeline2D();
            _pipeline.AddStage(new ClearStage(ColorRgba.Black));
            _pipeline.AddStage(new TileRenderStage(scaledMemoryCache));
            _pipeline.AddStage(new PrimitiveRenderStage(() => _primitives));
            _pipeline.AddStage(new StatsRenderStage());

            // -----------------------
            // Drive frames
            // -----------------------
            Application.Idle += OnIdle;
        }

        private void OnIdle(object? sender, EventArgs e)
        {
            // Let it run, so i can count fps
            if (false)
            {
                var elapsedMs = _frameTimer.Elapsed.TotalMilliseconds;

                if (elapsedMs < TargetFrameTimeMs)
                    return;

                _frameTimer.Restart();
            }

            RenderFrame();
        }

        private void RenderFrame()
        {
            foreach (var vp in _viewports)
            {
                // Handle input per viewport
                vp.CameraInput.HandleInput(vp.Input);
                vp.Input.Clear();

                // Render per viewport
                vp.Viewport.BeginFrame(out var context);
                try
                {
                    _pipeline.Render(context);
                }
                finally
                {
                    vp.Viewport.EndFrame();
                }
            }
        }

        ViewportHost CreateViewPortHost(PictureBox pictureBox)
        {
            var camera = new Camera2D();
            var input = new InputQueue();
            var cameraInput = new CameraPanZoomHandler(camera);
            var viewport = new PictureBoxViewport(pictureBox, camera);
            var inputSource = new WinFormsInputSource(pictureBox, input);

            return new ViewportHost
            {
                Viewport = viewport,
                Input = input,
                CameraInput = cameraInput
            };
        }

        private static List<IPrimitive2D> GenerateRandomLines(    int count,    float worldSize = 256f,    int? seed = 12345)
        {
            var rnd = seed.HasValue ? new Random(seed.Value) : new Random();
            var list = new List<IPrimitive2D>(count);

            float Half = worldSize * 0.5f;

            for (int i = 0; i < count; i++)
            {
                var a = new WorldVector(
                    (float)(rnd.NextDouble() * worldSize),
                    (float)(rnd.NextDouble() * worldSize));

                var b = new WorldVector(
                    (float)(rnd.NextDouble() * worldSize),
                    (float)(rnd.NextDouble() * worldSize));


                list.Add(new Line2D(a, b));
            }

            return list;
        }


        private static List<IPrimitive2D> GenerateSteeredPath(
    int segmentCount,
    float worldSize = 256f,
    float stepLength = 0.25f,     // small steps
    float turnStrength = 0.15f,   // radians per step
    int? seed = 12345)
        {
            var rnd = seed.HasValue ? new Random(seed.Value) : new Random();
            var list = new List<IPrimitive2D>(segmentCount);

            float half = worldSize * 0.5f;

            // Start in the center of the world
            var current = new WorldVector(half, half);

            // Initial heading
            float angle = (float)(rnd.NextDouble() * MathF.Tau);

            for (int i = 0; i < segmentCount; i++)
            {
                // Random steering input [-1, +1]
                float pull = (float)(rnd.NextDouble() * 2.0 - 1.0);

                // Integrate angular velocity
                angle += pull * turnStrength;

                var direction = new WorldVector(
                    MathF.Cos(angle),
                    MathF.Sin(angle));

                var next = current + direction * stepLength;

                // Soft world bounds (Fallout-style invisible walls)
                if (MathF.Abs(next.X) > half || MathF.Abs(next.Y) > half)
                {
                    // Turn back toward center instead of hard reflection
                    var toCenter = WorldVector.Normalize(-current);
                    angle = MathF.Atan2(toCenter.Y, toCenter.X);
                    next = current + new WorldVector(
                        MathF.Cos(angle),
                        MathF.Sin(angle)) * stepLength;
                }

                list.Add(new Line2D(current, next));
                current = next;
            }

            return list;
        }




    }

    class ViewportHost
    {
        public required PictureBoxViewport Viewport;
        public required InputQueue Input;
        public required CameraPanZoomHandler CameraInput;
    }



}



