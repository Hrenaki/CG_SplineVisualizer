using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CG_SplineVisualizer.Objects
{
    public class Grid2DObject
    {
        public Font Font { get; set; }
        public Vector3 Color { get; private set; }
        public List<TextBlock> LabelsX { get; private set; }
        public List<TextBlock> LabelsY { get; private set; }

        public float Dx { get; set; }
        public float Dy { get; set; }

        public int VAO { get; private set; }
        private int VBO;

        public Grid2DObject(Font font, Vector3 color, float width, float height, float dx, float dy)
        {
            Font = font;

            Color = color;

            Dx = dx;
            Dy = dy;

            int pointCount_x = (int)Math.Ceiling(width / Dx);
            int pointCount_y = (int)Math.Ceiling(height / Dy);

            LabelsX = new List<TextBlock>();
            LabelsY = new List<TextBlock>();

            //for (int i = 0; i < pointCount_x; i++)
            //    LabelsX[i] = new TextBlock("", new Vector3(0, 0, 0), font, Color, 0.3f);
            //for (int i = 0; i < pointCount_y; i++)
            //    LabelsY[i] = new TextBlock("", new Vector3(0, 0, 0), font, Color);

            float[] buffer = new float[] { -1, -1, -1, 1, 1, 1, 1, -1 };

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, buffer.Length * sizeof(float), buffer, BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Update(new Vector3(0), width, height);
        }
        public void Update(Vector3 curPosition, float width, float height)
        {
            Vector3 tmp;
            if (LabelsX.Count == 0 || LabelsX[0].Position.X - (curPosition.X - width / 2) >= Dx || (curPosition.X + width / 2) - LabelsX[LabelsX.Count - 1].Position.X >= Dx)
            {
                // updating labels x
                LabelsX.Clear();
                float minX = (float)(Math.Ceiling((curPosition.X - width / 2) / Dx) * Dx);
                float maxX = (float)(Math.Floor((curPosition.X + width / 2) / Dx) * Dx);
            
                int i = 0;
                for (float x = minX; x <= maxX; x = minX + (++i) * Dx)
                {
                    tmp = new Vector3(x, -0.05f, 0);
                    LabelsX.Add(new TextBlock(tmp.X.ToString("E2"), tmp, Font, Color, 0.3f));
                    //LabelsX[i].Position = ;
                    //LabelsX[i].Text = x.ToString("E2");
                }
            }
            
            //if (LabelsY[0].Position.Y - (curPosition.Y - height / 2) >= Dy || (curPosition.Y + height / 2) - LabelsY[LabelsY.Count - 1].Position.Y >= Dy)
            //{
            //    // updating labels y
            //    float minY = (float)(Math.Ceiling((curPosition.Y - height / 2) / Dy) * Dy);
            //    float maxY = (float)(Math.Floor((curPosition.Y + height / 2) / Dy) * Dy);
            //
            //    int i = 0;
            //    for (float y = minY; y <= maxY; y = minY + (++i) * Dy)
            //    {
            //        LabelsY[i].Position = new Vector3(0.0f, y, LabelsY[i].Position.Z);
            //        LabelsY[i].Text = y.ToString("E2");
            //    }
            //}
        }
    }
}