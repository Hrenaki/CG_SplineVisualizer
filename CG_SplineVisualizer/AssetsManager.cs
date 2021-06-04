using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpFont;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CG_SplineVisualizer
{
    public static class AssetsManager
    {
        private static Library library = new Library();
        public static Dictionary<string, Font> Fonts = new Dictionary<string, Font>();

        public static void LoadFontFrom(string file, uint height, uint width = 0, string fontName = "")
        {
            if (!File.Exists(file))
                throw new FileNotFoundException($"File '{file}' wasn't found!");

            Face face = new Face(library, file);
            Font font = new Font(face, width, height);
            Fonts.Add(fontName.Equals(string.Empty) ? "Default" : fontName, font);
        }
    }
    public class Texture
    {
        public int TexId { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Texture(int width, int height)
        {
            Width = width;
            Height = height;
            TexId = GL.GenTexture();
        }
    }
}