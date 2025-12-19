using Engine2D.Rendering.Camera;
using Engine2D.Rendering.Pipeline;
using System.Numerics;

namespace RenderLab.Targets.WinForms
{
    // =========================
    // PictureBox viewport (frame host)
    // =========================

    public sealed class PictureBoxViewport
    {
        private readonly PictureBox _pictureBox;
        private Bitmap? _buffer;

        private Graphics? _frameGraphics;
        private WinFormsGraphics2D? _frameBackend;

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

            if (_pictureBox.Width <= 0 || _pictureBox.Height <= 0)
                return;

            _buffer = new Bitmap(_pictureBox.Width, _pictureBox.Height);
            _pictureBox.Image = _buffer;

            Camera.ViewportSize = GetClientSize();
        }

        public void BeginFrame(out RenderContext2D context)
        {
            if (_buffer == null)
            {
                context = default;
                return;
            }

            // Acquire a backend for this frame.
            _frameGraphics = Graphics.FromImage(_buffer);
            _frameBackend = new WinFormsGraphics2D(_frameGraphics);

            // Keep camera viewport current in case layout changed.
            Camera.ViewportSize = GetClientSize();

            context = new RenderContext2D(Camera, _frameBackend);
        }

        public void EndFrame()
        {
            // Release backend resources.
            _frameGraphics?.Dispose();
            _frameGraphics = null;
            _frameBackend = null;

            _pictureBox.Invalidate();
        }

        Vector2 GetClientSize()
            => new Vector2(_pictureBox.ClientSize.Width, _pictureBox.ClientSize.Height);
    }
}
