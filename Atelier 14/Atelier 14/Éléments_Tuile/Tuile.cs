using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public abstract class Tuile : PrimitiveDeBaseAnimée
   {
      protected const int NB_TRIANGLES = 2;
      protected Vector3[,] PtsSommets { get; private set; }
      protected Vector3 Origine { get; private set; }
      protected Vector2 Delta { get; set; }
      protected BasicEffect EffetDeBase { get; private set; }
        protected SpriteBatch spriteBatch { get; set; }



        public Tuile(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, float intervalleMAJ)
         : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
      {
         Delta = new Vector2(étendue.X, étendue.Y);
         Origine = new Vector3(-Delta.X / 2, -Delta.Y / 2, 0); //pour centrer la primitive au point (0,0,0)
      }

      public override void Initialize()
      {
         NbSommets = NB_TRIANGLES + 2;
         PtsSommets = new Vector3[2, 2];
            spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            CréerTableauSommets();
         CréerTableauPoints();
         base.Initialize();
      }

      protected abstract void CréerTableauSommets();

      protected override void LoadContent()
      {
         EffetDeBase = new BasicEffect(GraphicsDevice);
         InitialiserParamètresEffetDeBase();

            base.LoadContent();
      }

      protected abstract void InitialiserParamètresEffetDeBase();

      protected virtual void CréerTableauPoints()
      {
         PtsSommets[0, 0] = new Vector3(Origine.X, Origine.Y, Origine.Z);
         PtsSommets[1, 0] = new Vector3(Origine.X + Delta.X, Origine.Y, Origine.Z);
         PtsSommets[0, 1] = new Vector3(Origine.X, Origine.Y + Delta.Y, Origine.Z);
         PtsSommets[1, 1] = new Vector3(Origine.X + Delta.X, Origine.Y + Delta.Y, Origine.Z);
      }
        public override void Draw(GameTime gameTime)
        {
            //BlendState oldBlendState = GraphicsDevice.BlendState;
            RasterizerState oldRasterizerState = GraphicsDevice.RasterizerState;
            //GraphicsDevice.BlendState = GestionAlpha;
            RasterizerState ÉtatRasterizer = new RasterizerState();
            ÉtatRasterizer.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = ÉtatRasterizer;
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;
            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                SetVertexBuffer(spriteBatch.GraphicsDevice);
                DessinerTriangleStrip();
            }
            base.Draw(gameTime);
            //GraphicsDevice.BlendState = oldBlendState;
            GraphicsDevice.RasterizerState = oldRasterizerState;
        }
        protected abstract void DessinerTriangleStrip();
        protected abstract void SetVertexBuffer(GraphicsDevice device);
   }
}

