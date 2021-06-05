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
        public string text = "";
        public string Text 
        {
            get => text;
            set
            {
                if (!text.Equals(value))
                {
                    text = value;
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
                if(value != lineSpacing)
                {
                    lineSpacing = value;
                    Update();
                }
            }
        }

        public int VAO { get; private set; }
        private int VBO;

        public TextBlock(string text, Vector3 position, Font font, Vector3 color, int lineSpacing = 0)
        {
            this.text = text;
            curFont = font;

            this.position = position;
            Color = color;
            this.lineSpacing = lineSpacing;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            Update();
        }
        private void Update()
        {
            List<float> buffer = new List<float>();

            float curX = position.X;
            float curY = position.Y;
            float xPos, yPos;
            float w, h;
            float texX, texY;
            for(int i = 0; i < text.Length; i++)
            {
                if(text[i] == '\n' || text[i] == '\r')
                {
                    curX = position.X;
                    curY -= (curFont.MinLineSpacing + lineSpacing) / (float)curFont.Atlas.Height;
                    continue;
                }

                Character character = (Character)CurrentFont.Atlas[text[i]];

                w = character.width / (float)curFont.Atlas.Width;
                h = character.height / (float)curFont.Atlas.Height;

                xPos = curX + character.bearingX / (float)curFont.Atlas.Width;
                yPos = curY - (h - character.bearingY / (float)curFont.Atlas.Height);

                texX = character.x / (float)curFont.Atlas.Width;
                texY = character.y / (float)curFont.Atlas.Height;

                buffer.AddRange(new float[] { xPos, yPos, position.Z, texX, texY + h });
                buffer.AddRange(new float[] { xPos, yPos + h, position.Z, texX, texY });
                buffer.AddRange(new float[] { xPos + w, yPos + h, position.Z, texX + w, texY });
                buffer.AddRange(new float[] { xPos + w, yPos, position.Z, texX + w, texY + h });

                curX += (character.advance >> 6) / (float)curFont.Atlas.Width;
            }

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
    }
}