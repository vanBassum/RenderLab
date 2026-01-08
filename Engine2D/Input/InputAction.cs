using Engine2D.Calc;

namespace Engine2D.Input
{
    public readonly struct InputAction
    {
        public InputActionType Type { get; }

        public ScreenVector Position { get; }      // Screen-space
        public ScreenVector Delta { get; }         // Movement or scroll
        public InputKey Key { get; }

        private InputAction(InputActionType type, ScreenVector position, ScreenVector delta, InputKey key)
        {
            Type = type;
            Position = position;
            Delta = delta;
            Key = key;
        }

        public static InputAction PointerDown(ScreenVector position)
            => new(InputActionType.PointerDown, position, ScreenVector.Zero, InputKey.None);

        public static InputAction PointerUp(ScreenVector position)
            => new(InputActionType.PointerUp, position, ScreenVector.Zero, InputKey.None);

        public static InputAction PointerMove(ScreenVector position, ScreenVector delta)
            => new(InputActionType.PointerMove, position, delta, InputKey.None);

        public static InputAction Scroll(ScreenVector position, int delta)
            => new(InputActionType.Scroll, position, new ScreenVector(0, delta), InputKey.None);

        public static InputAction KeyDown(InputKey key)
            => new(InputActionType.KeyDown, ScreenVector.Zero, ScreenVector.Zero, key);

        public static InputAction KeyUp(InputKey key)
            => new(InputActionType.KeyUp, ScreenVector.Zero, ScreenVector.Zero, key);
    }
}
