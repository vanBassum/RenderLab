using Engine2D.Rendering.Camera;
using System.Numerics;

namespace Engine2D.Tiles.Providers
{
    public sealed class TileCoverageProvider
    {
        public void GetWorldCoverage(Camera2D camera, IViewport2D viewport, out Vector2 worldMin, out Vector2 worldMax)
        {
            // Corner sampling using the viewport’s inverse transform.
            var a = viewport.ScreenToWorld(ScreenVector.Zero, camera);
            var b = viewport.ScreenToWorld(viewport.ScreenSize, camera);

            worldMin = new Vector2(MathF.Min(a.X, b.X), MathF.Min(a.Y, b.Y));
            worldMax = new Vector2(MathF.Max(a.X, b.X), MathF.Max(a.Y, b.Y));
        }
    }
}
