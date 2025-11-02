using System;
using System.Drawing;

namespace ConsoleApp3
{
    class Randomizer
    {
        private Random r;
        private int LOW_INT_VAL = -25;
        private int HIGH_INT_VAL = 25;

        public Randomizer()
        {
            r = new Random();
        }

        /// <summary>
        /// This method returns an random color when requested.
        /// </summary>
        /// <returns>The color, randomly generated!</returns>
        public Color RandomColor()
        {
            int genR = r.Next(0, 255);
            int genG = r.Next(0, 255);
            int genB = r.Next(0, 255);

            Color col = Color.FromArgb(genR, genG, genB);

            return col;
        }

        public int RandomInt()
        {
            int i = r.Next(LOW_INT_VAL, HIGH_INT_VAL);

            return i;
        }

        public int RandomInt(int minVal, int maxVal)
        {
            int i = r.Next(minVal,maxVal);

            return i;
        }

        public int RandomInt(int maxval)
        {
            int i = r.Next(maxval);

            return i;
        }
    }
}
