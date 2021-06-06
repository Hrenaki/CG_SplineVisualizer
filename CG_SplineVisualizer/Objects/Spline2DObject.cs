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
    public class Spline2DObject
    {
        public int BasePointCount { get => points.Count; }
        public int AdditionalPointCount { get; private set; }
        private int lastAddedPointIndex = -1;

        List<NumMath.Vector2> points;

        public PointSample PointSample { get; set; }

        ISpline2D spline;

        public int VAO { get; private set; }
        private int VBO;
        private int EBO;

        public int PointVAO { get; private set; }
        private int PointVBO;

        int segmentCount = 10;
        public int LineCount { get; private set; }
        public Spline2DObject(ISpline2D spline, Vector3 pointColor, float size = PointSize.Medium, PointShape shape = PointShape.Quad)
        {
            this.spline = spline;
            points = new List<NumMath.Vector2>();
            PointSample = new PointSample(pointColor, size, shape);
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
            GL.BufferData(BufferTarget.ElementArrayBuffer, 0, new uint[] { }, BufferUsageHint.StreamDraw);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);


            PointVAO = GL.GenVertexArray();
            PointVBO = GL.GenBuffer();

            GL.BindVertexArray(PointVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, PointVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 0, new float[] { }, BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        }
        public void Update()
        {
            if (InputManager.IsKeyDown(Key.R))
                RemoveLastAddedPoint();

            Rebuild();
        }
        public void AddPoint(OpenTK.Vector2 point)
        {
            NumMath.Vector2 newPoint = new NumMath.Vector2(point.X, point.Y);           
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

            lastAddedPointIndex = i;

            Rebuild();
        }
        public void Rebuild()
        {
            int i;
            int pointsCount = points.Count;
            if (pointsCount > 0)
            {
                float[] buff;

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

                AdditionalPointCount = buff.Length / 2;

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BufferData(BufferTarget.ArrayBuffer, buff.Length * sizeof(float), buff, BufferUsageHint.StreamDraw);

                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                pointsCount = points.Count;
                buff = new float[pointsCount * 2];
                for (i = 0; i < pointsCount; i++)
                {
                    buff[2 * i] = (float)points[i].X;
                    buff[2 * i + 1] = (float)points[i].Y;
                }

                GL.BindVertexArray(PointVAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, PointVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, buff.Length * sizeof(float), buff, BufferUsageHint.StreamDraw);
                GL.BindVertexArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }
        }
        public void RemoveLastAddedPoint()
        {
            if(lastAddedPointIndex > -1)
            {
                points.RemoveAt(lastAddedPointIndex);
                lastAddedPointIndex = -1;

                Rebuild();
            }
        }
    }
}