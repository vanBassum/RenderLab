using System.Numerics;

namespace Engine2D.Input
{
    public readonly struct InputAction
    {
        public InputActionType Type { get; }

        public Vector2 Position { get; }      // Screen-space
        public Vector2 Delta { get; }         // Movement or scroll
        public InputKey Key { get; }

        private InputAction(
            InputActionType type,
            Vector2 position,
            Vector2 delta,
            InputKey key)
        {
            Type = type;
            Position = position;
            Delta = delta;
            Key = key;
        }

        public static InputAction PointerDown(Vector2 position)
            => new(InputActionType.PointerDown, position, Vector2.Zero, InputKey.None);

        public static InputAction PointerUp(Vector2 position)
            => new(InputActionType.PointerUp, position, Vector2.Zero, InputKey.None);

        public static InputAction PointerMove(Vector2 position, Vector2 delta)
            => new(InputActionType.PointerMove, position, delta, InputKey.None);

        public static InputAction Scroll(Vector2 position, float delta)
            => new(InputActionType.Scroll, position, new Vector2(0, delta), InputKey.None);

        public static InputAction KeyDown(InputKey key)
            => new(InputActionType.KeyDown, Vector2.Zero, Vector2.Zero, key);

        public static InputAction KeyUp(InputKey key)
            => new(InputActionType.KeyUp, Vector2.Zero, Vector2.Zero, key);
    }
}
