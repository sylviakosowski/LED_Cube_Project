using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.Drawing;
using System.IO;

namespace CubeVisualization
{
    public class Game : GameWindow
    {
        /* SHADERS */
        /* Stores the shaders fs.glsl and vs.glsl together in a a
         * usable form (program object). */
        int pgmID;
        int vsID; //stores address of vs.glsl shader
        int fsID; //stores address of fs.glsl shader

        /* SHADER ATTRIBUTES */
        int attribute_vpos; //Pos attribute from shade
        int attribute_vcol; //Color attribute from shader
        int uniform_mview; //View attribute from shader

        /* BUFFERS */
        int vbo_position; //Position buffer
        int vbo_color; //Color buffer
        int vbo_mview; //View buffer
        int ibo_elements; //Cube index buffer

        /* BUFFER DATA - VECTOR ARRAYS */
        Vector3[] vertData;
        Vector3[] colData;

        //Stores everything we're going to draw
        private List<Cube> objects = new List<Cube>();

        /* CUBE */
        private int[] indiceData;

        float time = 0.0f;

        /* Camera stuff */
        Camera cam = new Camera();
        Vector2 lastMousePos = new Vector2();

        private Queue<Vector3[]> imageFrames = new Queue<Vector3[]>();
        
        /* Constructor for Game class which tells it to use multi-sampling */
        public Game() : base(512,512, new GraphicsMode(32, 24, 0, 4))
        {
            /* Create a window 512 x 512 pixels, with 32-bit colro depth,
             * d2 bits in the depth buffer, no stencil buffer, and 4x 
             * sampling for anti-aliasing.
             */
        }

        public Queue<Vector3[]> ImageFrames
        {
            get { return imageFrames; }
            set { imageFrames = value;  }
        }

        /* Changes the colors of each cube in the cube. */
        public void changeCubeColors(Vector3[] colors)
        {
            int i = 0;
            foreach(Cube c in objects)
            {
                c.Color = colors[i];
                i++;
            }
        }

        /* What happens when the window is first loaded. */
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            initProgram();
            Title = "CUb3 V1zu@l1z@t10n br0"; //window title
            GL.ClearColor(Color4.Black); //background color
            GL.PointSize(5f);
        }

        /* What happens when a frame is rendered. */
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Viewport(0, 0, Width, Height);

            //Render the background color
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.Enable(EnableCap.DepthTest);

            //Use the variables we want to draw
            GL.EnableVertexAttribArray(attribute_vpos);
            GL.EnableVertexAttribArray(attribute_vcol);

            //Iterates through the cubes and draws each of them individually.
            int indiceat = 0;
            foreach (Cube c in objects )
            {
                GL.UniformMatrix4(uniform_mview, false, ref c.ModelViewProjectionMatrix);
                GL.DrawElements(BeginMode.Triangles, c.IndiceCount, DrawElementsType.UnsignedInt, indiceat * sizeof(uint));
                indiceat += c.IndiceCount;
            }

            //Do stuff to keep code clean
            GL.DisableVertexAttribArray(attribute_vpos);
            GL.DisableVertexAttribArray(attribute_vcol);
            GL.Flush();

            //Everything must be drawn before we swap buffers
            //Double buffered setup
            SwapBuffers();

            if (imageFrames.Count > 0)
            {
                Vector3[] currentImage = imageFrames.Dequeue();
                changeCubeColors(currentImage);
                Console.WriteLine("Current color: " + currentImage[0].ToString());
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            /* Make cursor move the camera whiel window has focus */
            if (Focused)
            {
                Vector2 delta = lastMousePos - new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);

                cam.AddRotation(delta.X, delta.Y);
                ResetCursor();
            }

            /* Gather up all the data we need to send to graphics card. */
            List<Vector3> verts = new List<Vector3>();
            List<int> inds = new List<int>();
            List<Vector3> colors = new List<Vector3>();

            int vertcount = 0;

            foreach (Cube c in objects)
            {
                verts.AddRange(c.GetVerts().ToList());
                inds.AddRange(c.GetIndices(vertcount).ToList());
                colors.AddRange(c.GetColorData().ToList());
                vertcount += c.VertCount;
            }

            vertData = verts.ToArray();
            indiceData = inds.ToArray();
            colData = colors.ToArray();

            time += (float)e.Time;

