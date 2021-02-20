using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShaderCompiler;
using SplineBuilder;

namespace CG_SplineVisualizer
{
    public class Window : GameWindow
    {
        List<float[]> splinePoints;
        Spline spline;
        int segmentCount = 10;

        Shader shader;

        int pointsVBO;
        int pointsVAO;
        int pointsEBO;

        public Window(int width, int height, string title) : base(width, height, GraphicsMode.Default, title)
        {
            splinePoints = new List<float[]>();
        }
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            pointsVBO = GL.GenBuffer();
            pointsVAO = GL.GenVertexArray();
            pointsEBO = GL.GenBuffer();

            GL.BindVertexArray(pointsVAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, pointsVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 0, new float[] { }, BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, pointsEBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 0, new uint[] { }, BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            shader = new Shader("shader.vert", "shader.frag");
            base.OnLoad(e);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            shader.Use();

            int i;
            int pointsCount = splinePoints.Count;

            if (pointsCount > 0)
            {
                GL.BindVertexArray(pointsVAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, pointsVBO);

                float[] buff;
                uint[] edges;

                if (pointsCount >= 3)
                {
                    pointsCount = (pointsCount - 1) * segmentCount + 1;
                    buff = new float[pointsCount * 2];

                    spline = new Spline(splinePoints);
                    spline.CreateSpline();

                    int pos = 0;
                    for (i = 1; i < splinePoints.Count; i++)
                    {
                        double rhs = splinePoints[i][0];
                        double lhs = splinePoints[i - 1][0];
                        double h = (rhs - lhs) / segmentCount;

                        int j = 0;
                        for (double x = lhs; x < rhs; x = lhs + j * h)
                        {
                            buff[pos++] = (float)x;
                            buff[pos++] = (float)spline.getValue(x);
                            j++;
                        }
                    }
                    buff[pos] = splinePoints[splinePoints.Count - 1][0];
                    buff[pos + 1] = splinePoints[splinePoints.Count - 1][1];
                }
                else
                {
                    buff = new float[pointsCount * 2];
                    for (i = 0; i < pointsCount; i++)
                    {
                        buff[2 * i] = splinePoints[i][0];
                        buff[2 * i + 1] = splinePoints[i][1];
                    }
                }

                GL.BufferData(BufferTarget.ArrayBuffer, buff.Length * sizeof(float), buff, BufferUsageHint.StreamDraw);

                if (pointsCount > 1)
                {
                    edges = new uint[pointsCount * 2 - 2];
                    edges[0] = 0;
                    for (i = 1; i < pointsCount - 1; i++)
                    {
                        edges[2 * i - 1] = (uint)i;
                        edges[2 * i] = edges[2 * i - 1];
                    }
                    edges[edges.Length - 1] = (uint)(pointsCount - 1);

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, pointsEBO);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, edges.Length * sizeof(uint), edges, BufferUsageHint.StreamDraw);
                    GL.DrawElements(PrimitiveType.Lines, edges.Length, DrawElementsType.UnsignedInt, 0);
                }
                else GL.DrawArrays(PrimitiveType.Points, 0, 1);

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            SwapBuffers();
            base.OnRenderFrame(e);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            if (Keyboard.GetState().IsKeyDown(Key.Escape))
                Exit();

            base.OnUpdateFrame(e);
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            float[] newPoint = new float[] { 2.0f * e.X / (float)Width - 1.0f, -2.0f * e.Y / (float)Height + 1.0f };
            
            if (splinePoints.Count != 0)
            {
                int i;
                for(i = 0; i < splinePoints.Count; i++)
                {
                    if(newPoint[0] < splinePoints[i][0])
                    {
                        splinePoints.Insert(i, newPoint);
                        break;
                    }
                }
                if (i == splinePoints.Count)
                    splinePoints.Add(newPoint);
            }
            else splinePoints.Add(newPoint);
            
            base.OnMouseDown(e);
        }
        protected override void OnUnload(EventArgs e)
        {
            shader.Dispose();
            GL.DeleteBuffer(pointsVBO);
            base.OnUnload(e);
        }
    }
}
