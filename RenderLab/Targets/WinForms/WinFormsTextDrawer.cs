namespace RenderLab.Targets.WinForms
{
    public sealed class WinFormsTextDrawer : IDisposable
    {
        public Font Font { get; }
        public int LineHeight { get; }

        // Most common “fast and tight” flags for overlays
        private const TextFormatFlags Flags = TextFormatFlags.NoPadding | TextFormatFlags.NoClipping | TextFormatFlags.SingleLine;

        public WinFormsTextDrawer(string fontName = "Consolas", float pxSize = 10f)
        {
            // Pixel units are usually what you want for overlays
            Font = new Font(fontName, pxSize, FontStyle.Regular, GraphicsUnit.Pixel);
            LineHeight = Font.Height;
        }

        public void Draw(Graphics g, int x, int y, string text, Color color)
        {
            // Point overload is fine, but Rectangle gives more control later
            TextRenderer.DrawText(g, text, Font, new Point(x, y), color, Flags);
        }

        public void Dispose() => Font.Dispose();
    }
}


