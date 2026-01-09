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
        private readonly GameLoop _gameLoop = new();

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
            var primitives = new List<IPrimitive2D> ();

            // -----------------------
            // Tile system
            // -----------------------
            var tileSpec = new TilePyramidSpec
            {
                TilePixelSize = 256,
                Level0WorldSize = 256f,
                Origin = new WorldVector(0, 0)
            };
            var tileScaler = new WinFormsTileImageScaler();
            var tileResampler = new WinFormsTileResampler(tileScaler);


            var httpTileSource = new HttpWebpTileProvider(TileUrlTemplate, 16);
            var diskTileCache = new TileDiskCache(httpTileSource, TileCacheFolder);
            var lodFallback = new TileProviderLodFallback(diskTileCache, tileResampler, tileSpec);
            var scaleStage = new TileProviderScaler(lodFallback, tileScaler);
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
            var mainViewport = CreateViewPortHost(pictureBox1, pipeline);
            _viewports.Add(mainViewport);


            // -----------------------
            // Drive frames
            // -----------------------
            _gameLoop.RenderAsyncCallback = Render;
            _ = _gameLoop.Start();
        }

        private async Task Render(GameLoopContext context)
        {
            foreach (var vp in _viewports)
            {
                vp.CameraInput.HandleInput(vp.Input);
                vp.Input.Clear();
                vp.RenderHost.RequestRedraw();
            }
        }

        private void DrawHud(HudDrawer hud)
        {
            int yPos = 0;
            hud.DrawLabel(new ScreenVector(10, yPos += 25), new ScreenVector(100, 20), $"FPS:    {_gameLoop.AverageFrameRate}");
            hud.DrawLabel(new ScreenVector(10, yPos += 25), new ScreenVector(100, 20), $"Zoom:   {hud.Context.Camera.Zoom}");
        }
         


        ViewportHost CreateViewPortHost(PictureBox pictureBox, RenderPipeline2D pipeLine)
        {
            var camera = new Camera2D();
            var input = new InputQueue();
            var renderHost = new PictureBoxRenderHost(pictureBox, pipeLine, camera);
            var inputSource = new WinFormsInputSource(pictureBox, input);
            var cameraInput = new CameraPanZoomHandler(camera, renderHost.Viewport, 1, 128);

            return new ViewportHost
            {
                RenderHost = renderHost,
                Input = input,
                CameraInput = cameraInput
            };
        }
    }

    class ViewportHost
    {
        public required PictureBoxRenderHost RenderHost;
        public required InputQueue Input;
        public required CameraPanZoomHandler CameraInput;
    }
}







