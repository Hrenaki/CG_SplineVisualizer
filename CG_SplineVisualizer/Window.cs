using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CG_SplineVisualizer.Objects;
using NumMath.Splines;
using ShaderCompiler;
using System.Drawing;

namespace CG_SplineVisualizer
{
    public class Window : GameWindow
    {
        ICamera camera;
        Spline2DObject spline2D;
        Shader shader;
        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            camera = new Camera()
            {
                Speed = 1.0f,
                Width = 2,
                Height = 2,
                NearPlane = 0.1f,
                FarPlane = 100f
            };

            spline2D = new Spline2DObject(new InterpolationSpline());

            this.MouseDown += InputManager.OnMouseDown;

            InputManager.MouseDown += spline2D.Update;
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            spline2D.Load();

            shader = new Shader("shader.vert", "shader.frag");
            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            spline2D.OnRender();

            shader.Use();

            GL.BindVertexArray(spline2D.VAO);
            //GL.DrawArrays(PrimitiveType.Points, 0, 2);
            GL.DrawElements(PrimitiveType.Lines, spline2D.LineCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            SwapBuffers();
            base.OnRenderFrame(e);
        }
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            //Console.WriteLine(e.X + " " + e.Y);
            base.OnMouseMove(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            InputManager.Update();

            if (InputManager.IsKeyDown(Key.Escape))
                Exit();

            base.OnUpdateFrame(e);
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            shader.Dispose();
            base.OnUnload(e);
        }
    }
}
