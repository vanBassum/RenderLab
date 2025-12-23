using Engine2D.Calc;
using System.Numerics;

namespace Engine2D.Rendering.Camera
{
    public sealed class Camera2D
    {
        public WorldVector Position { get; set; } = WorldVector.Zero;
        public float Zoom { get; set; } = 1.0f;
    }
}
