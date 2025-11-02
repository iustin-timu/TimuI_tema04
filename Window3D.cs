using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

namespace ConsoleApp3
{
    class Window3D : GameWindow
    {
        // --- Declarații ---
        private KeyboardState previousKeyboard;
        private MouseState previousMouse;
        private Randomizer rando;
        private Axes axes;
        private Grid grid;
        private Camera3DIsometric cam;
        
        private List<Objectoid> rainOgObjects;
        private bool GRAVITY = true;


        private bool axesVisible = true;
        private bool gridVisible = true;
        private bool objectVisible = true;

        // Stocăm valorile inițiale ale camerei
        private Vector3 initialEye;
        private Vector3 initialTarget;
        private Vector3 initialUp;

        // --- Constante ---
        private Color DEFAULT_BACK_COLOR = Color.Black;


        // --- Constructor ---
        public Window3D() : base(1280, 768, new GraphicsMode(32, 24, 0, 8))
        {
            VSync = VSyncMode.On;

            // Inițializare obiecte
            rando = new Randomizer();
            axes = new Axes();
            grid = new Grid();
            
            rainOgObjects = new List<Objectoid>();

            // --- MODIFICARE: Poziția "pe Axa Roșie, mai sus" ---

            // Poziția VECHE (pe axa Albastră, jos):
            // initialEye = new Vector3(5, 5, 30); 
            // initialTarget = new Vector3(5, 0, 0); 

            // Poziția NOUĂ (pe axa Roșie, mai sus):
            initialEye = new Vector3(30, 15, 5); // X=30 (Axa Roșie), Y=15 (Mai sus), Z=5
            initialTarget = new Vector3(0, 0, 0); // Ne uităm spre origine

            initialUp = new Vector3(0, 1, 0); // "Sus" este tot în sus pe Y

            // Inițializare cameră
            cam = new Camera3DIsometric(initialEye, initialTarget, initialUp);
        }

        // --- OnLoad ---
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            // Reactivăm netezirea (Antialiasing) pentru un aspect 3D mai fin
            GL.Enable(EnableCap.PolygonSmooth);
            GL.Hint(HintTarget.PolygonSmoothHint, HintMode.Nicest);
            GL.Disable(EnableCap.LineSmooth);

            // Setăm culoarea de fundal
            GL.ClearColor(DEFAULT_BACK_COLOR);
            previousMouse = Mouse.GetState();

            // Afișăm meniul în consolă la pornire
            DisplayHelp();
        }

        // --- OnResize ---
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, this.Width, this.Height);

            // Folosim proiecția Perspectivă
            SetProjection(this.Width, this.Height);

            cam.SetCamera();
        }

        // --- SetProjection (Perspectivă 3D) ---
        private void SetProjection(int width, int height)
        {
            float fovy = MathHelper.PiOver4;
            float aspect = (float)width / (float)height;
            float near = 1.0f;
            float far = 1000.0f;

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(fovy, aspect, near, far);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref perspective);
        }

        // --- OnUpdateFrame ---
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            KeyboardState currentKeyboard = Keyboard.GetState();

            // Logică taste
            if (currentKeyboard[Key.Escape]) Exit();
            if (currentKeyboard[Key.H] && !previousKeyboard[Key.H]) DisplayHelp();

            // R. Resetare la valorile inițiale
            if (currentKeyboard[Key.R] && !previousKeyboard[Key.R])
            {
                GL.ClearColor(DEFAULT_BACK_COLOR);
                cam.Reset(initialEye, initialTarget, initialUp);
                SetProjection(this.Width, this.Height);
            }

            // T. Schimbare vizibilitate grid
            if (currentKeyboard[Key.T] && !previousKeyboard[Key.T])
            {
                gridVisible = !gridVisible;
            }

            // M. Schimbare vizibilitate sisteme de axe
            if (currentKeyboard[Key.M] && !previousKeyboard[Key.M])
            {
                axesVisible = !axesVisible;
            }

            // O. Schimbare vizibilitate obiect
            if (currentKeyboard[Key.O] && !previousKeyboard[Key.O])
            {
                objectVisible = !objectVisible;
            }

            MouseState currentMouse = Mouse.GetState();

            // --- Deplasare Cameră (W/S este acum ZOOM) ---
            if (currentKeyboard[Key.W]) cam.MoveForward(); // Apropiere
            if (currentKeyboard[Key.S]) cam.MoveBackward(); // Depărtare
            if (currentKeyboard[Key.A]) cam.MoveLeft();
            if (currentKeyboard[Key.D]) cam.MoveRight();
            if (currentKeyboard[Key.Q]) cam.MoveUp();
            if (currentKeyboard[Key.E]) cam.MoveDown();
            if(currentKeyboard[Key.G] && !previousKeyboard[Key.G])
            {
                GRAVITY = !GRAVITY;
            }

            if (currentMouse[MouseButton.Left] && !previousMouse[MouseButton.Left])
            {
                rainOgObjects.Clear();
            }

            // MODIFICAT: Plouă cu obiecte la click dreapta
            if (currentMouse[MouseButton.Right] && !previousMouse[MouseButton.Right])
            {
                rainOgObjects.Add(new Objectoid(GRAVITY));
            }

            // K. Schimbare culoare de fundal
            if (currentKeyboard[Key.K] && !previousKeyboard[Key.K])
            {
                GL.ClearColor(rando.RandomColor());
            }

            // V. Toggle rapid grid/axe
            if (currentKeyboard[Key.V] && !previousKeyboard[Key.V])
            {
                axesVisible = !axesVisible;
                gridVisible = !gridVisible;
            }

            // Object falling logic 
            foreach (Objectoid obj in rainOgObjects)
            {
                obj.UpdatePosition(GRAVITY);
            }

            // Stocăm starea tastaturii
            previousKeyboard = currentKeyboard;
            previousMouse = currentMouse;
        }

        // --- OnRenderFrame ---
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Setăm camera LA FIECARE FRAME
            cam.SetCamera();

            // Desenare obiecte
            if (gridVisible)
            {
                grid.Draw();
            }

            if (axesVisible)
            {
                axes.Draw();
            }

            foreach (Objectoid obj in rainOgObjects)
            {
                if (objectVisible)
                {
                    obj.Draw();
                }
            }

            SwapBuffers();
        }

        // --- DisplayHelp (Actualizat) ---
        private void DisplayHelp()
        {
            Console.WriteLine("\n      MENU (MOD 3D PERSPECTIVĂ)");
            Console.WriteLine(" ESC - parasire aplicatie");
            Console.WriteLine(" K - schimbare culoare de fundal");
            Console.WriteLine(" R - reseteaza vederea la valorile implicite");
            Console.WriteLine(" M - schimbare vizibilitate sisteme de axe");
            Console.WriteLine(" T - schimbare vizibilitate grid");
            Console.WriteLine(" O - schimbare vizibilitate obiect");
            Console.WriteLine(" V - toggle vizibilitate grid/axe (rapid)");
            Console.WriteLine(" G - toggle gravitație obiecte");
            Console.WriteLine("------------------------------------------");
            Console.WriteLine(" Deplasare camera (Perspectivă)");
            Console.WriteLine(" W/S - Apropiere / Depărtare (Zoom)");
            Console.WriteLine(" A/D - Stanga/Dreapta (Pan)");
            Console.WriteLine(" Q/E - Sus/Jos (Pan)");
            Console.WriteLine("Mouse Dreapta - spown cuburi");
            Console.WriteLine("Mouse Stânga - curăță ecranul de cuburi)");


        }
    }
}