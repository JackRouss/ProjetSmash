using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AtelierXNA
{
   public class MatériauÉclairé:Matériau
   {
      public Lumière LumièreJeu { get; private set; }
      public Vector3 CouleurLumièreAmbiante { get; private set; }
      public Vector4 CouleurLumièreDiffuse { get; private set; }
      public Vector3 CouleurLumièreEmissive { get; private set; }
      public Vector3 CouleurLumièreSpéculaire { get; private set; }
      public float PuissanceSpéculaire { get; private set; }
      public float CarréDistanceLumière { get; set; }
      public Texture2D BumpMap { get; private set; }



      public MatériauÉclairé(Caméra caméraJeu, Lumière lumièreJeu, Vector3 couleurAmbiante, Vector4 couleurDiffuse, 
                             Vector3 couleurEmissive,Vector3 couleurSpéculaire, float puissanceSpéculaire)
         :base(caméraJeu)
      {
         LumièreJeu = lumièreJeu;
         CouleurLumièreAmbiante = couleurAmbiante;
         CouleurLumièreDiffuse = couleurDiffuse;
         CouleurLumièreEmissive = couleurEmissive;
         CouleurLumièreSpéculaire = couleurSpéculaire;
         PuissanceSpéculaire = puissanceSpéculaire;
      }

      public override void UpdateMatériau(Vector3 position, Matrix monde)
      {
         base.UpdateMatériau(position, monde);
         CarréDistanceLumière = (float)Math.Pow(Vector3.Distance(LumièreJeu.Position, Position), 2);
      }

   }
}
