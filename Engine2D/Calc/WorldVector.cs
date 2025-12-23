using System.Numerics;

namespace Engine2D.Calc
{
    public readonly struct WorldVector
    {
        public readonly Vector2 Value;

        public float X => Value.X;
        public float Y => Value.Y;

        // -------------------------
        // Constructors
        // -------------------------

        public WorldVector(float x, float y)
        {
            Value = new Vector2(x, y);
        }

        public WorldVector(Vector2 value)
        {
            Value = value;
        }

        // -------------------------
        // Static properties
        // -------------------------

        public static WorldVector Zero => new WorldVector(Vector2.Zero);
        public static WorldVector One => new WorldVector(Vector2.One);
        public static WorldVector UnitX => new WorldVector(Vector2.UnitX);
        public static WorldVector UnitY => new WorldVector(Vector2.UnitY);

        // -------------------------
        // Length / magnitude
        // -------------------------

        public float Length()
            => Value.Length();

        public float LengthSquared()
            => Value.LengthSquared();

        public bool IsNearlyZero(float epsilon = 0.001f)
            => LengthSquared() <= epsilon * epsilon;

        // -------------------------
        // Operators
        // -------------------------

        public static WorldVector operator +(WorldVector a, WorldVector b)
            => new WorldVector(a.Value + b.Value);

        public static WorldVector operator -(WorldVector a, WorldVector b)
            => new WorldVector(a.Value - b.Value);

        public static WorldVector operator -(WorldVector v)
            => new WorldVector(-v.Value);

        public static WorldVector operator *(WorldVector v, float scalar)
            => new WorldVector(v.Value * scalar);

        public static WorldVector operator /(WorldVector v, float scalar)
            => new WorldVector(v.Value / scalar);

        // -------------------------
        // Equality (INTENTIONALLY STRICT)
        // -------------------------

        public static bool operator ==(WorldVector a, WorldVector b)
            => a.Value == b.Value;

        public static bool operator !=(WorldVector a, WorldVector b)
            => !(a == b);

        public override bool Equals(object? obj)
            => obj is WorldVector other && Value.Equals(other.Value);

        public override int GetHashCode()
            => Value.GetHashCode();

        // -------------------------
        // Conversions
        // -------------------------

        public static implicit operator Vector2(WorldVector v)
            => v.Value;

        public static explicit operator WorldVector(Vector2 v)
            => new WorldVector(v);

        public static WorldVector Normalize(WorldVector v) 
            => new WorldVector(Vector2.Normalize(v.Value));


        // -------------------------
        // Debugging
        // -------------------------

        public override string ToString()
            => $"World({X}, {Y})";
    }
}
