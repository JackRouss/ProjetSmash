using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA.Éléments_Tuile
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class TuileTexturéeAniméeJulia :TuileTexturée
    {
        Effect julia;
        RessourcesManager<Effect> GestionnaireDeShaders { get; set; }
        VertexBuffer VB;
        CaméraDePoursuite CaméraTuile { get; set; }
        InputManager GestionClavier { get; set; }

        Vector2 pan = new Vector2(0.25f, 0);
        float zoom = 3;

        public TuileTexturéeAniméeJulia(Game game, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, string nomTextureTuile, float intervalleMAJ)
            : base(game, homothétieInitiale, rotationInitiale,positionInitiale, étendue, nomTextureTuile, intervalleMAJ)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }
        protected override void LoadContent()
        {
            GestionClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionnaireDeShaders = Game.Services.GetService(typeof(RessourcesManager<Effect>)) as RessourcesManager<Effect>;

            julia = GestionnaireDeShaders.Find("Julia");
            CaméraTuile = Game.Components.First(t => t is CaméraDePoursuite) as CaméraDePoursuite;



            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            GamePadState pad = GamePad.GetState(PlayerIndex.One);

            if (pad.Buttons.A == ButtonState.Pressed)
                zoom /= 1.0005f;

            if (pad.Buttons.X == ButtonState.Pressed)
                zoom *= 1.0005f;

            //if(GestionClavier.EstNouvelleTouche(Keys.Q))
            //    zoom /= 1.05f;
            //if(GestionClavier.EstNouvelleTouche(Keys.E))
            //    zoom *= 1.05f;



            float panSensitivity = 0.001f * (float)Math.Log(zoom + 1);

            pan += new Vector2(pad.ThumbSticks.Left.X, -pad.ThumbSticks.Left.Y) * panSensitivity;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
         Matrix projection = Matrix.CreateOrthographicOffCenter
            (0,GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
         Matrix halfPixelOffset = Matrix.CreateTranslation(-0.5f, -0.5f, 0);
         //julia.Parameters["MatrixTransform"].SetValue(halfPixelOffset * projection);
         julia.Parameters["MatrixTransform"].SetValue(GetMonde() * CaméraTuile.Vue * CaméraTuile.Projection);
         //julia.Parameters["Texture"].SetValue(textureTuile);

         GraphicsDevice device = spriteBatch.GraphicsDevice;
            //VertexBuffer VB = new VertexBuffer(;
            //IndexBuffer IB = new IndexBuffer(;
            //GraphicsDevice.SetRenderTarget(null);
            //GraphicsDevice.Clear(Color.CornflowerBlue);
            float aspectRatio = (float)device.Viewport.Height / (float)device.Viewport.Width;
            julia.CurrentTechnique.Passes[0].Apply();
            julia.Parameters["Pan"].SetValue(pan);
            julia.Parameters["Zoom"].SetValue(zoom);
            julia.Parameters["Aspect"].SetValue(aspectRatio);
         //device.SetVertexBuffer(VB);
         //device.Indices = IB;
         //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
         //base.Draw(gameTime);
            foreach (EffectPass passeEffet in julia.CurrentTechnique.Passes)
            {
               passeEffet.Apply();
               GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, 0, 2);
            }


      }
      protected override void SetUpVertexBuffer()
        {
            VB = new VertexBuffer(spriteBatch.GraphicsDevice, new VertexDeclaration
              (
                  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(sizeof(float) * 5, VertexElementFormat.Color, VertexElementUsage.Color, 0)), Sommets.Length, BufferUsage.WriteOnly);
            VB.SetData(Sommets);
        }
        protected override void SetVertexBuffer(GraphicsDevice device)
        {
            device.SetVertexBuffer(VB);
        }
    }
}
