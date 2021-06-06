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
        public TextBlock Zero { get; private set; }

        private float scale = 0.75f;

        private string format = "E2";
        public string Format
        {
            get => format;
            set
            {
                if(format.Equals(value))
                {
                    format = value;
                    Update();
                }
            }
        }

        public float Dx { get; set; }
        public float Dy { get; set; }

        public int VAO { get; private set; }
        private int VBO;

        public Grid2DObject(Font font, Vector3 color)
        {
            Font = font;
            Color = color;

            LabelsX = new List<TextBlock>();
            LabelsY = new List<TextBlock>();

            float[] buffer = new float[] { -1, -1, 1, -1, 1, 1, -1, 1 };

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, buffer.Length * sizeof(float), buffer, BufferUsageHint.StreamDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            Update();
        }
        public void Update()
        {
            ICamera camera = Camera.CurCamera;

            float width = camera.Width;
            float height = camera.Height;
            Vector3 curPosition = camera.Position;

            Matrix4 tmpMatrix = camera.View * camera.Projection;
            tmpMatrix.Transpose();

            Vector3 tmp;
            TextBlock textBlock;
            if (LabelsX.Count == 0 || LabelsX[0].Position.X - (curPosition.X - width) >= Dx || (curPosition.X + width) - LabelsX[LabelsX.Count - 1].Position.X >= Dx)
            {
                // updating labels x

                foreach (TextBlock block in LabelsX)
                    block.ClearBuffers();
                LabelsX.Clear();

                float minX = (float)(Math.Ceiling((curPosition.X - width / 2) / Dx) * Dx);
                float maxX = (float)(Math.Floor((curPosition.X + width / 2) / Dx) * Dx);
            
                int i = 0;
                for (float x = minX; x <= maxX; x = minX + (++i) * Dx)
                {
                    if (x != 0)
                    {
                        textBlock = new TextBlock(x.ToString(format), Vector3.Zero, Font, Color, scale);
                        tmp = (tmpMatrix * new Vector4(x, Math.Min(Math.Max(0, -camera.Height / 2.0f + camera.Position.Y + textBlock.Height * camera.Height / 2.0f), camera.Height / 2.0f + camera.Position.Y), 0, 1.0f)).Xyz;
                        textBlock.Position = tmp;
                        LabelsX.Add(textBlock);
                    }
                }
            }
            
            if (LabelsY.Count == 0 || LabelsY[0].Position.Y - (curPosition.Y - height / 2) >= Dy || (curPosition.Y + height / 2) - LabelsY[LabelsY.Count - 1].Position.Y >= Dy)
            {
                // updating labels y
                foreach (TextBlock block in LabelsY)
                    block.ClearBuffers();
                LabelsY.Clear();

                float minY = (float)(Math.Ceiling((curPosition.Y - height / 2) / Dy) * Dy);
                float maxY = (float)(Math.Floor((curPosition.Y + height / 2) / Dy) * Dy);

                int i = 0;
                for (float y = minY; y <= maxY; y = minY + (++i) * Dy)
                {
                    if (y != 0)
                    {
                        textBlock = new TextBlock(y.ToString(format), Vector3.Zero, Font, Color, scale);
                        tmp = (tmpMatrix * new Vector4(Math.Min(Math.Max(0, -camera.Width / 2.0f + camera.Position.X), camera.Width / 2.0f + camera.Position.X - textBlock.Width * camera.Width / 2.0f), y, 0, 1.0f)).Xyz;
                        textBlock.Position = tmp;
                        LabelsX.Add(textBlock);
                    }
                }
            }

            Zero = new TextBlock(0.ToString(format), (tmpMatrix * new Vector4(Vector3.Zero, 1.0f)).Xyz, Font, Color, scale);
        }
    }
}