namespace Engine2D.Calc
{
    public struct ScreenVector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public ScreenVector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static ScreenVector Zero => new ScreenVector(0, 0);

        // -------------------------
        // Operators
        // -------------------------

        public static ScreenVector operator +(ScreenVector a, ScreenVector b)
            => new ScreenVector(a.X + b.X, a.Y + b.Y);

        public static ScreenVector operator -(ScreenVector a, ScreenVector b)
            => new ScreenVector(a.X - b.X, a.Y - b.Y);

        public static ScreenVector operator +(ScreenVector a, int value)
            => new ScreenVector(a.X + value, a.Y + value);

        public static ScreenVector operator -(ScreenVector a, int value)
            => new ScreenVector(a.X - value, a.Y - value);

        public static ScreenVector operator *(ScreenVector a, int scalar)
            => new ScreenVector(a.X * scalar, a.Y * scalar);

        public static ScreenVector operator /(ScreenVector a, int divisor)
            => new ScreenVector(a.X / divisor, a.Y / divisor);

        public static bool operator ==(ScreenVector a, ScreenVector b)
            => a.X == b.X && a.Y == b.Y;

        public static bool operator !=(ScreenVector a, ScreenVector b)
            => !(a == b);

        // -------------------------
        // Overrides
        // -------------------------

        public override bool Equals(object? obj)
            => obj is ScreenVector other && this == other;

        public override int GetHashCode()
            => HashCode.Combine(X, Y);

        public override string ToString()
            => $"({X}, {Y})";
    }
}
