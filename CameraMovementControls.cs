using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Graphics
{
    public class CameraMovementControls
    {
        private readonly Camera _camera;
        private readonly GameWindow _window;

        private Vector2 _lastMousePos;
        private bool _firstMove = true;

        public CameraMovementControls(Camera camera, GameWindow window)
        {
            _camera = camera;
            _window = window;

            _window.MouseMove += OnMouseMove;
            _window.MouseWheel += OnMouseWheel;
        }

        public void Update(float deltaTime)
        {
            var input = _window.KeyboardState;

            if (input.IsKeyDown(Keys.W))
                _camera.ProcessKeyboard(CameraMovement.Forward, deltaTime);
            if (input.IsKeyDown(Keys.S))
                _camera.ProcessKeyboard(CameraMovement.Backward, deltaTime);
            if (input.IsKeyDown(Keys.A))
                _camera.ProcessKeyboard(CameraMovement.Left, deltaTime);
            if (input.IsKeyDown(Keys.D))
                _camera.ProcessKeyboard(CameraMovement.Right, deltaTime);
            if (input.IsKeyDown(Keys.Space))
                _camera.ProcessKeyboard(CameraMovement.Up, deltaTime);
            if (input.IsKeyDown(Keys.LeftShift))
                _camera.ProcessKeyboard(CameraMovement.Down, deltaTime);
        }

        private void OnMouseMove(MouseMoveEventArgs e)
        {
            if (!_window.IsFocused)
            {
                return;
            }

            if (!_window.MouseState.IsButtonDown(MouseButton.Right))
                return;

            if (_firstMove)
            {
                _lastMousePos = new Vector2(e.X, e.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = e.X - _lastMousePos.X;
                var deltaY = e.Y - _lastMousePos.Y;
                _lastMousePos = new Vector2(e.X, e.Y);

                _camera.ProcessMouseMovement(deltaX, deltaY);
            }
        }

        private void OnMouseWheel(MouseWheelEventArgs e)
        {
            _camera.ProcessMouseScroll(e.OffsetY);
        }
    }

}
