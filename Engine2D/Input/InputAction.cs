using Engine2D.Calc;

namespace Engine2D.Input
{
    public readonly struct InputAction
    {
        public InputActionType Type { get; }

        public WorldVector Position { get; }      // Screen-space
        public WorldVector Delta { get; }         // Movement or scroll
        public InputKey Key { get; }

        private InputAction(InputActionType type, WorldVector position, WorldVector delta, InputKey key)
        {
            Type = type;
            Position = position;
            Delta = delta;
            Key = key;
        }

        public static InputAction PointerDown(WorldVector position)
            => new(InputActionType.PointerDown, position, WorldVector.Zero, InputKey.None);

        public static InputAction PointerUp(WorldVector position)
            => new(InputActionType.PointerUp, position, WorldVector.Zero, InputKey.None);

        public static InputAction PointerMove(WorldVector position, WorldVector delta)
            => new(InputActionType.PointerMove, position, delta, InputKey.None);

        public static InputAction Scroll(WorldVector position, float delta)
            => new(InputActionType.Scroll, position, new WorldVector(0, delta), InputKey.None);

        public static InputAction KeyDown(InputKey key)
            => new(InputActionType.KeyDown, WorldVector.Zero, WorldVector.Zero, key);

        public static InputAction KeyUp(InputKey key)
            => new(InputActionType.KeyUp, WorldVector.Zero, WorldVector.Zero, key);
    }
}
