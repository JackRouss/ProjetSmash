using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA.Menus
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MenuPvP : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const string MESSAGE = "Mode de jeu";
        const string PvP = "Joueur contre Joueur";
        const string PvBot = "Joueur contre AI";
        const float ESPACE_ENTRE_OPTIONS = 40;
        public enum ÉTAT { PvP , PvBot }
        public ÉTAT CHOIX;
        public bool multiplayer { get; set; }

        Vector2 POSITION_PvP { get; set; }
        Vector2 POSITION_PvBot { get; set; }
        Vector2 POSITION_MESSAGE { get; set; }

        int CptChoix { get; set; }
        int CptCouleur { get; set; }
        Color[] COULEURS = { Color.Firebrick, Color.Red, Color.OrangeRed, Color.Orange, Color.Gold, Color.Yellow, Color.YellowGreen, Color.LawnGreen, Color.Green, Color.DarkTurquoise, Color.DeepSkyBlue, Color.Blue, Color.DarkSlateBlue, Color.Indigo, Color.Purple };

        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInputClavier { get; set; }
        InputControllerManager GestionInputMannette { get; set; }

        #region Éléments qui vont s'afficher.
        SpriteFont ArialFont { get; set; }
        ArrièrePlanDéroulant ArrièrePlan { get; set; }
        Color CouleurTexte { get; set; }
        #endregion

        float IntervalleMAJAnimation { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        public bool PvPActiver { get; set; }
        public bool PvBotActiver { get; set; }
        public bool PasserMenuSuivant { get; set; }
        public MenuPvP(Game game, float intervalleMAJAnimation)
            : base(game)
        {
            IntervalleMAJAnimation = intervalleMAJAnimation;

        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GestionnaireDeFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInputClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionInputMannette = Game.Services.GetService(typeof(InputControllerManager)) as InputControllerManager;
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            ArialFont = GestionnaireDeFonts.Find("Arial");


            ArrièrePlan = new ArrièrePlanDéroulant(this.Game, "Fond4", Atelier.INTERVALLE_MAJ_STANDARD);
            ArrièrePlan.Initialize();
            CouleurTexte = Color.White;
            CptChoix = 0;


            POSITION_MESSAGE = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(MESSAGE).X) / 2, 0);
            POSITION_PvP = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(PvP).X) / 2, ArialFont.MeasureString(PvP).Y + ESPACE_ENTRE_OPTIONS);
            POSITION_PvBot = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(PvBot).X) / 2, POSITION_PvP.Y + ArialFont.MeasureString(PvBot).Y + ESPACE_ENTRE_OPTIONS);

            PvPActiver = false;
            PvBotActiver = false;
            PasserMenuSuivant = false;
            multiplayer = false;

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;

            GérerEntrées();
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJAnimation)
            {
                ++CptCouleur;
                if (CptCouleur == COULEURS.Length)
                {
                    CptCouleur = 0;
                }
                TempsÉcouléDepuisMAJ = 0;
            }
            ArrièrePlan.Update(gameTime);
        
            base.Update(gameTime);
        }

        private void GérerEntrées()
        {
            if (GestionInputClavier.EstClavierActivé || GestionInputMannette.EstManetteActivée(PlayerIndex.One))
            {

                if (GestionInputClavier.EstNouvelleTouche(Keys.Up) || GestionInputMannette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickUp))
                {
                    CptChoix -= 1;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Down) || GestionInputMannette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickDown))
                {
                    CptChoix += 1;
                }
                if (!multiplayer)
                {
                    CptChoix = 1;
                }
                else
                {
                    maxCpt();
                }
                switch (CptChoix)
                {
                    case 0: CHOIX = ÉTAT.PvP; break;
                    case 1: CHOIX = ÉTAT.PvBot; break;
                }


                if ((GestionInputClavier.EstNouvelleTouche(Keys.Enter) || GestionInputMannette.EstNouvelleTouche(PlayerIndex.One, Buttons.A)) && ÉTAT.PvP == CHOIX)
                {
                    PvPActiver = true;
                    PasserMenuSuivant = true;
                }
                if ((GestionInputClavier.EstNouvelleTouche(Keys.Enter) || GestionInputMannette.EstNouvelleTouche(PlayerIndex.One, Buttons.A)) && ÉTAT.PvBot == CHOIX)
                {
                    PvBotActiver = true;
                    PasserMenuSuivant = true;
                }
            }
        }

        void maxCpt()
        {
            CptChoix = CptChoix > 1 ? 1 : CptChoix;
            CptChoix = CptChoix < 0 ? 0 : CptChoix;

        }

        public override void Draw(GameTime gameTime)
        {
            ArrièrePlan.Draw(gameTime);
            GestionSprites.Begin();
            if (multiplayer)
            {
                GestionSprites.DrawString(ArialFont, PvP, POSITION_PvP, DéterminerCouleur(ÉTAT.PvP));
                GestionSprites.DrawString(ArialFont, PvBot, POSITION_PvBot, DéterminerCouleur(ÉTAT.PvBot));
            }else
            {
                GestionSprites.DrawString(ArialFont, PvP, POSITION_PvP, Color.DarkGray);
                GestionSprites.DrawString(ArialFont, PvBot, POSITION_PvBot, DéterminerCouleur(ÉTAT.PvBot));
            }

           

            GestionSprites.DrawString(ArialFont, MESSAGE, POSITION_MESSAGE, CouleurTexte);
            GestionSprites.End();
            base.Draw(gameTime);
        }
        Color DéterminerCouleur(ÉTAT unÉtat)
        {
            if (unÉtat == CHOIX)
            {
                return COULEURS[CptCouleur];
            }
            else
            {
                return CouleurTexte;
            }
        }
    }
}
