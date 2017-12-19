using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;
        World m_world_food = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                // m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\microwave"), "microwave.3ds", 
                //      (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                //  m_world_food = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\burger"), "hamburger.3ds", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                m_world = new World(
                       Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\microwave"), "microwave.3ds",
                       Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\burger"), "hamburger.3ds",
                       (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);




            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
         //   m_world_food.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            labela0.Content = m_world.Ambijentalna0.ToString();
            labela1.Content = m_world.Ambijentalna1.ToString();
            labela2.Content = m_world.Ambijentalna2.ToString();
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.Width, (int)openGLControl.Height);
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F4: this.Close(); break;
                case Key.W:
                   // faza 2 : tacka 8
                    if(m_world.RotationX > 15.0f) { 
                        m_world.RotationX -= 5.0f;
                        Console.WriteLine(m_world.RotationX.ToString());
                    }
                    break;
                case Key.S:

                    if (m_world.RotationX < 150.0f)
                    {
                        m_world.RotationX += 5.0f;
                        Console.WriteLine(m_world.RotationX.ToString());
                    }

                    break;
                case Key.A: m_world.RotationY -= 5.0f; break;
                case Key.D: m_world.RotationY += 5.0f; break;
                case Key.Add: m_world.SceneDistance += 5.0f; break;
                case Key.Subtract: m_world.SceneDistance -= 5.0f; break;
                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool) opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName)," " ," ",(int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK );
                        }
                    }
                    break;
            }
        }

        private void umanjiButton_Click(object sender, RoutedEventArgs e)
        {
           
            if(m_world.ScaleX > 0.2f)
            {
                m_world.ScaleX -= 0.2f;
            }
           // Console.WriteLine(m_world.ScaleX.ToString());
        }

        private void uvecajButton_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.ScaleX < 3.0f)
            {
                m_world.ScaleX += 0.2f;
            }
           // Console.WriteLine(m_world.ScaleX.ToString());
        }



        private void addAmbient0_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.Ambijentalna0 < 3.0f)
            {
                m_world.Ambijentalna0 += 0.2f;
                labela0.Content = m_world.Ambijentalna0.ToString();
            }

            if(m_world.Ambijentalna0 < 0.1f)
            {
                labela0.Content = "0.0";
            }
            Console.WriteLine(m_world.Ambijentalna0.ToString());
        }

        private void subAmbient0_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.Ambijentalna0 > 0.1f)
            {
                m_world.Ambijentalna0 -= 0.2f;
                labela0.Content = m_world.Ambijentalna0.ToString();
            }

            if (m_world.Ambijentalna0 < 0.1f)
            {
                labela0.Content = "0.0";
            }
            Console.WriteLine(m_world.Ambijentalna0.ToString());
        }


        private void subAmbient1_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.Ambijentalna1 > 0.1f)
            {
                m_world.Ambijentalna1 -= 0.2f;
                labela1.Content = m_world.Ambijentalna1.ToString();
            }

            if (m_world.Ambijentalna1 < 0.1f)
            {
                labela1.Content = "0.0";
            }
            Console.WriteLine(m_world.Ambijentalna1.ToString());
        }

        private void addAmbient2_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.Ambijentalna2 < 3.0f)
            {
                m_world.Ambijentalna2 += 0.2f;
                labela2.Content = m_world.Ambijentalna2.ToString();
            }
            if (m_world.Ambijentalna2 < 0.1f)
            {
                labela2.Content = "0.0";
            }
            Console.WriteLine(m_world.Ambijentalna2.ToString());
        }

        private void subAmbient2_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.Ambijentalna2 > 0.1f)
            {
                m_world.Ambijentalna2 -= 0.2f;
                labela2.Content = m_world.Ambijentalna2.ToString();
            }
            if (m_world.Ambijentalna2 < 0.1f)
            {
                labela2.Content = "0.0";
            }
            Console.WriteLine(m_world.Ambijentalna2.ToString());
        }

        private void addAmbient1_Click(object sender, RoutedEventArgs e)
        {
            if (m_world.Ambijentalna1 < 3.0f)
            {
                m_world.Ambijentalna1 += 0.2f;
                labela1.Content = m_world.Ambijentalna1.ToString();
            }

            if (m_world.Ambijentalna1 < 0.1f)
            {
                labela1.Content = "0.0";
            }
            Console.WriteLine(m_world.Ambijentalna1.ToString());
        }

        private void uvecajButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (m_world.ScaleX < 3.0f)
            {
                m_world.ScaleX += 0.2f;
            }
        }

        private void uvecajVisinuStolaButton_Click(object sender, RoutedEventArgs e)
        {

            if (m_world.VisinaStola < 8.0f)
            {
                m_world.VisinaStola += 0.5f;
                m_world.VisinaTepiha -= 0.5f;
                m_world.VisinaPostolja -= 0.5f;
                m_world.VisinaTable += 0.5f;
            }
        }

        private void umanjiVisinuStolaButton_Click(object sender, RoutedEventArgs e)
        {

            if (m_world.VisinaStola > 0.5f)
            {
                m_world.VisinaStola -= 0.5f;
                m_world.VisinaTepiha += 0.5f;
                m_world.VisinaPostolja += 0.5f;
                m_world.VisinaTable -= 0.5f;
            }
        }
    }
}
