using System;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class Lumière
   {
      Vector3 position;
      public Vector3 Position
      {
         get { return position; }
         set
         {
            if (value != null)
            {
               position = value;
            }
            else
            {
               throw new ArgumentNullException("Position");
            }
         }
      }

      float rayon;
      public float Rayon
      {
         get { return rayon; }
         set
         {
            if (value >= 0)
            {
               rayon = value;
            }
            else
            {
               throw new ArgumentException("Le rayon doit être supérieure ou égale à 0");
            }
         }
      }

      Vector3 couleur;
      public Vector3 Couleur
      {
         get { return couleur; }
         set
         {
            if (value != null)
            {
               couleur = value;
            }
            else
            {
               throw new ArgumentNullException("Couleur");
            }
         }
      }

      float intensité;
      public float Intensité
      {
         get { return intensité; }
         set
         {
            if (value >= 0)
            {
               intensité = value;
            }
            else
            {
               throw new ArgumentException("Intensité doit être supérieure ou égale à 0");
            }
         }
      }

      Vector3 intensitéSpéculaire;
      public Vector3 IntensitéSpéculaire
      {
         get { return intensitéSpéculaire; }
         set
         {
            if (value != null)
            {
               intensitéSpéculaire = value;
            }
            else
            {
               throw new ArgumentNullException("IntensitéSpéculaire");
            }
         }
      }

      Vector4 intensitéDiffuse;
      public Vector4 IntensitéDiffuse
      {
         get { return intensitéDiffuse; }
         set
         {
            if (value != null)
            {
               intensitéDiffuse = value;
            }
            else
            {
               throw new ArgumentNullException("IntensitéDiffuse");
            }
         }
      }

      public Lumière(Game jeu, Vector3 position, Vector3 couleur, float rayon, float intensité, Vector3 intensitéSpéculaire, Vector4 intensitéDiffuse)
      {
         Position = position;
         Couleur = couleur;
         Rayon = rayon;
         Intensité = intensité;
         IntensitéSpéculaire = Vector3.Normalize(intensitéSpéculaire * intensité);
         IntensitéDiffuse = Vector4.Normalize(intensitéDiffuse * intensité);
      }
   }
}
