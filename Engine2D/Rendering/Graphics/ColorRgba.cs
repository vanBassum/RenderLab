namespace Engine2D.Rendering.Graphics
{
    public readonly struct ColorRgba
    {
        public readonly byte R;
        public readonly byte G;
        public readonly byte B;
        public readonly byte A;

        public ColorRgba(byte r, byte g, byte b, byte a = 255)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public static readonly ColorRgba Black = new(0, 0, 0);
        public static readonly ColorRgba Lime = new(0, 255, 0);
    }
}
