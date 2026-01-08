using Engine2D.Calc;
using Engine2D.Rendering.Camera;
using Engine2D.Rendering.Graphics;
using Engine2D.Rendering.Pipeline;

namespace Engine2D.Input
{
    public sealed class CameraPanZoomHandler : IInputHandler
    {
        public int MinZoom { get; set; }
        public int MaxZoom { get; set; }

        private readonly Camera2D _camera;
        private readonly IViewport2D _viewport;

        private bool _dragging;
        private int _zoomStepIndex;

        public CameraPanZoomHandler(Camera2D camera, IViewport2D viewport, int min = 1, int max = 32)
        {
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
            _viewport = viewport ?? throw new ArgumentNullException(nameof(viewport));

            MinZoom = min;
            MaxZoom = max;

            _camera.Zoom = 1.0f;
            _zoomStepIndex = CalculateStepFromZoom(_camera.Zoom);
        }

        public void HandleInput(InputQueue input)
        {
            foreach (var action in input.Actions)
            {
                switch (action.Type)
                {
                    case InputActionType.PointerDown:
                        _dragging = true;
                        break;

                    case InputActionType.PointerUp:
                        _dragging = false;
                        break;

                    case InputActionType.PointerMove:
                        if (_dragging)
                        {
                            var prevScreen = action.Position - action.Delta;
                            var worldPrev = _viewport.ScreenToWorld(prevScreen, _camera);
                            var worldNow = _viewport.ScreenToWorld(action.Position, _camera);
                            var worldDelta = worldNow - worldPrev;

                            _camera.Position -= worldDelta;
                        }
                        break;

                    case InputActionType.Scroll:
                        {
                            int stepDelta = Math.Sign(action.Delta.Y);
                            if (stepDelta == 0)
                                break;

                            var anchor = action.Position; // screen-space
                            StepZoomAround(anchor, stepDelta);
                            break;
                        }
                }
            }
        }

        private void StepZoomAround(ScreenVector anchorScreen, int delta)
        {
            var worldAnchorBefore = _viewport.ScreenToWorld(anchorScreen, _camera);

            float oldZoom = _camera.Zoom;
            float newZoom = ComputeSteppedZoom(delta);

            if (MathF.Abs(newZoom - oldZoom) < 0.0001f)
                return;

            _camera.Zoom = newZoom;

            var worldAnchorAfter = _viewport.ScreenToWorld(anchorScreen, _camera);
            var worldDelta = worldAnchorAfter - worldAnchorBefore;
            _camera.Position -= worldDelta;
        }

        private float ComputeSteppedZoom(int delta)
        {
            int stepA = CalculateStepFromZoom(MinZoom);
            int stepB = CalculateStepFromZoom(MaxZoom);

            int minStep = Math.Min(stepA, stepB);
            int maxStep = Math.Max(stepA, stepB);

            _zoomStepIndex = Math.Clamp(_zoomStepIndex + delta, minStep, maxStep);
            return CalculateZoomFromStep(_zoomStepIndex);
        }

        private int CalculateStepFromZoom(float zoom)
        {
            int baseLevel = (int)MathF.Floor(MathF.Log2(zoom));
            float baseZoom = MathF.Pow(2, baseLevel);
            bool halfStep = zoom >= baseZoom * 1.5f;
            return baseLevel * 2 + (halfStep ? 1 : 0);
        }

        private float CalculateZoomFromStep(int step)
        {
            int baseLevel = step / 2;
            bool halfStep = (step & 1) != 0;

            float baseZoom = MathF.Pow(2, baseLevel);
            return halfStep ? baseZoom * 1.5f : baseZoom;
        }
    }
}
