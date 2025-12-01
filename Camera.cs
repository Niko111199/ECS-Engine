using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace Graphics
{
    public class Camera
    {
        public Vector3 Position;
        public float Pitch;
        public float Yaw = -90f;
        public float Roll = 0f;

        public float Fov = 45f;
        private float _aspectRatio;
        public float AspectRatio
        {
            get => _aspectRatio;
            set => _aspectRatio = value;
        }

        public float Speed = 2.5f;
        public float Sensitivity = 0.1f;
        public float ZoomSensitivity = 1.5f;

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            _aspectRatio = aspectRatio;
        }

        public void UpdateAspectRatio(int width, int height)
        {
            _aspectRatio = (float)width / (float)height;
        }


        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + GetFront(), GetUp());
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Fov), _aspectRatio, 0.1f, 100f);
        }

        public void ProcessKeyboard(CameraMovement direction, float deltaTime)
        {
            float velocity = Speed * deltaTime;

            if (direction == CameraMovement.Forward)
                Position += GetFront() * velocity;
            if (direction == CameraMovement.Backward)
                Position -= GetFront() * velocity;
            if (direction == CameraMovement.Left)
                Position -= GetRight() * velocity;
            if (direction == CameraMovement.Right)
                Position += GetRight() * velocity;
            if (direction == CameraMovement.Up)
                Position += GetUp() * velocity;
            if (direction == CameraMovement.Down)
                Position -= GetUp() * velocity;
        }

        public void ProcessMouseMovement(float deltaX, float deltaY, bool constrainPitch = true)
        {
            deltaX *= Sensitivity;
            deltaY *= Sensitivity;

            Yaw += deltaX;
            Pitch -= deltaY;

            if (constrainPitch)
            {
                if (Pitch > 89f) Pitch = 89f;
                if (Pitch < -89f) Pitch = -89f;
            }
        }

        public void ProcessMouseScroll(float offsetY)
        {
            Fov -= offsetY * ZoomSensitivity;
            if (Fov < 1f) Fov = 1f;
            if (Fov > 90f) Fov = 90f;
        }

        private Vector3 GetFront()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            return Vector3.Normalize(front);
        }

        private Vector3 GetRight()
        {
            return Vector3.Normalize(Vector3.Cross(GetFront(), Vector3.UnitY));
        }

        private Vector3 GetUp()
        {
            return Vector3.Normalize(Vector3.Cross(GetRight(), GetFront()));
        }

        public void goToCamera(CameraComponent target)
        {
            this.Position = target.Camera.Position;
            this.Yaw = target.Camera.Yaw;
            this.Pitch = target.Camera.Pitch;
            this.Roll = target.Camera.Roll;
            this.Fov = target.Camera.Fov;
        }
    }

    public enum CameraMovement
    {
        Forward,
        Backward,
        Left,
        Right,
        Up,
        Down
    }
}
