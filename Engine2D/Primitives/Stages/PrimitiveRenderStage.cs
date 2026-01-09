using Engine2D.Calc;
using Engine2D.Primitives.Abstractions;
using Engine2D.Primitives.Filtering;
using Engine2D.Primitives.Geometry;
using Engine2D.Rendering.Camera;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;
using System.Numerics;

namespace Engine2D.Primitives.Stages
{
    public sealed class PrimitiveRenderStage : IRenderer2D
    {
        private readonly Func<IEnumerable<IPrimitive2D>> _getPrimitives;
        private readonly WorldCoverageLineFilter _filter;

        public PrimitiveRenderStage(Func<IEnumerable<IPrimitive2D>> getPrimitives)
        {
            _getPrimitives = getPrimitives;
            _filter = new WorldCoverageLineFilter();
        }

        public void Render(in RenderContext2D context)
        {
            var worldMin = context.Viewport.GetWorldMin(context.Camera);
            var worldMax = context.Viewport.GetWorldMax(context.Camera);

            foreach (var primitive in GetVisiblePrimitives(_getPrimitives(), context.Camera, context.Viewport, worldMin, worldMax))
            {
                primitive.Render(context);
            }
        }

        private IEnumerable<IPrimitive2D> GetVisiblePrimitives(IEnumerable<IPrimitive2D> all, Camera2D camera, IViewport2D viewport, Vector2 worldMin, Vector2 worldMax)
        {
            foreach (var primitive in all)
            {
                if (_filter.IsVisible(primitive, camera, viewport, worldMin, worldMax))
                {
                    yield return primitive;
                }
            }
        }
    }
}
