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
using Engine2D.Tiles.Models;
using Engine2D.Tiles.Providers;
using Engine2D.Tiles.Rendering;
using RenderLab.Targets.WinForms;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Forms;

namespace RenderLab
{
    public partial class Form1 : Form
    {
        private readonly Stopwatch _frameTimer = Stopwatch.StartNew();
        private readonly GameLoop _gameLoop = new();

        //private readonly string TileUrlTemplate = "https://gtamap.xyz/mapStyles/styleSatelite/{z}/{x}/{y}.jpg";
        //private readonly string TileCacheFolder = "TileCache/gta5";

        private readonly string TileUrlTemplate = "https://storage-cdn.wemod.com/maps/b1c977d8-59dc-4ff7-b0f0-63488f7bfcd6/tiles/{z}/{y}/{x}.webp";
        private readonly string TileCacheFolder = "Data/fallout4/Tiles";
        private readonly string RegressionDataFile = "Data/fallout4/regression_data.json";
        private readonly string WaypointsDataFile = "Data/fallout4/waypoints_data.json";


        private readonly MemoryManager _memoryManager = new();

        private readonly PlayerPrimitive _playerPrimitive = new(new Vector2(0, 0));
        private readonly LinearRegression _linearRegression = new();

        private ViewportHost? _mainViewport;

        Vector2 gamePosition = Vector2.Zero;

        private List<Vector2> _playerWaypoints = new();
        private WaypointLog? _waypointLog;



        public Form1()
        {
            InitializeComponent();
        }

        IEnumerable<IPrimitive2D> GetAllPrimitives()
        {
            yield return _playerPrimitive;

            foreach (var anchor in CreateAnchorPrimitives())
                yield return anchor;

            foreach (var waypointPrimitive in CreateWaypointPrimitives())
                yield return waypointPrimitive;
        }

        IEnumerable<ViewportHost> GetAllViewPorts()
        {
            if (_mainViewport != null)
                yield return _mainViewport;
        }



        IEnumerable<IPrimitive2D> CreateAnchorPrimitives()
        {
            foreach (var anchor in _linearRegression.GetPoints())
            {
                var calculatedMapPos = _linearRegression.HasModel
                ? _linearRegression.GameToMap(anchor.Game)
                : anchor.Map;

                yield return new AnchorPrimitive(anchor.Map, calculatedMapPos);
            }
        }

