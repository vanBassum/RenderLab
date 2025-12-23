using Engine2D.Input;
using Engine2D.Primitives;
using Engine2D.Rendering.Camera;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using Engine2D.Rendering.Stages;
using Engine2D.Tiles.Caching;
using Engine2D.Tiles.Rendering;
using RenderLab.Targets.WinForms;
using System.Diagnostics;
using System.Numerics;

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
            _primitives = new List<IPrimitive2D>
            {
                new Line2D(new Vector2(-100, 0), new Vector2(100, 0)),
                new Line2D(new Vector2(0, -100), new Vector2(0, 100)),
                new Line2D(new Vector2(-150, -150), new Vector2(150, 150)),
            };

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
            if (true)
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
    }

    class ViewportHost
    {
        public required PictureBoxViewport Viewport;
        public required InputQueue Input;
        public required CameraPanZoomHandler CameraInput;
    }



}



