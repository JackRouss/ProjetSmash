using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class TuileTexturée : Tuile
   {
      //const int NB_TRIANGLES = 2;
      RessourcesManager<Texture2D> gestionnaireDeTextures;
      Texture2D textureTuile;
        //private Game game;

       public VertexPositionTexture[] Sommets { get; set; }
      Vector2[,] PtsTexture { get; set; }
      string NomTextureTuile { get; set; }
      BlendState GestionAlpha { get; set; }

      public TuileTexturée(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, string nomTextureTuile, float intervalleMAJ)
         : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, intervalleMAJ)
      {
         NomTextureTuile = nomTextureTuile;
      }
        protected override void CréerTableauSommets()
      {
         PtsTexture = new Vector2[2, 2];
         CréerTableauPointsTexture();
         Sommets = new VertexPositionTexture[NbSommets];
      }

      private void CréerTableauPointsTexture()
      {
         PtsTexture[0, 0] = new Vector2(0, 1);
         PtsTexture[1, 0] = new Vector2(1, 1);
         PtsTexture[0, 1] = new Vector2(0, 0);
         PtsTexture[1, 1] = new Vector2(1, 0);
      }

      protected override void InitialiserSommets() // Est appelée par base.Initialize()
      {
         int NoSommet = -1;
         for (int j = 0; j < 1; ++j)
         {
            for (int i = 0; i < 2; ++i)
            {
               Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]);
               Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
            }
         }
            SetUpVertexBuffer();
      }

      protected override void LoadContent()
      {

         gestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
         textureTuile = gestionnaireDeTextures.Find(NomTextureTuile);
         base.LoadContent();
      }

      protected override void InitialiserParamètresEffetDeBase()
      {
         EffetDeBase.TextureEnabled = true;
         EffetDeBase.Texture = textureTuile;
         GestionAlpha = BlendState.AlphaBlend;
      }
      protected override void DessinerTriangleStrip()
      {
         GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, 0, NB_TRIANGLES);
      }
        public virtual void  Mirroir()
        {
            Vector3 buffer = new Vector3(PtsSommets[0, 0].X, PtsSommets[0, 0].Y, PtsSommets[0, 0].Z);
            PtsSommets[0, 0] = new Vector3(PtsSommets[1, 0].X, PtsSommets[1, 0].Y, PtsSommets[1, 0].Z);
            PtsSommets[1, 0] = buffer;
            buffer = new Vector3(PtsSommets[0, 1].X, PtsSommets[0, 1].Y, PtsSommets[0, 1].Z);
            PtsSommets[0, 1] = new Vector3(PtsSommets[1, 1].X, PtsSommets[1, 1].Y, PtsSommets[1, 1].Z);
            PtsSommets[1, 1] = buffer;
            InitialiserSommets();
        }
        protected virtual void SetUpVertexBuffer()
        {

        }
        
       
        protected override void SetVertexBuffer(GraphicsDevice device)
        {

        }

    }
}
