using Engine2D.Rendering.Camera;

namespace Engine2D.Input
{
    public sealed class CameraPanZoomHandler : IInputHandler
    {
        public int MinZoom { get; set; }
        public int MaxZoom { get; set; }

        private readonly Camera2D _camera;
        private bool _dragging;
        private int _zoomStepIndex = 0;

        public CameraPanZoomHandler(Camera2D camera, int min = 1, int max = 32)
        {
            _camera = camera;
            _camera.Zoom = 1.0f;
            MinZoom = min;
            MaxZoom = max;
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
                        StepZoomSimple(action.Delta.Y > 0 ? +1 : -1);
                        break;
                }
            }
        }

        private void StepZoomSimple(int delta)
        {
            float zoomFactor = delta > 0 ? 1.25f : 0.8f;
            float newZoom = _camera.Zoom * zoomFactor;
            newZoom = Math.Clamp(newZoom, MinZoom, MaxZoom);
            _camera.Zoom = newZoom;
        }

        private void StepZoom(int delta)
        {
            int maxStep = CalculateStepFromZoom(MaxZoom);
            int minStep = CalculateStepFromZoom(MinZoom);

            _zoomStepIndex = Math.Clamp(_zoomStepIndex + delta, minStep, maxStep);
            float newZoom = CalculateZoomFromStep(_zoomStepIndex);

            _camera.Zoom = newZoom;
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
            int baseLevel = _zoomStepIndex / 2;
            bool halfStep = (_zoomStepIndex & 1) != 0;

            float baseZoom = MathF.Pow(2, baseLevel);
            return halfStep ? baseZoom * 1.5f : baseZoom;
        }
    }
}

