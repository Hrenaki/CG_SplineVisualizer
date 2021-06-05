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
        public static Dictionary<string, Font> Fonts { get; private set; }
        public static Dictionary<string, Shader> Shaders { get; private set; }
        internal static Dictionary<string, ShaderType> ShaderExtensions { get; private set; }
        public static void Init()
        {
            Fonts = new Dictionary<string, Font>();
            Shaders = new Dictionary<string, Shader>();
            ShaderExtensions = new Dictionary<string, ShaderType>()
            {
                { ".vsh", ShaderType.VertexShader },
                { ".fsh", ShaderType.FragmentShader },
                { ".gsh", ShaderType.GeometryShader }
            };
        }
        public static void LoadFontFrom(string file, uint height, uint width = 0, string fontName = "")
        {
            if (!File.Exists(file))
                throw new FileNotFoundException($"File '{file}' wasn't found!");

            Face face = new Face(library, file);
            Font font = new Font(face, width, height);
            Fonts.Add(fontName.Equals(string.Empty) ? "Default" : fontName, font);
        }
        public static Shader LoadShader(string name, params ShaderComponent[] components)
        {
            int program = GL.CreateProgram();
            foreach (ShaderComponent component in components)
                GL.AttachShader(program, component.Id);
            GL.LinkProgram(program);

            string infoLog;
            if ((infoLog = GL.GetProgramInfoLog(program)) != string.Empty)
                throw new Exception("Link error in " + name + " : " + infoLog);

            foreach (ShaderComponent component in components)
            {
                GL.DetachShader(program, component.Id);
                GL.DeleteShader(component.Id);
            }

            Shader shader = new Shader(program);
            Shaders[name] = shader;
            return shader;
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
    public class Shader : IDisposable
    {
        public int Handle;
        private bool disposedValue = false;
        public Dictionary<string, int> Locations { get; private set; }
        public Shader(int id)
        {
            Handle = id;
            Locations = new Dictionary<string, int>();

            int uniformCount;
            string uniformName;
            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out uniformCount);
            for (int i = 0; i < uniformCount; i++)
            {
                uniformName = GL.GetActiveUniform(Handle, i, out _, out _);
                if (uniformName.EndsWith("[0]"))
                    uniformName = uniformName.Substring(0, uniformName.Length - 3);
                Locations.Add(uniformName, GL.GetUniformLocation(Handle, uniformName));
            }
        }
        public void Use()
        {
            GL.UseProgram(Handle);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                GL.DeleteProgram(Handle);
                disposedValue = true;
            }
        }
        ~Shader()
        {
            GL.DeleteProgram(Handle);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
    public class ShaderComponent
    {
        public int Id { get; private set; }
        public ShaderType Type { get; private set; }
        public ShaderComponent(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException($"File '{filename}' doesn't exist!");

            string extension = Path.GetExtension(filename);
            if (AssetsManager.ShaderExtensions.ContainsKey(extension))
                Type = AssetsManager.ShaderExtensions[extension];
            else throw new Exception($"Can't find shader type matches '{extension}' extension!");

            Id = GL.CreateShader(Type);
            GL.ShaderSource(Id, File.ReadAllText(filename));
            GL.CompileShader(Id);
            string infoLog;
            if ((infoLog = GL.GetShaderInfoLog(Id)) != string.Empty)
                throw new Exception(filename + " : " + infoLog);
        }
    }
}