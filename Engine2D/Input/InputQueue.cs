namespace Engine2D.Input
{
    public sealed class InputQueue
    {
        private readonly List<InputAction> _actions = new();

        public IReadOnlyList<InputAction> Actions => _actions;

        public void Enqueue(InputAction action)
        {
            _actions.Add(action);
        }

        public void Clear()
        {
            _actions.Clear();
        }
    }
}
