using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace ChaosEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dictionary<string, int> shaders = new Dictionary<string, int>();
        private int VAO;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowsFormsHost_Initialized(object sender, EventArgs e)
        {
            GLView.MakeCurrent();
        }

        private void GLView_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.FromArgb(100, 100, 100));
            ChaosTime.Initialize();
            Logger.Initialize();
            //shaders["default"] = ChaosUtils.LoadShaderProgram("Resources\\defaultShader_vertex.glsl", "Resources\\defaultShader_fragment.glsl");
            VAO = loadTriangle();
        }

        private int loadTriangle()
        {
            float[] vertices = { -0.5f, -0.5f, 0f, 0.5f, -0.5f, 0f, 0f, 0.5f, 0f };

            int VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            int VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);

            return VAO;
        }

        private void GLView_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            ChaosTime.UpdateTime();
            ChaosPhysics.Frame();
            GL.Viewport(0, 0, GLView.Width, GLView.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Clear(ClearBufferMask.DepthBufferBit);
            //GL.UseProgram(shaders["default"]);

            GL.BindVertexArray(VAO);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            GLView.SwapBuffers();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GL.Viewport(0, 0, GLView.Width, GLView.Height);
        }
    }
}
