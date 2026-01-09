using Engine2D.Calc;
using Engine2D.Input;

namespace RenderLab.Targets.WinForms
{
    public sealed class WinFormsInputSource
    {
        private readonly Control _control;
        private readonly InputQueue _queue;

        private ScreenVector _lastMousePos;

        public WinFormsInputSource(Control control, InputQueue queue)
        {
            _control = control;
            _queue = queue;

            _control.MouseDown += OnMouseDown;
            _control.MouseUp += OnMouseUp;
            _control.MouseMove += OnMouseMove;
            _control.MouseWheel += OnMouseWheel;
            _control.KeyDown += OnKeyDown;
            _control.KeyUp += OnKeyUp;

            _control.Focus();
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            _lastMousePos = new ScreenVector(e.X, e.Y);
            _queue.Enqueue(InputAction.PointerDown(_lastMousePos, TranslateMouseButton(e.Button)));
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            var pos = new ScreenVector(e.X, e.Y);
            _queue.Enqueue(InputAction.PointerUp(pos, TranslateMouseButton(e.Button)));
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            var pos = new ScreenVector(e.X, e.Y);
            var delta = pos - _lastMousePos;
            _lastMousePos = pos;

            if (delta != ScreenVector.Zero)
                _queue.Enqueue(InputAction.PointerMove(pos, delta));
        }

        private void OnMouseWheel(object? sender, MouseEventArgs e)
        {
            var pos = new ScreenVector(e.X, e.Y);
            _queue.Enqueue(InputAction.Scroll(pos, e.Delta));
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            var key = TranslateKey(e.KeyCode);
            if (key != InputKey.None)
                _queue.Enqueue(InputAction.KeyDown(key));
        }

        private void OnKeyUp(object? sender, KeyEventArgs e)
        {
            var key = TranslateKey(e.KeyCode);
            if (key != InputKey.None)
                _queue.Enqueue(InputAction.KeyUp(key));
        }

        private static InputMouseButton TranslateMouseButton(MouseButtons btn)
        {
            return btn switch
            {
                MouseButtons.Left => InputMouseButton.Left,
                MouseButtons.Right => InputMouseButton.Right,
                MouseButtons.Middle => InputMouseButton.Middle,
                _ => InputMouseButton.None
            };
        }

        private static InputKey TranslateKey(Keys key)
        {
            return key switch
            {
                Keys.W => InputKey.W,
                Keys.A => InputKey.A,
                Keys.S => InputKey.S,
                Keys.D => InputKey.D,

                Keys.Left => InputKey.Left,
                Keys.Right => InputKey.Right,
                Keys.Up => InputKey.Up,
                Keys.Down => InputKey.Down,

                Keys.Space => InputKey.Space,
                Keys.Escape => InputKey.Escape,

                _ => InputKey.None
            };
        }
    }
}
