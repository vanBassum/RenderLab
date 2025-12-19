using Engine2D.Input;
using Engine2D.Primitives;
using Engine2D.Rendering.Camera;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using Engine2D.Rendering.Stages;
using Engine2D.Tiles;
using RenderLab.Targets.WinForms;
using System.Diagnostics;
using System.Drawing.Imaging;
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
                Zoom = 1f
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


            var bmp = new Bitmap(256, 256, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.DarkSlateGray);
                g.FillRectangle(Brushes.DimGray, 0, 0, 128, 128);
                g.FillRectangle(Brushes.Gray, 128, 128, 128, 128);
            }

            var tileImage = new WinFormsTileImage(bmp);

            var tileZoomSelector = new TileZoomSelector();
            int tileZoom = tileZoomSelector.SelectTileZoom(camera.Zoom);

            // -----------------------
            // Render pipeline (engine)
            // -----------------------
            _pipeline = new RenderPipeline2D();
            _pipeline.AddStage(new ClearStage(ColorRgba.Black));
            _pipeline.AddStage(
                new TileRenderStage(
                    new DebugGridTileProvider(tileZoom)));

            _pipeline.AddStage(new PrimitiveRenderStage(() => _primitives));
            _pipeline.AddStage(new FpsCounterStage());

            // -----------------------
            // Drive frames
            // -----------------------
            Application.Idle += OnIdle;
        }

        private void OnIdle(object? sender, EventArgs e)
        {
            // Let it run, so i can count fps
            //var elapsedMs = _frameTimer.Elapsed.TotalMilliseconds;
            //
            //if (elapsedMs < TargetFrameTimeMs)
            //    return;
            //
            //_frameTimer.Restart();
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



