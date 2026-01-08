using Engine2D.Calc;
using Engine2D.Hud.Stages;
using Engine2D.Input;
using Engine2D.Primitives.Abstractions;
using Engine2D.Primitives.Geometry;
using Engine2D.Primitives.Stages;
using Engine2D.Rendering.Camera;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using Engine2D.Rendering.Stages;
using Engine2D.Tiles.Abstractions;
using Engine2D.Tiles.Caching;
using Engine2D.Tiles.Rendering;
using RenderLab.Targets.WinForms;
using System.Diagnostics;

namespace RenderLab
{
    public partial class Form1 : Form
    {
        private readonly Stopwatch _frameTimer = Stopwatch.StartNew();
        private readonly List<ViewportHost> _viewports = new();
        private readonly float FpsLimit = -1; // Unlimited FPS
        private float _fpsTimeAccum;
        private int _fpsFrameCount;
        private float fps;

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
            var primitives = GenerateSteeredPath(1000);

            // -----------------------
            // Tile system
            // -----------------------
            var tileSpec = new TilePyramidSpec
            {
                TilePixelSize = 256,
                Level0WorldSize = 256f,
                Origin = new WorldVector(0, 0)
            };
            var httpTileSource = new HttpWebpTileProvider(TileUrlTemplate, 16);
            var diskTileCache = new TileDiskCache(httpTileSource, TileCacheFolder);
            var tileScaler = new WinFormsTileImageScaler();
            var scaleStage = new TileProviderScaler(diskTileCache, tileScaler);
            var memoryTileCache = new TileMemoryCache(scaleStage, 256);

            // -----------------------
            // Render pipeline
            // -----------------------
            var pipeline = new RenderPipeline2D();
            pipeline.AddStage(new ClearStage(ColorRgba.Black));
            pipeline.AddStage(new TileRenderStage(memoryTileCache, tileSpec));
            pipeline.AddStage(new PrimitiveRenderStage(() => primitives));
            pipeline.AddStage(new HudRenderStage(DrawHud));

            // -----------------------
            // Viewports
            // -----------------------
            _viewports.Add(CreateViewPortHost(pictureBox1, pipeline));

            // -----------------------
            // Drive frames
            // -----------------------
            Application.Idle += OnIdle;
        }

        private void DrawHud(HudDrawer hud)
        {
            int yPos = 0;
            hud.DrawLabel(new ScreenVector(10, yPos += 25), new ScreenVector(100, 20), $"FPS: {(int)fps}");
            hud.DrawLabel(new ScreenVector(10, yPos += 25), new ScreenVector(100, 20), $"Zoom: {hud.Context.Camera.Zoom}");
        }

        private void OnIdle(object? sender, EventArgs e)
        {
            float elapsed = (float)_frameTimer.Elapsed.TotalSeconds;
            if (elapsed <= 0f)
                return;

            // Accumulate FPS stats
            _fpsTimeAccum += elapsed;
            _fpsFrameCount++;

            // Update FPS every 500 ms
            if (_fpsTimeAccum >= 0.5f)
            {
                fps = _fpsFrameCount / _fpsTimeAccum;
                _fpsTimeAccum = 0f;
                _fpsFrameCount = 0;
            }

            // Optional FPS limiting
            if (FpsLimit > 0)
            {
                float targetFrameTime = 1f / FpsLimit;
                if (elapsed < targetFrameTime)
                {
                    int sleepMs = (int)((targetFrameTime - elapsed) * 1000f);
                    if (sleepMs > 0)
                        Thread.Sleep(sleepMs);
                    return;
                }
            }

            _frameTimer.Restart();
            RenderFrame();
        }



         
        private void RenderFrame()
        {
            foreach (var vp in _viewports)
            {
                vp.CameraInput.HandleInput(vp.Input);
                vp.Input.Clear();
                vp.RenderHost.RequestRedraw();
            }
        }


        ViewportHost CreateViewPortHost(PictureBox pictureBox, RenderPipeline2D pipeLine)
        {
            var camera = new Camera2D();
            var input = new InputQueue();
            var cameraInput = new CameraPanZoomHandler(camera, 1, 32);
            var renderHost = new PictureBoxRenderHost(pictureBox, pipeLine, camera);
            var inputSource = new WinFormsInputSource(pictureBox, input);

            return new ViewportHost
            {
                RenderHost = renderHost,
                Input = input,
                CameraInput = cameraInput
            };
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
        public required PictureBoxRenderHost RenderHost;
        public required InputQueue Input;
        public required CameraPanZoomHandler CameraInput;
    }


}