            //Tells OpenGL that we'll be using pos buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            //Send data array to graphics card
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, 
                (IntPtr)(vertData.Length * Vector3.SizeInBytes), 
                vertData, BufferUsageHint.StaticDraw);
            //Tell GL to use this buffer for the vPos var
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            //Do same thing for color
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_color);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, 
                (IntPtr)(colData.Length * Vector3.SizeInBytes), 
                colData, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vcol, 3, VertexAttribPointerType.Float, true, 0, 0);

            //Do same thing for cube indices
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo_elements);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 
                (IntPtr)(indiceData.Length * sizeof(int)), 
                indiceData, BufferUsageHint.StaticDraw);

            /* Update cube's model-view-projection matrices. */
            foreach (Cube c in objects)
            {
                c.CalculateModelMatrix();
                c.ViewProjectionMatrix = cam.GetViewMatrix() 
                    * Matrix4.CreatePerspectiveFieldOfView(1.3f, ClientSize.Width / (float)ClientSize.Height, 1.0f, 40.0f);
                c.ModelViewProjectionMatrix = c.ModelMatrix * c.ViewProjectionMatrix;
            }

            //Clear buffer binding and set up to use program with shaders
            GL.UseProgram(pgmID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        /* What happens when the window is resized. */
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //Tells OpenGL where the window is and how we want to draw to it.
            GL.Viewport(ClientRectangle.X, ClientRectangle.Y,
                ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = 
                Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);

            GL.MatrixMode(MatrixMode.Projection);

            GL.LoadMatrix(ref projection);
        }

        /* Returns ID for a new program object. */
        void initProgram()
        {
            pgmID = GL.CreateProgram();

            //Load our shaders from the files
            loadShader("vs.glsl", ShaderType.VertexShader, pgmID, out vsID);
            loadShader("fs.glsl", ShaderType.FragmentShader, pgmID, out fsID);

            //The program needs to be linked (like C, compile then link)
            GL.LinkProgram(pgmID);
            Console.WriteLine(GL.GetProgramInfoLog(pgmID));

            //Get values we need from shaders.
            attribute_vpos = GL.GetAttribLocation(pgmID, "vPosition");
            attribute_vcol = GL.GetAttribLocation(pgmID, "vColor");
            uniform_mview = GL.GetUniformLocation(pgmID, "modelview");

            //Make sure attributes were found.
            if (attribute_vpos == -1 || attribute_vcol == -1 || uniform_mview == -1)
            {
                Console.WriteLine("Error binding attributes");
            }

            //Create buffers
            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out vbo_color);
            GL.GenBuffers(1, out vbo_mview);
            GL.GenBuffers(1, out ibo_elements);

            //Add objects to draw to list
            Random rand = new Random();

            for(int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    for(int z = 0; z < 8; z++)
                    {
                        Cube c = new Cube(new Vector3((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()));
                        c.Position = new Vector3((float)(x - 4), (float)(y - 4), (float)(z - 4));
                        c.Rotation = Vector3.Zero;
                        c.Scale = Vector3.One * (0.3f);
                        objects.Add(c);
                    }
                }
            }
        }

        /* Creates a new shader, using a value from the ShaderType enum, laods code
         * for it, compiles it, and adds it to the program, printing any errors. */
        void loadShader(String filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        /* Camera controls */
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if(e.KeyChar == 'z')
            {
                Exit();
            }

            switch(e.KeyChar)
            {
                case 'w':
                    cam.Move(0f, 0.2f, 0f);
                    break;
                case 'a':
                    cam.Move(-0.2f, 0f, 0f);
                    break;
                case 's':
                    cam.Move(0f, -0.2f, 0f);
                    break;
                case 'd':
                    cam.Move(0.2f, 0f, 0f);
                    break;
                case 'q':
                    cam.Move(0f, 0f, 0.2f);
                    break;
                case 'e':
                    cam.Move(0f, 0f, -0.2f);
                    break;
            }
        }

        /* Center mouse cursor in our window */
        void ResetCursor()
        {
            OpenTK.Input.Mouse.SetPosition(Bounds.Left + Bounds.Width / 2, Bounds.Top + Bounds.Height / 2);
            lastMousePos = new Vector2(OpenTK.Input.Mouse.GetState().X, OpenTK.Input.Mouse.GetState().Y);
        }

        /* Reset cursor when the window is switched to again if lost focus */
        protected override void OnFocusedChanged(EventArgs e)
        {
            base.OnFocusedChanged(e);

            if (Focused)
            {
                ResetCursor();
            }
        }
    }
}