        IEnumerable<IPrimitive2D> CreateWaypointPrimitives()
        {
            if (_playerWaypoints == null || _playerWaypoints.Count == 0)
                yield break;

            int index = 0;
            var prev = _playerWaypoints[0];
            var color = ColorGenerator.FromIndex(index);

            foreach (var waypoint in _playerWaypoints)
            {
                var distance = Vector2.Distance(prev, waypoint);
                if (distance > 500f)
                {
                    index++;
                    color = ColorGenerator.FromIndex(index);
                }

                var mapPos = _linearRegression.GameToMap(waypoint);
                yield return new WaypointDot(mapPos, color);

                prev = waypoint;
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            // -----------------------
            // Tile system
            // -----------------------
            var tileSpec = new TilePyramidSpec
            {
                TilePixelSize = 256,
                Level0WorldSize = 256f,
                Origin = new Vector2(0, 0)
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
            pipeline.AddStage(new PrimitiveRenderStage(GetAllPrimitives));
            pipeline.AddStage(new HudRenderStage(DrawHud));

            // -----------------------
            // Viewports
            // -----------------------
            _mainViewport = CreateViewPortHost(pictureBox1, pipeline);

            // -----------------------
            // Drive frames
            // -----------------------
            _gameLoop.RenderAsyncCallback = Render;
            _gameLoop.UpdateAsyncCallback = Update;
            _ = _gameLoop.Start();


            LoadRegressionData();

            _playerWaypoints = WaypointLog.LoadAll(WaypointsDataFile);
            _waypointLog = new WaypointLog(WaypointsDataFile);
        }

        private static readonly JsonSerializerOptions RegressionJsonOptions =
            new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true, // only needed if Anchor uses fields instead of properties
                Converters = { new Vector2JsonConverter() }
            };


        void LoadRegressionData()
        {
            if (!File.Exists(RegressionDataFile))
                return;

            var json = File.ReadAllText(RegressionDataFile);
            var anchors = JsonSerializer.Deserialize<List<Anchor>>(json, RegressionJsonOptions);
            if (anchors == null)
                return;

            _linearRegression.ClearPoints();
            foreach (var anchor in anchors)
            {
                _linearRegression.AddPoint(anchor);
            }
        }

        void SaveRegressionData()
        {
            var anchors = _linearRegression.GetPoints().ToList();
            var json = JsonSerializer.Serialize(anchors, RegressionJsonOptions);
            var directory = Path.GetDirectoryName(RegressionDataFile);
            if (string.IsNullOrEmpty(directory))
                return;
            Directory.CreateDirectory(directory);
            File.WriteAllText(RegressionDataFile, json);
        }


        private async Task Update(GameLoopContext context)
        {
            if (!_memoryManager.IsAttached)
            {
                if (!_memoryManager.Attach("Fallout4"))
                    return;
            }

            IntPtr moduleBase = _memoryManager.GetProcessBase();

            if (moduleBase == IntPtr.Zero)
                return;

            float xGame = _memoryManager.ReadFloat(moduleBase + 0x32DBF4C);
            float yGame = _memoryManager.ReadFloat(moduleBase + 0x32DBF50);

            gamePosition = new System.Numerics.Vector2(xGame, yGame);

            if (_playerWaypoints.Count == 0)
            {
                _playerWaypoints.Add(gamePosition);
                _waypointLog?.Append(gamePosition);
            }
            else
            {
                var distance = Vector2.Distance(gamePosition, _playerWaypoints.Last());
                if (distance >= 100f)
                {
                    _playerWaypoints.Add(gamePosition);
                    _waypointLog?.Append(gamePosition);
                }
            }


            if (_linearRegression.HasModel)
            {
                var mapPos = _linearRegression.GameToMap(gamePosition);
                _playerPrimitive.Position = mapPos;
                _mainViewport.RenderHost.Camera.Position = mapPos;
            }
        }


        private async Task Render(GameLoopContext context)
        {
            foreach (var vp in GetAllViewPorts())
            {
                vp.CameraInput.HandleInput(vp.Input);
                vp.AnchorHandler.HandleInput(vp.Input);
                vp.Input.Clear();
                vp.RenderHost.RequestRedraw();
            }
        }

        private void DrawHud(HudDrawer hud)
        {
            int yPos = 0;
            hud.DrawLabel(new ScreenVector(10, yPos += 25), new ScreenVector(100, 20), $"FPS:      {_gameLoop.AverageFrameRate}");
            hud.DrawLabel(new ScreenVector(10, yPos += 25), new ScreenVector(100, 20), $"Zoom:     {hud.Context.Camera.Zoom}");
            hud.DrawLabel(new ScreenVector(10, yPos += 25), new ScreenVector(200, 20), $"Attached: {_memoryManager.IsAttached}");
            hud.DrawLabel(new ScreenVector(10, yPos += 25), new ScreenVector(200, 20), $"Position: X={gamePosition.X:F2} Y={gamePosition.Y:F2}");
            hud.DrawLabel(new ScreenVector(10, yPos += 25), new ScreenVector(200, 20), $"Waypoints {_playerWaypoints.Count}");

        }

        ViewportHost CreateViewPortHost(PictureBox pictureBox, RenderPipeline2D pipeLine)
        {
            var camera = new Camera2D();
            var input = new InputQueue();
            var renderHost = new PictureBoxRenderHost(pictureBox, pipeLine, camera);
            var inputSource = new WinFormsInputSource(pictureBox, input);
            var cameraInput = new CameraPanZoomHandler(camera, renderHost.Viewport, 1, 128);
            var anchorHandler = new AnchorPlacementHandler(screenPosition =>
            {
                var mapPosition = renderHost.Viewport.ScreenToWorld(screenPosition, camera);

                var anchor = new Anchor
                {
                    Game = gamePosition,
                    Map = mapPosition,
                };

                _linearRegression.AddPoint(anchor);
                SaveRegressionData();
            });

            return new ViewportHost
            {
                RenderHost = renderHost,
                Input = input,
                CameraInput = cameraInput,
                AnchorHandler = anchorHandler,
            };
        }


        public sealed class WaypointDot : IPrimitive2D
        {
            public Vector2 Position { get; }
            public ColorRgba Color { get; set; } = ColorRgba.Lime;
            public WaypointDot(Vector2 position, ColorRgba color)
            {
                Position = position;
                Color = color;
            }
            public void Render(RenderContext2D context)
            {
                var p = context.Viewport.WorldToScreen(Position, context.Camera);
                context.Graphics.DrawCircle(p, 1f, Color, 1f);
            }
        }


        public sealed class WaypointLog : IDisposable
        {
            private readonly FileStream _stream;
            private readonly StreamWriter _writer;

            private static readonly JsonSerializerOptions JsonOptions = new()
            {
                WriteIndented = false
            };

            public WaypointLog(string path)
            {
                var dir = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                // ShareRead allows your loader or tools to read while you keep it open.
                _stream = new FileStream(
                    path,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.Read,
                    bufferSize: 16 * 1024,
                    options: FileOptions.SequentialScan);

                _writer = new StreamWriter(_stream, System.Text.Encoding.UTF8)
                {
                    AutoFlush = false
                };
            }

            public void Append(Vector2 pos)
            {
                var item = new WaypointLogItem(
                    t: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    x: pos.X,
                    y: pos.Y);

                string json = JsonSerializer.Serialize(item, JsonOptions);
                _writer.WriteLine(json);

                // Choose your durability/perf tradeoff:
                _writer.Flush();         // flush StreamWriter buffer -> FileStream
                _stream.Flush(true);     // flush OS buffers to disk (more expensive)
            }

            public static List<Vector2> LoadAll(string path)
            {
                var list = new List<Vector2>();
                if (!File.Exists(path))
                    return list;

                foreach (var line in File.ReadLines(path))
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    try
                    {
                        var item = JsonSerializer.Deserialize<WaypointLogItem>(line, JsonOptions);
                        list.Add(new Vector2(item.x, item.y));
                    }
                    catch
                    {
                        // Ignore malformed/truncated lines; optionally log it.
                    }
                }

                return list;
            }

            public void Dispose()
            {
                _writer.Dispose();
                _stream.Dispose();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _waypointLog?.Dispose();
            base.OnFormClosed(e);
        }
    }

    record WaypointLogItem(long t, float x, float y);
}







