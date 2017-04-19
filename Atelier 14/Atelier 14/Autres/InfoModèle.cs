using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
   public class InfoModèle
   {
      public Texture2D Texture { get; private set; }
      public bool TextureActive { get; private set; }
      public float PuissanceSpéculaire { get; private set; }
      public Vector3 CouleurAmbiante { get; private set; }
      public Vector4 CouleurDiffuse { get; private set; }
      public Vector3 CouleurEmissive { get; private set; }
      public Vector3 CouleurSpéculaire { get; private set; }
      public Vector4 IntensitéLumièreDiffuse { get; private set; }
      public Vector3 IntensitéLumièreSpéculaire { get; private set; }
      public Effect EffetAffichage { get; private set; }

      public InfoModèle(Effect effetAffichage, Texture2D texture, bool textureActive, Vector3 couleurAmbiante, Vector4 couleurDiffuse, Vector3 couleurEmissive, Vector3 couleurSpéculaire, float puissanceSpéculaire)
      {
         EffetAffichage = effetAffichage;
         Texture = texture;
         TextureActive = textureActive;
         CouleurAmbiante = couleurAmbiante;
         CouleurDiffuse = couleurDiffuse;
         CouleurEmissive = couleurEmissive;
         CouleurSpéculaire = couleurSpéculaire;
         PuissanceSpéculaire = puissanceSpéculaire;
         IntensitéLumièreDiffuse = Vector4.One;
         IntensitéLumièreSpéculaire = Vector3.One;
      }
   }
}
