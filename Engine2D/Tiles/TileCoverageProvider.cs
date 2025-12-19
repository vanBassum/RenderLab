using Engine2D.Rendering.Camera;
using System.Numerics;

namespace Engine2D.Tiles
{
    public sealed class TileCoverageProvider
    {
        private readonly Camera2D _camera;

        public TileCoverageProvider(Camera2D camera)
        {
            _camera = camera;
        }

        public void GetWorldCoverage(Vector2 viewportSize, out Vector2 worldMin, out Vector2 worldMax)
        {
            Vector2 halfViewportWorld =
                viewportSize / _camera.Zoom * 0.5f;

            // Padding: always request slightly more than visible area
            // This avoids missing edge tiles due to rounding
            const float paddingFactor = 1.1f;

            halfViewportWorld *= paddingFactor;

            worldMin = _camera.Position - halfViewportWorld;
            worldMax = _camera.Position + halfViewportWorld;
        }

    }
}
