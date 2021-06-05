using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using CG_SplineVisualizer.Objects;
using NumMath.Splines;
using ShaderCompiler;

namespace CG_SplineVisualizer
{
    public class Window : GameWindow
    {
        ICamera camera;
        Spline2DObject spline2D;
        Shader defaultShader, textShader, pointShader;
        TextBlock textBlock;

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            camera = new Camera()
            {
                Speed = 1.0f,
                Width = 2,
                Height = 2,
                NearPlane = 0.1f,
                FarPlane = 100f,
                Position = new Vector3(0, 0, 5)
            };
            AssetsManager.Init();
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            defaultShader = AssetsManager.LoadShader("default", new ShaderComponent("Assets\\Shaders\\default.vsh"), new ShaderComponent("Assets\\Shaders\\default.fsh"));
            textShader = AssetsManager.LoadShader("text", new ShaderComponent("Assets\\Shaders\\text.vsh"), new ShaderComponent("Assets\\Shaders\\text.fsh"));
            AssetsManager.LoadShader("pointQuad", new ShaderComponent("Assets\\Shaders\\point.vsh"), new ShaderComponent("Assets\\Shaders\\point_quad.gsh"), new ShaderComponent("Assets\\Shaders\\default.fsh"));
            AssetsManager.LoadShader("pointTriangle", new ShaderComponent("Assets\\Shaders\\point.vsh"), new ShaderComponent("Assets\\Shaders\\point_triangle.gsh"), new ShaderComponent("Assets\\Shaders\\default.fsh"));

            AssetsManager.LoadFontFrom("Assets\\Fonts\\Tahoma.ttf", 50);
            textBlock = new TextBlock("Hello world!\nSecond line", new Vector3(0, 0, 0), AssetsManager.Fonts["Default"], new Vector3(0.5f, 0.1f, 0.9f), 10);
            
            spline2D = new Spline2DObject(new InterpolationSpline(), new Vector3(1, 0, 1), PointSize.Large, PointShape.Triangle);
            spline2D.Load();

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            switch(spline2D.PointSample.Shape)
            {
                case PointShape.Quad:
                    pointShader = AssetsManager.Shaders["pointQuad"];
                    break;
                case PointShape.Triangle:
                    pointShader = AssetsManager.Shaders["pointTriangle"];
                    break;
            }

            pointShader.Use();
            var tmp = camera.Projection;
            GL.UniformMatrix4(pointShader.Locations["proj"], false, ref tmp);
            tmp = camera.View;
            GL.UniformMatrix4(pointShader.Locations["view"], false, ref tmp);
            GL.Uniform1(pointShader.Locations["radius"], spline2D.PointSample.Size);
            GL.Uniform3(pointShader.Locations["aColor"], spline2D.PointSample.Color);

            GL.BindVertexArray(spline2D.PointVAO);
            GL.DrawArrays(PrimitiveType.Points, 0, spline2D.PointCount);
            GL.BindVertexArray(0);

            defaultShader.Use();
            tmp = camera.Projection;
            GL.UniformMatrix4(defaultShader.Locations["proj"], false, ref tmp);
            tmp = camera.View;
            GL.UniformMatrix4(defaultShader.Locations["view"], false, ref tmp);
            
            GL.BindVertexArray(spline2D.VAO);
            if (spline2D.PointCount == 1)
                GL.DrawArrays(PrimitiveType.Points, 0, 1);
            else if (spline2D.LineCount > 0)
                GL.DrawElements(PrimitiveType.Lines, spline2D.LineCount * 2, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            textShader.Use();
            GL.Uniform3(textShader.Locations["textColor"], textBlock.Color);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textBlock.CurrentFont.Atlas.Tex.TexId);
            GL.BindVertexArray(textBlock.VAO);
            GL.DrawArrays(PrimitiveType.Quads, 0, textBlock.Text.Length * 4);
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            SwapBuffers();
            base.OnRenderFrame(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            InputManager.Update();
            if (InputManager.IsKeyDown(Key.Escape))
                Exit();
            base.OnUpdateFrame(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Vector4 vector = new Vector4(2.0f * e.X / Width - 1.0f, -2.0f * e.Y / Height + 1.0f, 0, 0);
            spline2D.AddPoint(((camera.Projection * camera.View).Inverted() * vector).Xy);
        }
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            camera.OnMouseWheel(this, e);
            base.OnMouseWheel(e);
        }
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            foreach (Shader shader in AssetsManager.Shaders.Values)
                shader.Dispose();
            base.OnUnload(e);
        }
    }
}