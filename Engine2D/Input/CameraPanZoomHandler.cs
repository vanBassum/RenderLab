using Engine2D.Rendering.Camera;

namespace Engine2D.Input
{
    public sealed class CameraPanZoomHandler : IInputHandler
    {
        private readonly Camera2D _camera;
        private bool _dragging;

        private int _zoomStepIndex = 0;

        private const int MinStep = -8;
        private const int MaxStep = 12;

        private int _zoomLevel = 0;

        public CameraPanZoomHandler(Camera2D camera)
        {
            _camera = camera;
            _camera.Zoom = 1.0f;
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

                    case InputActionType.PointerMove when _dragging:
                        _camera.Position -= action.Delta / _camera.Zoom;
                        break;

                    case InputActionType.Scroll:
                        StepZoom(action.Delta.Y > 0 ? +1 : -1);
                        break;
                }
            }
        }

        private void StepZoom(int delta)
        {
            _zoomStepIndex = Math.Clamp(
                _zoomStepIndex + delta,
                MinStep,
                MaxStep);

            int baseLevel = _zoomStepIndex / 2;
            bool halfStep = (_zoomStepIndex & 1) != 0;

            float baseZoom = MathF.Pow(2, baseLevel);

            _camera.Zoom = halfStep
                ? baseZoom + baseZoom * 0.5f
                : baseZoom;
        }

    }
}
