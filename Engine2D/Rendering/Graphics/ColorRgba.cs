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
        public static readonly ColorRgba White = new(255, 255, 255);
        public static readonly ColorRgba Green = new(0, 255, 0);
        public static readonly ColorRgba Red = new(255, 0, 0);
        public static readonly ColorRgba Blue = new(0, 0, 255);
        public static readonly ColorRgba Pink = new(255, 192, 203);
        public static readonly ColorRgba DarkPink = new(128, 32, 64);

    }
}
