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
using System.Reflection;
using System.IO;
using SharpFont;
using System.Runtime.InteropServices;

namespace CG_SplineVisualizer
{
    public class Window : GameWindow
    {
        ICamera camera;
        Spline2DObject spline2D;
        Shader shader;
        Shader textShader;
        TextBlock textBlock;

        Vector3 textColor = new Vector3(1, 0, 0);

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

            //this.MouseDown += InputManager.OnMouseDown;
            //this.MouseWheel += InputManager.OnMouseWheel;
            //
            //InputManager.MouseDown += spline2D.Update;
            //InputManager.MouseWheel += camera.OnMouseWheel;

            AssetsManager.LoadFontFrom("Assets\\Fonts\\Tahoma.ttf", 50);
            textBlock = new TextBlock("Hello world!\nSecond line", new Vector3(0, 0, 0), AssetsManager.Fonts["Default"], 10);
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            //spline2D.Load();

            //shader = new Shader("shader.vert", "shader.frag");
            textShader = new Shader("Assets\\Shaders\\textShader.vert", "Assets\\Shaders\\textShader.frag");

            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //spline2D.OnRender();
            //
            //shader.Use();
            //
            //GL.BindVertexArray(spline2D.VAO);
            //GL.DrawElements(PrimitiveType.Lines, spline2D.LineCount, DrawElementsType.UnsignedInt, 0);
            //GL.BindVertexArray(0);

            textShader.Use();

            textShader.SetVector3("textColor", textColor);

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
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            textShader.Dispose();
            base.OnUnload(e);
        }
    }
}