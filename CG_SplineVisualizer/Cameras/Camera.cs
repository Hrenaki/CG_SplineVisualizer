using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Input;

namespace CG_SplineVisualizer
{
    public class Camera : ICamera
    {
        public static ICamera CurCamera { get; private set; } = null;
        public Vector3 Position { get; set; }

        private Vector3 right = new Vector3(1, 0, 0);
        private Vector3 up = new Vector3(0, 1, 0);
        private Vector3 forward = new Vector3(0, 0, -1);

        public Matrix4 Projection
        {
            get
            {
                //return Matrix4.CreateOrthographic(Width, Height, NearPlane, FarPlane);
                return new Matrix4(2 / Width, 0, 0, 0,
                                   0, 2 / Height, 0, 0,
                                   0, 0, 1 / (FarPlane - NearPlane), 0,
                                   0, 0, -NearPlane / (FarPlane - NearPlane), 1);
            }
        }
        public Matrix4 View 
        { 
            get
            {
                return new Matrix4(right.X, up.X, forward.X, 0,
                                   right.Y, up.Y, forward.Y, 0,
                                   right.Z, up.Z, forward.Z, 0,
                                   Vector3.Dot(right, -Position), Vector3.Dot(up, -Position), Vector3.Dot(forward, -Position), 1);
            }
        }
        public float Speed { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float NearPlane { get; set; }
        public float FarPlane { get; set; }
        public float ZoomFactor { get; set; }

        private float originSpeed;
        private float originHeight;
        private float originWidth;

        public Camera(Vector3 position, float speed, float width, float height, float nearPlane, float farPlane)
        {
            if (CurCamera == null)
                CurCamera = this;

            Position = position;
            Speed = speed;
            Width = width;
            Height = height;
            NearPlane = nearPlane;
            FarPlane = farPlane;

            ZoomFactor = 1.0f;
            originSpeed = Speed;
            originWidth = Width;
            originHeight = Height;
        }

        public void Update()
        {
            if (InputManager.IsKeyDown(Key.W))
                Position += Speed * forward;
            if (InputManager.IsKeyDown(Key.S))
                Position -= Speed * forward;
            if (InputManager.IsKeyDown(Key.D))
                Position += Speed * right;
            if (InputManager.IsKeyDown(Key.A))
                Position -= Speed * right;
            if (InputManager.IsKeyDown(Key.ShiftLeft))
                Position += Speed * up;
            if (InputManager.IsKeyDown(Key.LControl))
                Position -= Speed * up;
        }
        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
                ZoomFactor *= 1.1f;
            else ZoomFactor /= 1.1f;

            if (ZoomFactor == 0)
                ZoomFactor = float.Epsilon;

            Speed = ZoomFactor * originSpeed;
            Height = ZoomFactor * originHeight;
            Width = ZoomFactor * originWidth;
        }

        public void MakeCurrent()
        {
            CurCamera = this;
        }
    }
}