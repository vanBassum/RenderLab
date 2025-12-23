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
        private PictureBoxViewport _viewport = null!;
        private RenderPipeline2D _pipeline = null!;
        private List<IPrimitive2D> _primitives = null!;

        private InputQueue _input = null!;
        private CameraPanZoomHandler _cameraInput = null!;
        private WinFormsInputSource _inputSource = null!;

        private readonly Stopwatch _frameTimer = Stopwatch.StartNew();
        private const double TargetFrameTimeMs = 1000.0 / 60.0;

        private readonly string TileUrlTemplate = "https://gtamap.xyz/mapStyles/styleSatelite/{z}/{x}/{y}.jpg";
        private readonly string TileCacheFolder = "TileCache/gta5";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // -----------------------
            // Camera (engine)
            // -----------------------
            var camera = new Camera2D
            {
                Position = Vector2.Zero,
                Zoom = 2f
            };

            // -----------------------
            // Input (engine + target)
            // -----------------------
            _input = new InputQueue();
            _cameraInput = new CameraPanZoomHandler(camera);
            _inputSource = new WinFormsInputSource(pictureBox1, _input);

            // -----------------------
            // World data (engine)
            // -----------------------
            _primitives = new List<IPrimitive2D>
            {
                new Line2D(new Vector2(-100, 0), new Vector2(100, 0)),
                new Line2D(new Vector2(0, -100), new Vector2(0, 100)),
                new Line2D(new Vector2(-150, -150), new Vector2(150, 150)),
            };

            // -----------------------
            // Viewport (target)
            // -----------------------
            _viewport = new PictureBoxViewport(pictureBox1, camera);

            // -----------------------
            // Tile system (engine)
            // -----------------------
            //var imageScaler = new WinFormsTileImageScaler();

            //var debug = new DebugTileSource();
            var httpTileSource = new HttpTileSource(TileUrlTemplate);
            var diskCache = new TileSourceDiskCache(httpTileSource, TileCacheFolder);

            
            var rawCache = new TileSourceMemoryCache(diskCache, 100);
            var tileScaler = new WinFormsTileImageScaler();
            var scaler = new ScaledTileSource(rawCache, tileScaler);
            var scaledCache = new ScaledTileSourceMemoryCache(scaler, 100);


            // -----------------------
            // Render pipeline (engine)
            // -----------------------
            _pipeline = new RenderPipeline2D();
            _pipeline.AddStage(new ClearStage(ColorRgba.Black));
            _pipeline.AddStage(new TileRenderStage(scaledCache));

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
            // Handle input (engine)
            _cameraInput.HandleInput(_input);
            _input.Clear();

            // Render frame
            _viewport.BeginFrame(out var context);

            try
            {
                _pipeline.Render(context);
            }
            finally
            {
                _viewport.EndFrame();
            }
        }
    }





}



