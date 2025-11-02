using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    /// <summary>
    /// Reprezintă un singur obiect 3D care poate fi desenat în scenă.
    /// Fiecare "Objectoid" are propria sa culoare, poziție de pornire aleatorie
    /// și este afectat de gravitație.
    /// </summary>
    class Objectoid
    {
        // --- Câmpuri (Variabile Membru) ---

        // Controlează dacă obiectul este desenat sau nu (folosit de tasta 'O').
        private bool visibility;

        // Determină dacă obiectul este afectat de gravitație.
        private bool isGravityBound;

        // Stochează culoarea unică a acestui obiect.
        private Color Colour;

        // Lista de puncte (vertice) care definesc forma 3D a obiectului.
        private List<Vector3> coordList;

        // O referință la clasa ajutătoare pentru a genera valori aleatorii.
        private Randomizer rando;

        // Viteza de cădere a obiectului (1 unitate pe cadru).
        // Notă: O valoare 'int' de 1 este foarte rapidă. Pentru o cădere mai lentă, ai putea folosi 'float GRAVITY_OFFSET = 0.1f'.
        private const int GRAVITY_OFFSET = 1;

        /// <summary>
        /// Constructorul clasei Objectoid.
        /// Este apelat de fiecare dată când se creează un nou obiect (ex: la click dreapta).
        /// Acesta stabilește o poziție, mărime și culoare aleatorie.
        /// </summary>
        /// <param name="gravity_status">Starea inițială a gravitației (trimisă din Window3D).</param>
        public Objectoid(bool gravity_status)
        {
            // Inițializează generatorul de numere aleatorii.
            rando = new Randomizer();

            visibility = true; // Obiectul este vizibil de la început.
            isGravityBound = gravity_status; // Setează starea gravitației primită de la Window3D.
            Colour = rando.RandomColor(); // Alege o culoare aleatorie pentru acest obiect.

            // Inițializează lista care va ține vârfurile obiectului.
            coordList = new List<Vector3>();

            // --- Generează o formă și poziție unică ---

            // Stabilește o mărime aleatorie (între 3 și 7).
            int size_offset = rando.RandomInt(3, 7);

            // Stabilește o înălțime de pornire aleatorie (între 40 și 60).
            int height_offset = rando.RandomInt(40, 60);

            // Stabilește o poziție X/Z aleatorie (cât de departe de centru).
            int radial_offset = rando.RandomInt(5, 15);

            // --- Creează vârfurile pentru formă ---
            // Aceste 10 puncte sunt folosite de GL.QuadStrip pentru a desena fețele obiectului.
            // Punctele definesc o formă de "L" tridimensional (sau cub incomplet) la poziția generată aleatoriu.
            coordList.Add(new Vector3(0 * size_offset + radial_offset, 0 * size_offset + height_offset, 1 * size_offset + radial_offset));
            coordList.Add(new Vector3(0 * size_offset + radial_offset, 0 * size_offset + height_offset, 0 * size_offset + radial_offset));
            coordList.Add(new Vector3(1 * size_offset + radial_offset, 0 * size_offset + height_offset, 1 * size_offset + radial_offset));
            coordList.Add(new Vector3(1 * size_offset + radial_offset, 0 * size_offset + height_offset, 0 * size_offset + radial_offset));
            coordList.Add(new Vector3(1 * size_offset + radial_offset, 1 * size_offset + height_offset, 1 * size_offset + radial_offset));
            coordList.Add(new Vector3(1 * size_offset + radial_offset, 1 * size_offset + height_offset, 0 * size_offset + radial_offset));
            coordList.Add(new Vector3(0 * size_offset + radial_offset, 1 * size_offset + height_offset, 1 * size_offset + radial_offset));
            coordList.Add(new Vector3(0 * size_offset + radial_offset, 1 * size_offset + height_offset, 0 * size_offset + radial_offset));
            // Ultimele 2 puncte sunt duplicate pentru a închide corect QuadStrip-ul (leagă ultima față de prima).
            coordList.Add(new Vector3(0 * size_offset + radial_offset, 0 * size_offset + height_offset, 1 * size_offset + radial_offset));
            coordList.Add(new Vector3(0 * size_offset + radial_offset, 0 * size_offset + height_offset, 0 * size_offset + radial_offset));

        }

        /// <summary>
        /// Desenează obiectul pe ecran.
        /// Această metodă este apelată în OnRenderFrame din Window3D pentru fiecare obiect din listă.
        /// </summary>
        public void Draw()
        {
            // Setează culoarea de desenare la culoarea unică a acestui obiect.
            GL.Color3(Colour);

            // Începe modul de desenare QuadStrip (o serie de patrulatere conectate).
            GL.Begin(PrimitiveType.QuadStrip);

            // Trimite fiecare vârf (Vertex) din listă către placa grafică.
            foreach (Vector3 v in coordList)
            {
                GL.Vertex3(v);
            }

            // Termină desenarea.
            GL.End();
        }

        /// <summary>
        /// Actualizează starea obiectului (ex: aplică gravitația).
        /// Această metodă este apelată în OnUpdateFrame din Window3D pentru fiecare obiect.
        /// </summary>
        /// <param name="gravity_status">Starea globală a gravitației (trimisă din Window3D).</param>
        public void UpdatePosition(bool gravity_status)
        {
            // Verifică dacă obiectul este vizibil, dacă gravitația e activă global (tasta G)
            // ȘI dacă obiectul NU a atins încă pământul.
            if (visibility && gravity_status && !GroundCollisionDetected())
            {
                // Dacă toate condițiile sunt adevărate, mută fiecare vârf al obiectului în jos.
                for (int i = 0; i < coordList.Count; i++)
                {
                    // Creează un nou Vector3 păstrând X și Z, dar scăzând Y-ul cu viteza gravitației.
                    coordList[i] = new Vector3(coordList[i].X, coordList[i].Y - GRAVITY_OFFSET, coordList[i].Z);
                }
            }
        }

        /// <summary>
        /// O funcție ajutătoare care verifică dacă vreun vârf al obiectului a atins pământul (Y <= 0).
        /// </summary>
        /// <returns>True dacă a atins pământul, altfel False.</returns>
        public bool GroundCollisionDetected()
        {
            foreach (Vector3 v in coordList)
            {
                // Dacă orice parte (vârf) a obiectului este la sau sub nivelul 0...
                if (v.Y <= 0)
                {
                    // ...atunci raportează coliziunea (returnează "adevărat").
                    return true;
                }
            }

            // Dacă s-a terminat bucla și niciun vârf nu a atins pământul, returnează "fals".
            return false;
        }

        /// <summary>
        /// Inversează starea de vizibilitate a obiectului (folosit de tasta 'O').
        /// </summary>
        public void ToggleVisibility()
        {
            // Setează vizibilitatea la opusul a ceea ce este acum (true devine false, false devine true).
            visibility = !visibility;
        }
    }
}