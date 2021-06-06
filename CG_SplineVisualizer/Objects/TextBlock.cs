using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CG_SplineVisualizer.Objects
{
    public class TextBlock
    {
        public float Width { get; private set; }
        public float Height { get; private set; }

        private string text = "";
        public string Text
        {
            get => text;
            set
            {
                if (!text.Equals(value))
                {
                    text = value;
                    Update();
                }
            }
        }

        private Vector3 position;
        public Vector3 Position
        {
            get => position;
            set
            {
                if (!value.Equals(position))
                {
                    position = new Vector3(value);
                    Update();
                }
            }
        }

        public Vector3 Color { get; set; }

        private Font curFont;
        public Font CurrentFont
        {
            get => curFont;
            set
            {
                if (!value.FamilyName.Equals(curFont.FamilyName))
                {
                    curFont = value;
                    Update();
                }
            }
        }

        private int lineSpacing;
        public int LineSpacing
        {
            get => lineSpacing;
            set
            {
                if (value != lineSpacing)
                {
                    lineSpacing = value;
                    Update();
                }
            }
        }

        private float scale;
        public float Scale
        {
            get => scale;
            set
            {
                if(value != scale)
                {
                    scale = value;
                    Update();
                }
            }           
        }

        public int VAO { get; private set; }
        private int VBO;

        public TextBlock(string text, Vector3 position, Font font, Vector3 color, float scale = 1.0f, int lineSpacing = 0)
        {
            this.text = text;
            curFont = font;

            this.Scale = scale;

            this.position = position;
            Color = color;
            this.lineSpacing = lineSpacing;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            Update();
        }
        public void Update()
        {
            List<float> buffer = new List<float>();

            Width = 0;
            Height = 0;

            float curScaleY = 1.0f / Window.CurWindow.Height * Scale;
            float curScaleX = 1.0f / Window.CurWindow.Width * Scale;

            float bearingY = curFont.MaxBearingY * curScaleY;
            float curLineSpacing = (curFont.MinLineSpacing + lineSpacing) * curScaleY;

            float originX = position.X;
            float originY = position.Y - bearingY;
            float xPos, yPos;
            float rectW, rectH;
            float texW, texH;
            float texX, texY;
            for(int i = 0; i < text.Length; i++)
            {
                if(text[i] == '\n' || text[i] == '\r')
                {
                    Width = Math.Max(originX - position.X, Width);

                    originX = position.X;
                    originY -= curLineSpacing;
                    continue;
                }

                Character character = (Character)CurrentFont.Atlas[text[i]];

                rectW = character.width * curScaleX;
                rectH = character.height * curScaleY;

                texW = character.width / (float)curFont.Atlas.Width;
                texH = character.height / (float)curFont.Atlas.Height;

                xPos = originX + character.bearingX * curScaleX;
                yPos = originY - rectH + character.bearingY * curScaleY;

                texX = character.x / (float)curFont.Atlas.Width;
                texY = character.y / (float)curFont.Atlas.Height;

                buffer.AddRange(new float[] { xPos, yPos, position.Z, texX, texY + texH });
                buffer.AddRange(new float[] { xPos, yPos + rectH, position.Z, texX, texY });
                buffer.AddRange(new float[] { xPos + rectW, yPos + rectH, position.Z, texX + texW, texY });
                buffer.AddRange(new float[] { xPos + rectW, yPos, position.Z, texX + texW, texY + texH });

                originX += (character.advance >> 6) * curScaleX;
            }
            Width = Math.Max(originX - position.X, Width);
            Height = Math.Abs(originY - position.Y) + curFont.MinLineSpacing * curScaleY - bearingY;

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, buffer.Count * sizeof(float), buffer.ToArray(), BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        public void ClearBuffers()
        {
            GL.DeleteVertexArray(VAO);
            GL.DeleteBuffer(VBO);
        }
    }
}