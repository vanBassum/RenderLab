using Engine2D.Rendering.Camera;

namespace Engine2D.Input
{
    public sealed class CameraPanZoomHandler : IInputHandler
    {
        private readonly Camera2D _camera;
        private bool _dragging;

        public CameraPanZoomHandler(Camera2D camera)
        {
            _camera = camera;
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
                        _camera.Zoom *= action.Delta.Y > 0 ? 1.1f : 0.9f;
                        break;
                }
            }
        }
    }
}
