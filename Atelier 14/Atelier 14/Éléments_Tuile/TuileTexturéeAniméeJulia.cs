using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


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
            GestionnaireDeShaders = Game.Services.GetService(typeof(RessourcesManager<Effect>)) as RessourcesManager<Effect>;

            julia = GestionnaireDeShaders.Find("Julia");



           
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
                zoom /= 1.05f;

            if (pad.Buttons.B == ButtonState.Pressed)
                zoom *= 1.05f;

            float panSensitivity = 0.01f * (float)Math.Log(zoom + 1);

            pan += new Vector2(pad.ThumbSticks.Left.X, -pad.ThumbSticks.Left.Y) * panSensitivity;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = spriteBatch.GraphicsDevice;
            //VertexBuffer VB = new VertexBuffer(;
            //IndexBuffer IB = new IndexBuffer(;
            //GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            float aspectRatio = (float)device.Viewport.Height / (float)device.Viewport.Width;
            julia.CurrentTechnique.Passes[0].Apply();
            julia.Parameters["Pan"].SetValue(pan);
            julia.Parameters["Zoom"].SetValue(zoom);
            julia.Parameters["Aspect"].SetValue(aspectRatio);
            //device.SetVertexBuffer(VB);
            //device.Indices = IB;
            //spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            base.Draw(gameTime);

        }
        protected override void SetUpVertexBuffer()
        {
            VB = new VertexBuffer(spriteBatch.GraphicsDevice, new VertexDeclaration
              (
                  new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                  new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0)), Sommets.Length, BufferUsage.WriteOnly);
            VB.SetData(Sommets);
        }
        protected override void SetVertexBuffer(GraphicsDevice device)
        {
            device.SetVertexBuffer(VB);
        }
    }
}
