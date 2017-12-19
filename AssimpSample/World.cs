// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;




namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi
        private enum TextureObjects {Carpet=0,Wood};
        private string[] m_textureFiles = { @"C:\Users\Vladimir\Desktop\AssimpSample\AssimpSample\bin\Debug\images\tepih.jpg",
        @"C:\Users\Vladimir\Desktop\AssimpSample\AssimpSample\bin\Debug\images\wood.jpg"};
        private uint[] m_textures = null;
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private float kamera;

        private float scale_x;

        private float ambijentalna0;
        private float ambijentalna1;
        private float ambijentalna2;

        private float visinaStola;
        private float visinaTepiha;
        private float visinaPostolja;
        private float visinaTable;
        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;
        //   private BitmapFont mf_font = null;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;
        private AssimpScene m_scene2;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = -50.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private Cube cube;
        private Cylinder cylinder;
        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        public AssimpScene Scene2
        {
            get { return m_scene2; }
            set { m_scene2 = value; }
        }



        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        public float Kamera
        {
            get { return kamera; }
            set { m_xRotation = value; }
        }

        public float ScaleX
        {
            get { return scale_x; }
            set { scale_x = value; }
        }

        public float Ambijentalna0
        {
            get { return ambijentalna0; }
            set { ambijentalna0 = value; }
        }

        public float Ambijentalna1
        {
            get { return ambijentalna1; }
            set { ambijentalna1 = value; }
        }

        public float Ambijentalna2
        {
            get { return ambijentalna2; }
            set { ambijentalna2 = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        public float VisinaStola
        {
            get { return visinaStola; }
            set { visinaStola = value; }
        }

        public float VisinaTepiha
        {
            get { return visinaTepiha; }
            set { visinaTepiha = value; }
        }

        public float VisinaPostolja
        {
            get { return visinaPostolja; }
            set { visinaPostolja = value; }
        }

        public float VisinaTable
        {
            get { return visinaTable; }
            set { visinaTable = value; }
        }


        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, String scenePath2, String sceneFileName2,  int width, int height, OpenGL gl)
        {
            cube = new Cube();
            cylinder = new Cylinder();
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_scene2 = new AssimpScene(scenePath2, sceneFileName2, gl);
            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];

            scale_x = 1.0f;
            ambijentalna0 = 0.8f;
            ambijentalna1 = 0.8f;
            ambijentalna2 = 0.0f;

            visinaStola = 3.0f;
            visinaTepiha = -8.5f;
            visinaPostolja = -8.0f;
            visinaTable = -1.5f;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        private void SetupLighting(OpenGL gl)
        {
         
            // bela svetlost
            float[] global_ambient = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);  
            float[] light0pos = new float[] { 0.0f,30.0f,-20.0f, 1.0f }; //pozicioniranje svetlosti na tacku iznad modela
            // podesena ambijentalana, difuzna i spekularna komponenta
            float[] light0ambient = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0diffuse = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] light0specular = new float[] { 1.0f, 1.0f, 1.0f, 1.0f };


            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);  // tackasti izvor svetlosti je se ugao rasipanja onog reflektora
                                                                        // 360 stepeni (2 puta 180, 180 je pola ugla)
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            //setovanje komponenti
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            // ukljuciti svetla
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            


            //zuta svetlost
            float[] global_ambient_yellow = new float[] { 0.0f, 0.0f, 0.0f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient_yellow);
            float[] light0pos_yellow = new float[] { 0.0f, 40.0f, -50.0f, 1.0f };
         
           float[] light0ambient_yellow = new float[] { ambijentalna0, ambijentalna1, ambijentalna2, 1.0f };
            float[] light0diffuse_yellow = new float[] { 0.5f, 0.5f, 0.0f, 1.0f };
            float[] light0specular_yellow = new float[] { 0.9f, 0.9f, 0.0f, 1.0f };

            float[] direction = { 0.0f, -1.0f, 0.0f };
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, direction);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 30.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light0pos_yellow);
            //setovanje komponenti
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, light0ambient_yellow);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, light0diffuse_yellow);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, light0specular_yellow);
            // ukljuciti svetla
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT1);

            gl.Enable(OpenGL.GL_NORMALIZE);



        }

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 1f, 0f);
            kamera = -5;
            
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.FrontFace(OpenGL.GL_CW);
            //faza 2 : tacka 1
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            //faza 2 : tacka 2  + tacka 9
            SetupLighting(gl);


            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL);
            gl.GenTextures(m_textureCount, m_textures);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);      // Linear Filtering

                // faza 2 : tacka 3
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                image.UnlockBits(imageData);
                image.Dispose();
            }


            m_scene.LoadScene();
            m_scene.Initialize();
            m_scene2.LoadScene();
            m_scene2.Initialize();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            // gl.Scale(3.5f, 3.5f, 3.5f);
            
            gl.PushMatrix();
            SetupLighting(gl);
            gl.Scale(scale_x, scale_x, 1);
            // gl.Translate(0.0f, 1.0f, -m_sceneDistance);
            gl.Translate(0.0f, 1.0f, m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            
            //faza 2 : tacka 6
            gl.LookAt(1.0f,0.0f, kamera, 
                      -1.0f, -1.0f, -1.0f,
                      0.0f, 1.0f, 0.0f);
            //postolje
            gl.PushMatrix();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            // gl.Translate(0.0f, 0.0f, 0.0f);
            gl.Rotate(180, 0, 0);
            cylinder.Height = 1;
            cylinder.BaseRadius = 3.0f;
            cylinder.TopRadius = 3.0f;
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(0.2265625f, 0f, 0.0f);
            gl.Rotate(-90f, 0f, 0f);
            cylinder.CreateInContext(gl);
            cylinder.Render(gl, RenderMode.Render);

            //zatvara se postolje
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, 1.0f);
            Disk disk = new Disk();
            gl.Color(0.2265625f, 0f, 0.0f);
            disk.InnerRadius = 0;
            disk.OuterRadius = 3.0f;
            disk.CreateInContext(gl);
            // faza 2 : tacka 5
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Carpet]);
            disk.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            //sto
            gl.PushMatrix();
            gl.Translate(0.0f, visinaTable, 0.0f);
            gl.Scale(3.5f, 0.5f, 3.5f);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(0.30859375f, 0.1484375f, 0.07421875f);
            // faza 2 : tacka 4
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Wood]);
            cube.Render(gl, RenderMode.Render);

            gl.PopMatrix();
            //  gl.PopMatrix();

            gl.PushMatrix();
           // gl.Rotate(180, 0, 0);
            Cube c = new Cube();

            gl.Translate(0.0f, -5.0f, 0.0f);
            // gl.Rotate(-90f, 180f, 0f);
            gl.Scale(0.5f, visinaStola, 0.5f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Wood]);
            c.Render(gl, RenderMode.Render);
            gl.PopMatrix();


            //ono dole, hehe
            gl.PushMatrix();
            Cube s = new Cube();
            gl.Translate(0.0f, visinaPostolja, 0.0f);
            gl.Scale(3.5f, 0.4, 3.5f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Wood]);
            s.Render(gl, RenderMode.Render);        
            gl.PopMatrix();


            //tepih?
            gl.PushMatrix();
            gl.Translate(-5.0f, visinaTepiha, 5.0f);
            gl.Rotate(-90.0f, 0.0f, 0.0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Carpet]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(0.0f, 0.0f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(0.0f, 10.0f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(10.0f, 10.0f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(10.0f,0.0f);
            gl.End();
           // gl.Enable(OpenGL.GL_CULL_FACE);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            m_scene.Draw();
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();

            // crtanje pljeskavice
            gl.PushMatrix();

            gl.Translate(1.0f, 0.0f, 0.5f);
           // gl.Translate(5.0f, 0.0f, 0.0f);
            m_scene2.Draw();
            gl.PopMatrix();


            gl.PopMatrix();
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.Viewport(0, m_width / 2, m_width / 2, m_height / 2);
            gl.PushMatrix();
            gl.Translate(2.0f, -3.5f, 0.0f);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho2D(-15.0f, 15.0f, -12.0f, 12.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            //poruka

            gl.PushMatrix();
            gl.Color(1.0f, 1f, 0.0f);
            gl.Translate(1.5f, -4.0f, 0.0f);
            //     gl.DrawText3D()

            gl.DrawText3D("Times New Roman Bold", 10, 0.0f, 0.0f, "Predmet: Racunarska grafika");
            gl.Translate(-11.5f, -1.0f, 0.0f);
            gl.DrawText3D("Times New Roman Bold", 10, 1f, 1f, "Sk.god: 2017/18.");
            gl.Translate(-6.370f, -1.0f, 0.0f);
            gl.DrawText3D("Times New Roman Bold", 10, 1f, 1f, "Ime: Vladimir");
            gl.Translate(-5.55f, -1.0f, 0.0f);
            gl.DrawText3D("Times New Roman Bold", 10, 1f, 1f, "Prezime: Jovicic");
            gl.Translate(-6.3f, -1.10f, 0.0f);
            gl.DrawText3D("Times New Roman Bold", 10, 1f, 1f, "Sifra zad: 16.1");
            gl.PopMatrix();

            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(45f, (float)m_width/m_height, 1.0f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            // gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();
            gl.Flush();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(45f, (double)width/height, 1.0f, 20000f);
            //  gl.Viewport();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
                m_scene2.Dispose();
                
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
