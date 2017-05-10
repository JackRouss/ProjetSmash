using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class Matériau
   {
      
      public Caméra CaméraJeu { get; private set; }
      public Matrix Monde { get; private set; }
      public Matrix VueProjection { get; private set; }
      public Matrix MondeVueProjection { get; private set; }
      public Matrix MondeTransposéeInverse { get; private set; }
      public Vector3 Position { get; private set; }


      public Matériau(Caméra caméraJeu)
      {
         Monde = Matrix.Identity;
         CaméraJeu = caméraJeu;
      }

      public virtual void UpdateMatériau(Vector3 position, Matrix monde)
      {
         Position = position;
         Monde = monde;
         VueProjection = CaméraJeu.Vue * CaméraJeu.Projection;
         MondeVueProjection = Monde * VueProjection;
         MondeTransposéeInverse = Matrix.Invert(Matrix.Transpose(Monde));
      }
   }
}
