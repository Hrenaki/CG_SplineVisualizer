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

        public static Window CurWindow { get; private set; } = null;
        Spline2DObject spline2D;
        Grid2DObject grid2D;

        Shader defaultShader, textShader, pointShader, gridShader;

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            if (CurWindow == null)
                CurWindow = this;

            AssetsManager.Init();
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            defaultShader = AssetsManager.LoadShader("default", new ShaderComponent("Assets\\Shaders\\default.vsh"), new ShaderComponent("Assets\\Shaders\\default.fsh"));
            textShader = AssetsManager.LoadShader("text", new ShaderComponent("Assets\\Shaders\\text.vsh"), new ShaderComponent("Assets\\Shaders\\text.fsh"));
            AssetsManager.LoadShader("pointQuad", new ShaderComponent("Assets\\Shaders\\point.vsh"), new ShaderComponent("Assets\\Shaders\\point_quad.gsh"), new ShaderComponent("Assets\\Shaders\\default.fsh"));
            AssetsManager.LoadShader("pointTriangle", new ShaderComponent("Assets\\Shaders\\point.vsh"), new ShaderComponent("Assets\\Shaders\\point_triangle.gsh"), new ShaderComponent("Assets\\Shaders\\default.fsh"));
            gridShader = AssetsManager.LoadShader("grid", new ShaderComponent("Assets\\Shaders\\grid.vsh"), new ShaderComponent("Assets\\Shaders\\grid.fsh"));

            AssetsManager.LoadFontFrom("Assets\\Fonts\\Tahoma.ttf", 50, 50);

            camera = new Camera(new Vector3(0, 0, 5), 0.05f, 2.1f, 2.1f, 0.1f, 100f);

            spline2D = new Spline2DObject(new InterpolationSpline(), new Vector3(1, 0, 1), PointSize.Medium, PointShape.Triangle);
            spline2D.Load();

            grid2D = new Grid2DObject(AssetsManager.Fonts["Default"], new Vector3(0, 0, 0));

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Matrix4 tmp;

            gridShader.Use();
            GL.Uniform3(gridShader.Locations["cameraPosition"], camera.Position);
            GL.Uniform1(gridShader.Locations["width"], camera.Width);
            GL.Uniform1(gridShader.Locations["height"], camera.Height);
            GL.Uniform1(gridShader.Locations["screenWidth"], Width);
            GL.Uniform1(gridShader.Locations["screenHeight"], Height);
            GL.Uniform1(gridShader.Locations["dx"], grid2D.Dx);
            GL.Uniform1(gridShader.Locations["dy"], grid2D.Dy);
            GL.Uniform3(gridShader.Locations["Color"], grid2D.Color);
            
            GL.BindVertexArray(grid2D.VAO);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.BindVertexArray(0);

            textShader.Use();
            GL.Uniform3(textShader.Locations["textColor"], grid2D.Color);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, grid2D.Font.Atlas.Tex.TexId);
            foreach (TextBlock textBlock in grid2D.LabelsX)
            {
                GL.BindVertexArray(textBlock.VAO);
                GL.DrawArrays(PrimitiveType.Quads, 0, textBlock.Text.Length * 4);
                GL.BindVertexArray(0);
            }
            foreach (TextBlock textBlock in grid2D.LabelsY)
            {
                GL.BindVertexArray(textBlock.VAO);
                GL.DrawArrays(PrimitiveType.Quads, 0, textBlock.Text.Length * 4);
                GL.BindVertexArray(0);
            }
            GL.BindVertexArray(grid2D.Zero.VAO);
            GL.DrawArrays(PrimitiveType.Quads, 0, grid2D.Zero.Text.Length * 4);
            GL.BindVertexArray(0);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            switch (spline2D.PointSample.Shape)
            {
                case PointShape.Quad:
                    pointShader = AssetsManager.Shaders["pointQuad"];
                    break;
                case PointShape.Triangle:
                    pointShader = AssetsManager.Shaders["pointTriangle"];
                    break;
            }
            
            pointShader.Use();
            tmp = camera.Projection;
            GL.UniformMatrix4(pointShader.Locations["proj"], false, ref tmp);
            tmp = camera.View;
            GL.UniformMatrix4(pointShader.Locations["view"], false, ref tmp);
            GL.Uniform1(pointShader.Locations["radius"], spline2D.PointSample.Size);
            GL.Uniform3(pointShader.Locations["aColor"], spline2D.PointSample.Color);
            
            GL.BindVertexArray(spline2D.PointVAO);
            GL.DrawArrays(PrimitiveType.Points, 0, spline2D.BasePointCount);
            GL.BindVertexArray(0);
            
            defaultShader.Use();
            tmp = camera.Projection;
            GL.UniformMatrix4(defaultShader.Locations["proj"], false, ref tmp);
            tmp = camera.View;
            GL.UniformMatrix4(defaultShader.Locations["view"], false, ref tmp);
            
            GL.BindVertexArray(spline2D.VAO);
            if (spline2D.BasePointCount == 1)
                GL.DrawArrays(PrimitiveType.Points, 0, 1);
            else if (spline2D.BasePointCount > 0)
                GL.DrawArrays(PrimitiveType.LineStrip, 0, spline2D.AdditionalPointCount);
            GL.BindVertexArray(0);

            SwapBuffers();
            base.OnRenderFrame(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            InputManager.Update();
            if (InputManager.IsKeyDown(Key.Escape))
                Exit();
            camera.Update();

            grid2D.Dx = (float)Math.Pow(2.0, Math.Floor(Math.Log(camera.ZoomFactor, 2))) / 2.0f;
            grid2D.Dy = grid2D.Dx;
            grid2D.Update();

            spline2D.Update();

            base.OnUpdateFrame(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Matrix4 tmpMatrix = camera.View * camera.Projection;
            tmpMatrix.Transpose();
            tmpMatrix.Invert();

            Vector4 vector = new Vector4(2.0f * e.X / Width - 1.0f, -2.0f * e.Y / Height + 1.0f, 0, 1.0f);
            spline2D.AddPoint((tmpMatrix * vector).Xy);
            spline2D.Update();
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