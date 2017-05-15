using System;

namespace AtelierXNA.Autres
{
    public class Générateur
    {
        Random g { get; set; }
        public Générateur() { g = new Random(); }
        public void ModifierSeed(int seed) { g = new Random(seed); }
        public void ResetSeed() { g = new Random(); }

        public int GénérerEntierAléatoire(int min, int max)
        {
            return g.Next(min, max);
        }

        public float GénérerFloatAléatoire(float min, float max)
        {
            int nbDécimales;
            if ((min == 0 && max == 0))
                return float.NaN;
            else if(min == 0)
                nbDécimales = GénérerEntierAléatoire(0, (int)(Math.Log10(Math.Abs(max))));
            else if(max == 0)
                nbDécimales = GénérerEntierAléatoire((int)(Math.Log10(Math.Abs(min))), 0);
            else
                nbDécimales = GénérerEntierAléatoire((int)(Math.Log10(Math.Abs(min))), (int)(Math.Log10(Math.Abs(max))));
            return (float)(Math.Pow(10,nbDécimales)*g.NextDouble());
        }

        public float NextFloat()
        {
            float aléatoire = (float)g.NextDouble();
            return aléatoire;
        }
    }
}
