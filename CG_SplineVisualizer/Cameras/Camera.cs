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
        public Vector3 Position { get; set; }

        private Vector3 right = new Vector3(1, 0, 0);
        private Vector3 up = new Vector3(0, 1, 0);
        private Vector3 forward = new Vector3(0, 0, -1);

        public Matrix4 Projection { get => Matrix4.CreateOrthographic(Width, Height, NearPlane, FarPlane); }
        public Matrix4 View { get => Matrix4.LookAt(Position, Position + forward, up); }
        public float Speed { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float NearPlane { get; set; }
        public float FarPlane { get; set; }

        public void Update()
        {
            if (InputManager.IsKeyDown(Key.W))
                Position += Speed * forward;
            if (InputManager.IsKeyDown(Key.S))
                Position -= Speed * forward;
            if (InputManager.IsKeyDown(Key.D))
                Position -= Speed * right;
            if (InputManager.IsKeyDown(Key.A))
                Position -= Speed * right;
            if (InputManager.IsKeyDown(Key.ShiftLeft))
                Position += Speed * up;
            if (InputManager.IsKeyDown(Key.LControl))
                Position -= Speed * up;
        }
        public void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Height += e.Delta * 2;
            Width += e.Delta * 2;
        }
    }
}