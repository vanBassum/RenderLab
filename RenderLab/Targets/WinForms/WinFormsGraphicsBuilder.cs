using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace RenderLab.Targets.WinForms
{
    public sealed class WinFormsGraphicsBuilder : IDisposable
    {
        private readonly WinFormsTextDrawer _text;

        public WinFormsGraphicsBuilder()
        {
            _text = new WinFormsTextDrawer("Consolas", 12f);
        }

        public WinFormsGraphics2D Create(Graphics g)
        {
            ConfigureGraphics(g);
            return new WinFormsGraphics2D(g, _text);
        }

        private static void ConfigureGraphics(Graphics g)
        {
            g.CompositingMode = CompositingMode.SourceCopy;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = PixelOffsetMode.Half;
            g.SmoothingMode = SmoothingMode.None;
            g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit; // optional
        }

        public void Dispose() => _text.Dispose();
    }



}



