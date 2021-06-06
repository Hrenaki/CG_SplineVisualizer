using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFont;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;

namespace CG_SplineVisualizer
{
    public class Font : IDisposable
    {
        public int MinLineSpacing { get; private set; }
        public int MaxBearingY { get; private set; }
        public string FamilyName { get; private set; }
        internal TextureAtlas Atlas { get; private set; }
        private Face face;

        public Font(Face face, uint width, uint height)
        {
            this.face = face;
            this.face.SetPixelSizes(width, height);

            FamilyName = face.FamilyName;

            Atlas = new TextureAtlas();
            Atlas.Generate(face);

            MinLineSpacing = 0;
            MaxBearingY = 0;
            Character? tmp;
            for(char c = (char)0; c < (char)255; c++)
            {                
                if ((tmp = Atlas[c]) == null)
                    break;
                if (tmp?.height > MinLineSpacing)
                    MinLineSpacing = (int)tmp?.height;
                if (tmp?.bearingY > MaxBearingY)
                    MaxBearingY = (int)tmp?.bearingY;
            }
        }
        public void Dispose()
        {
            face.Dispose();
        }
    }
    internal class TextureAtlas
    {
        internal readonly int Width = 512;
        internal int Height = 0;
        private int texturePadding = 2;
        internal Texture Tex { get; private set; }
        private Dictionary<char, Character> characters = new Dictionary<char, Character>();
        internal Character? this[char c]
        {
            get
            {
                if(characters.ContainsKey(c))
                {
                    return characters[c];
                }
                return null;
            }
        }

        public void Generate(Face face)
        {
            int curX = 0;
            Height = 0;

            int rowHeight = 0;
            for(byte c = 0; c < 128; c++)
            {
                face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);

                if(curX + face.Glyph.Bitmap.Width + 1 >= Width)
                {
                    curX = 0;
                    Height += rowHeight + texturePadding;
                }
                else rowHeight = Math.Max(rowHeight, face.Glyph.Bitmap.Rows);

                Character character = new Character()
                {
                    x = curX,
                    y = Height,
                    width = face.Glyph.Bitmap.Width,
                    height = face.Glyph.Bitmap.Rows,
                    bearingX = face.Glyph.BitmapLeft,
                    bearingY = face.Glyph.BitmapTop,
                    advance = face.Glyph.Advance.X,
                };
                characters.Add(Convert.ToChar(c), character);

                curX += character.width + texturePadding;
            }
            Height += rowHeight;

            Tex = new Texture(Width, Height);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Tex.TexId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Red, PixelType.UnsignedByte, IntPtr.Zero);

            //face.LoadChar(32, LoadFlags.Render, LoadTarget.Normal);
            //
            //Width = face.Glyph.Bitmap.Width;
            //Height = face.Glyph.Bitmap.Rows;

            //GL.TexSubImage2D(TextureTarget.Texture2D, 0, curCharacter.x, curCharacter.y, curCharacter.width, curCharacter.height, PixelFormat.Red, PixelType.UnsignedByte, curCharacter.buffer);

            Character curCharacter;
            for(byte c = 0; c < 128; c++)
            {
                face.LoadChar(c, LoadFlags.Render, LoadTarget.Normal);
            
                curCharacter = characters[Convert.ToChar(c)];
                GL.TexSubImage2D(TextureTarget.Texture2D, 0, curCharacter.x, curCharacter.y, curCharacter.width, curCharacter.height, PixelFormat.Red, PixelType.UnsignedByte, face.Glyph.Bitmap.Buffer);
            }

            //GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, 1, 1, PixelFormat.Red, PixelType.UnsignedByte, new byte[] { 255 });
            //GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, 1, 1, 0, PixelFormat.Red, PixelType.UnsignedByte, new byte[] { 255 });
            //int w, h;
            //GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureWidth, out w);
            //GL.GetTexLevelParameter(TextureTarget.Texture2D, 0, GetTextureParameter.TextureHeight, out h);
            //
            //
            //IntPtr buffer = Marshal.AllocHGlobal(h * w * 4);
            //GL.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, buffer);
            //byte[] byte_arr = new byte[h * w * 4];
            //Marshal.Copy(buffer, byte_arr, 0, byte_arr.Length);
            //
            //Bitmap bitmap = new Bitmap(w, h);
            //
            //for (int i = 0; i < h; i++)
            //    for (int j = 0; j < w; j++)
            //        bitmap.SetPixel(j, i, Color.FromArgb((int)byte_arr[(i * w + j) * 4 + 3], (int)byte_arr[(i * w + j) * 4 + 0], (int)byte_arr[(i * w + j) * 4 + 1], (int)byte_arr[(i * w + j) * 4 + 2]));
            //
            //bitmap.Save("atlas.png", System.Drawing.Imaging.ImageFormat.Png);

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
    internal struct Character
    {
        public int x;
        public int y;

        public int width;
        public int height;

        public int bearingX;
        public int bearingY;

        public int advance;
    }
}