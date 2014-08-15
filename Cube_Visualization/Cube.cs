using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Cube_Visualization
{
    /* 
     * Class to represent a cube in OpenGL. Used to create virtual LEDs
     * (each LED is a solid color cube in my visualization. They are
     * arranged in 3D space to form a cube of cubes.)
     */
    public class Cube
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        /* Number of vertices, indices, and color data. */
        public int VertCount;
        public int IndiceCount;
        public int ColorDataCount;

        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;

        private Vector3 color = new Vector3(1, 1, 1);

        public Cube(Vector3 color)
        {
            VertCount = 8;
            IndiceCount = 36;
            ColorDataCount = 8;
            this.color = color;
        }

        public Vector3 Color
        {
            get{ return color; }
            set { color = value; }
        }

        /* Returns a vertex array for the cube. */
        public Vector3[] GetVerts()
        {
            return new Vector3[] {
                new Vector3(-0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, 0.5f,  0.5f),
                new Vector3(-0.5f, 0.5f,  0.5f),
            };
        }

        public int[] GetIndices(int offset = 0)
        {
            /* Create initial index array */
            int[] inds = new int[] {
                //left
                0, 2, 1,
                0, 3, 2,
                //back
                1, 2, 6,
                6, 5, 1,
                //right
                4, 5, 6,
                6, 7, 4,
                //top
                2, 3, 6,
                6, 3, 7,
                //front
                0, 7, 3,
                0, 4, 7,
                //bottom
                0, 1, 5,
                0, 5, 4
            };

            if (offset != 0)
            {
                for(int i = 0; i < inds.Length; i++)
                {
                    inds[i] += offset;
                }
            }

            return inds;
        }

        /* Returns an array of color data */
        public Vector3[] GetColorData()
        {
            return new Vector3[] {
                Color,
                Color, 
                Color,
                Color,
                Color, 
                Color, 
                Color, 
                Color
            };
        }

        /* Create model matrix */
        public void CalculateModelMatrix()
        {
            ModelMatrix = Matrix4.Scale(Scale) * Matrix4.CreateRotationX(Rotation.X)
                * Matrix4.CreateRotationY(Rotation.Y) * Matrix4.CreateRotationZ(Rotation.Z)
                * Matrix4.CreateTranslation(Position);
        }


    }
}
