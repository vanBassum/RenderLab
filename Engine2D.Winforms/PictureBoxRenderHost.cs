using Engine2D.Calc;
using Engine2D.Rendering.Camera;
using Engine2D.Rendering.Pipeline;
using System.Drawing.Drawing2D;

namespace RenderLab.Targets.WinForms
{
    public sealed class PictureBoxRenderHost : IDisposable
    {
        private readonly PictureBox _pictureBox;
        private readonly RenderPipeline2D _pipeline;
        private readonly WinFormsGraphicsBuilder _graphicsBuilder;


        public Camera2D Camera { get; }
        public CenteredViewport2D Viewport { get; private set; } = null!;

        public PictureBoxRenderHost(PictureBox pictureBox, RenderPipeline2D pipeline, Camera2D camera)
        {
            _pictureBox = pictureBox;
            _pipeline = pipeline;
            Camera = camera;

            _graphicsBuilder = new WinFormsGraphicsBuilder();

            _pictureBox.Paint += OnPaint;
            _pictureBox.Resize += (_, _) => UpdateViewport();

            Viewport = new CenteredViewport2D(new ScreenVector(_pictureBox.ClientSize.Width, _pictureBox.ClientSize.Height));

            UpdateViewport();
        }

        public void Dispose()
        {
            _graphicsBuilder.Dispose();
        }

        public void RequestRedraw()
        {
            _pictureBox.Invalidate();
        }

        private void UpdateViewport()
        {
            if (_pictureBox.ClientSize.Width <= 0 || _pictureBox.ClientSize.Height <= 0)
                return;

            Viewport.ScreenSize = new ScreenVector(_pictureBox.ClientSize.Width, _pictureBox.ClientSize.Height);
        }

        private void OnPaint(object? sender, PaintEventArgs e)
        {
            if (Viewport == null)
                return;

            using var backend = _graphicsBuilder.Create(e.Graphics);

            var context = new RenderContext2D(Camera, Viewport, backend);

            _pipeline.Render(context);
        }

        private static void ConfigureGraphics(Graphics g)
        {
            g.CompositingMode = CompositingMode.SourceCopy;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.SmoothingMode = SmoothingMode.None;
        }
    }



}



