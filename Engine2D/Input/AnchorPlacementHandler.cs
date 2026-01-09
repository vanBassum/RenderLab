using Engine2D.Calc;

namespace Engine2D.Input
{
    public sealed class AnchorPlacementHandler : IInputHandler
    {
        private readonly Action<ScreenVector> _onAnchor;

        public AnchorPlacementHandler(Action<ScreenVector> onAnchor)
        {
            _onAnchor = onAnchor ?? throw new ArgumentNullException(nameof(onAnchor));
        }

        public void HandleInput(InputQueue input)
        {
            foreach (var action in input.Actions)
            {
                if (action.Type == InputActionType.PointerDown &&
                    action.MouseButton == InputMouseButton.Right)
                {
                    _onAnchor(action.Position);
                }
            }
        }
    }
}
