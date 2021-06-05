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
        Grid2DObject grid2D;

        Shader defaultShader, textShader, pointShader, gridShader;
        TextBlock textBlock;

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
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
            textBlock = new TextBlock("AAAA", new Vector3(0, 0, 0), AssetsManager.Fonts["Default"], new Vector3(0), 0.5f);

            camera = new Camera(new Vector3(0, 0, 5), 0.05f, 2.1f, 2.1f, 0.1f, 100f);

            spline2D = new Spline2DObject(new InterpolationSpline(), new Vector3(1, 0, 1), PointSize.Small, PointShape.Triangle);
            spline2D.Load();

            grid2D = new Grid2DObject(AssetsManager.Fonts["Default"], new Vector3(0, 0, 0), camera.Width, camera.Height, 0.25f, 0.25f);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            gridShader.Use();
            GL.Uniform3(gridShader.Locations["cameraPosition"], camera.Position);
            GL.Uniform1(gridShader.Locations["width"], camera.Width);
            GL.Uniform1(gridShader.Locations["height"], camera.Height);
            GL.Uniform1(gridShader.Locations["screenWidth"], (float)Width);
            GL.Uniform1(gridShader.Locations["screenHeight"], (float)Height);
            GL.Uniform1(gridShader.Locations["dx"], grid2D.Dx);
            GL.Uniform1(gridShader.Locations["dy"], grid2D.Dy);
            GL.Uniform3(gridShader.Locations["Color"], grid2D.Color);
            
            GL.BindVertexArray(grid2D.VAO);
            GL.DrawArrays(PrimitiveType.Quads, 0, 4);
            GL.BindVertexArray(0);
            
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
            GL.Uniform3(textShader.Locations["textColor"], grid2D.Color);
            tmp = camera.Projection;
            GL.UniformMatrix4(defaultShader.Locations["proj"], false, ref tmp);
            tmp = camera.View;
            GL.UniformMatrix4(defaultShader.Locations["view"], false, ref tmp);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textBlock.CurrentFont.Atlas.Tex.TexId);
            foreach (TextBlock textBlock in grid2D.LabelsX)
            {                
                GL.BindVertexArray(textBlock.VAO);
                GL.DrawArrays(PrimitiveType.Quads, 0, textBlock.Text.Length * 4);
                GL.BindVertexArray(0);
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);

            //textShader.Use();
            //GL.Uniform3(textShader.Locations["textColor"], textBlock.Color);
            //GL.ActiveTexture(TextureUnit.Texture0);
            //GL.BindTexture(TextureTarget.Texture2D, textBlock.CurrentFont.Atlas.Tex.TexId);
            //GL.BindVertexArray(textBlock.VAO);
            //GL.DrawArrays(PrimitiveType.Quads, 0, textBlock.Text.Length * 4);
            //GL.BindVertexArray(0);
            //GL.BindTexture(TextureTarget.Texture2D, 0);

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

            grid2D.Update(camera.Position, camera.Width, camera.Height);

            //grid2D.Update(camera.Position, camera.Width, camera.Height);
            base.OnUpdateFrame(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            Vector4 vector = new Vector4(2.0f * e.X / Width - 1.0f, -2.0f * e.Y / Height + 1.0f, 0, 1.0f);
            var proj = camera.Projection;
            var view = camera.View;

            proj.Transpose();
            view.Transpose();

            proj.Invert();
            view.Invert();

            Vector3 result = (view * proj * vector).Xyz;
            spline2D.AddPoint(result.Xy);

            //Matrix4 tmp = camera.View * camera.Projection;
            //tmp.Transpose();
            //spline2D.AddPoint((tmp.Inverted() * vector).Xy + camera.Position.Xy);
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