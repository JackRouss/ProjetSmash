using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MenuPrincipal : Microsoft.Xna.Framework.DrawableGameComponent
    {
        SpriteBatch GestionSprites { get; set; }

        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }

        InputManager GestionInputClavier { get; set; }
        InputControllerManager GestionInputManette { get; set; }

        Texture2D Démarrer { get; set; }
        Texture2D Quitter { get; set; }
        Texture2D DémarrerSélectionner { get; set; }
        Texture2D QuitterSélectionner { get; set; }
        ArrièrePlanDéroulant FondÉcran { get; set; }

        bool DémarrerVisible { get; set; }
        bool DémarrerSélectionnerVisible { get; set; }
        bool QuitterSélectionnerVisible { get; set; }
        bool QuitterVisible { get; set; }
        public bool PasserMenuSuivant { get; set; }

        public MenuPrincipal(Game jeu)
            :base(jeu){ }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize()
        {
            Vector2 centre = new Vector2(Game.Window.ClientBounds.Width / 3, Game.Window.ClientBounds.Height / 4);

            FondÉcran = new ArrièrePlanDéroulant(this.Game, "Fond4", Atelier.INTERVALLE_MAJ_STANDARD);
            FondÉcran.Initialize();
            DémarrerVisible = false;
            DémarrerSélectionnerVisible = true;
            QuitterSélectionnerVisible = false;
            QuitterVisible = true;
            PasserMenuSuivant = false;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected  override void LoadContent()
        {
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInputClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionInputManette = Game.Services.GetService(typeof(InputControllerManager)) as InputControllerManager;
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            Démarrer = GestionnaireDeTextures.Find("DémarerPartie2");
            Quitter = GestionnaireDeTextures.Find("QuitterPartie2");
            DémarrerSélectionner = GestionnaireDeTextures.Find("DémarerPartie2Sélectionner");
            QuitterSélectionner = GestionnaireDeTextures.Find("QuitterPartie2Sélectionner");
        }

       


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Game.Exit();
            FondÉcran.Update(gameTime);
            GérerSelectionOption();
            GérerOptionSélectionnner();
            base.Update(gameTime);
        }

        void GérerSelectionOption()
        {

            if (GestionInputClavier.EstClavierActivé || GestionInputManette.EstManetteActivée(PlayerIndex.One))
            {
                if (GestionInputClavier.EstNouvelleTouche(Keys.Down) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickDown))
                {
                    DémarrerSélectionnerVisible = false;
                    DémarrerVisible = true;
                    QuitterSélectionnerVisible = true;
                    QuitterVisible = false;
                }

                if (GestionInputClavier.EstNouvelleTouche(Keys.Up) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickUp))
                {
                    DémarrerSélectionnerVisible = true;
                    DémarrerVisible = false;
                    QuitterSélectionnerVisible = false;
                    QuitterVisible = true;
                }
            
            }
        }

        void GérerOptionSélectionnner()
        {
            if (GestionInputClavier.EstClavierActivé || GestionInputManette.EstManetteActivée(PlayerIndex.One))
            {
                if (GestionInputClavier.EstNouvelleTouche(Keys.Enter) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A))
                {
                    if (DémarrerSélectionnerVisible)
                    {
                        PasserMenuSuivant = true;
                    }
                    if (QuitterSélectionnerVisible)
                    {
                        Game.Exit();
                    }
                }
            }
        }

        

        void GérerVisibilité()
        {
            if (DémarrerVisible)
            {
                GestionSprites.Draw(Démarrer, new Vector2(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 4), Color.White);
            }
            if (DémarrerSélectionnerVisible)
            {
                GestionSprites.Draw(DémarrerSélectionner, new Vector2(Game.Window.ClientBounds.Width / 2, Game.Window.ClientBounds.Height / 4), Color.White);
            }
            if (QuitterVisible)
            {
                GestionSprites.Draw(Quitter, new Vector2(Game.Window.ClientBounds.Width / 2, 3*Game.Window.ClientBounds.Height / 4), Color.White);
            }
            if (QuitterSélectionnerVisible)
            {
                GestionSprites.Draw(QuitterSélectionner, new Vector2(Game.Window.ClientBounds.Width / 2, 3*Game.Window.ClientBounds.Height / 4), Color.White);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            FondÉcran.Draw(gameTime);
            GestionSprites.Begin();
            GérerVisibilité();
            GestionSprites.End();
            base.Draw(gameTime);
        }


    }
}
