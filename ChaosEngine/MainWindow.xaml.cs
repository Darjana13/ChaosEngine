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
        }

        private void GLView_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            ChaosTime.UpdateTime();
            ChaosPhysics.Frame();
            GL.Viewport(0, 0, GLView.Width, GLView.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GLView.SwapBuffers();
        }
    }
}
