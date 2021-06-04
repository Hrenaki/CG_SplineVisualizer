using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using NumMath.Splines;
using NumMath;
using OpenTK.Graphics.OpenGL;

namespace CG_SplineVisualizer.Objects
{
    public class SimpleSpline : ISpline2D
    {
        public void CreateSpline(NumMath.Vector2[] points)
        {
            return;
        }

        public double GetValue(double x)
        {
            return -x * x;
        }
    }

    public class Spline2DObject
    {
        List<NumMath.Vector2> points;
        ISpline2D spline;

        public int VAO;
        private int VBO;
        private int EBO;

        int segmentCount = 10;
        public int LineCount { get => 2 * (points.Count - 1) * segmentCount; }
        public Spline2DObject(ISpline2D spline)
        {
            this.spline = spline;
            points = new List<NumMath.Vector2>();
        }

        public void Load()
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 0, new float[] { }, BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 0, new uint[] { }, BufferUsageHint.DynamicDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        public void Update(object sender, MouseButtonEventArgs e)
        {
            int[] data = new int[4];
            GL.GetInteger(GetPName.Viewport, data);

            NumMath.Vector2 newPoint = new NumMath.Vector2(2.0f * e.X / data[2] - 1.0f, -2.0f * e.Y / data[3] + 1.0f);
            
            int i;
            for (i = 0; i < points.Count; i++)
            {
                if (newPoint.X < points[i].X)
                {
                    points.Insert(i, newPoint);
                    break;
                }
            }
            if (i == points.Count)
                points.Add(newPoint);
        }
        public void OnRender()
        {
            int i;
            int pointsCount = points.Count;

            if (pointsCount > 0)
            {
                float[] buff;
                uint[] edges;

                if (pointsCount >= 3)
                {
                    pointsCount = (pointsCount - 1) * segmentCount + 1;
                    buff = new float[pointsCount * 2];

                    spline.CreateSpline(points.ToArray());

                    int pos = 0;
                    for (i = 1; i < points.Count; i++)
                    {
                        double rhs = points[i].X;
                        double lhs = points[i - 1].X;
                        double h = (rhs - lhs) / segmentCount;

                        int j = 0;
                        for (double x = lhs; x < rhs; x = lhs + j * h)
                        {
                            buff[pos++] = (float)x;
                            buff[pos++] = (float)spline.GetValue(x);
                            j++;
                        }
                    }
                    buff[pos] = (float)points[points.Count - 1].X;
                    buff[pos + 1] = (float)spline.GetValue(points[points.Count - 1].X);
                }
                else
                {
                    buff = new float[pointsCount * 2];
                    for (i = 0; i < pointsCount; i++)
                    {
                        buff[2 * i] = (float)points[i].X;
                        buff[2 * i + 1] = (float)points[i].Y;
                    }
                }

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
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

                    GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, edges.Length * sizeof(uint), edges, BufferUsageHint.StreamDraw);
                    GL.DrawElements(PrimitiveType.Lines, edges.Length, DrawElementsType.UnsignedInt, 0);
                }
                else GL.DrawArrays(PrimitiveType.Points, 0, 1);

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }
        }
    }
}