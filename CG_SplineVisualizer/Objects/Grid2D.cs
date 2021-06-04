using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace CG_SplineVisualizer
{
    public class Grid2DObject
    {
        float[] base_x;
        float[] base_y;

        public int VAO { get; private set; }
        private int VBO;
        private int EBO;

        public Grid2DObject(float[] base_x, float[] base_y)
        {
            this.base_x = base_x;
            this.base_y = base_y;
        }

        public void Load()
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);


        }


    }
}
