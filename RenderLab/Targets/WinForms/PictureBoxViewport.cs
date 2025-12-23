using Engine2D.Rendering.Camera;
using Engine2D.Rendering.Pipeline;
using System.Numerics;

namespace RenderLab.Targets.WinForms
{

    public sealed class PictureBoxViewport
    {
        private readonly PictureBox _pictureBox;

        private Bitmap? _buffer;
        private Graphics? _frameGraphics;
        private WinFormsGraphics2D? _frameBackend;

        private IViewport2D? _viewport;

        public Camera2D Camera { get; }

        public PictureBoxViewport(PictureBox pictureBox, Camera2D camera)
        {
            _pictureBox = pictureBox;
            Camera = camera;

            _pictureBox.SizeChanged += (_, _) => Resize();
            Resize();
        }

        private void Resize()
        {
            _buffer?.Dispose();
            _buffer = null;
            _viewport = null;

            if (_pictureBox.Width <= 0 || _pictureBox.Height <= 0)
                return;

            _buffer = new Bitmap(_pictureBox.Width, _pictureBox.Height);
            _pictureBox.Image = _buffer;

            _viewport = new CenteredViewport2D(GetClientSize());
        }

        public void BeginFrame(out RenderContext2D context)
        {
            if (_buffer == null || _viewport == null)
            {
                context = default;
                return;
            }

            _frameGraphics = Graphics.FromImage(_buffer);
            _frameBackend = new WinFormsGraphics2D(_frameGraphics);

            context = new RenderContext2D(
                Camera,
                _viewport,
                _frameBackend
            );
        }

        public void EndFrame()
        {
            _frameGraphics?.Dispose();
            _frameGraphics = null;
            _frameBackend = null;

            _pictureBox.Invalidate();
        }

        private ScreenVector GetClientSize()
            => new ScreenVector(
                _pictureBox.ClientSize.Width,
                _pictureBox.ClientSize.Height
            );
    }
}
