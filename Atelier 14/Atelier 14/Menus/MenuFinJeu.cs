using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace AtelierXNA.Menus
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MenuFinJeu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const string R…SUMER_PARTIE = "Retour au menu";
        const string MENU_PRINCIPAL = "Recommencer";
        const string QUITTER = "Quitter le jeu";
        const string OPTIONS = "Options";
        const float ESPACE_ENTRE_OPTIONS = 40;
        public enum …TAT { MENUPINCIPAL, RECOMMENCER, QUITTER };
        public …TAT CHOIX;
        Color[] COULEURS = { Color.Firebrick, Color.Red, Color.OrangeRed, Color.Orange, Color.Gold, Color.Yellow, Color.YellowGreen, Color.LawnGreen, Color.Green, Color.DarkTurquoise, Color.DeepSkyBlue, Color.Blue, Color.DarkSlateBlue, Color.Indigo, Color.Purple };

        Vector2 POSITION_R…SUMER_PARTIE { get; set; }
        Vector2 POSITION_MENU_PRINCIPAL { get; set; }
        Vector2 POSITION_QUITTER { get; set; }
        Vector2 POSITION_OPTIONS { get; set; }


        RessourcesManager<SpriteFont> GestionnaireFonts { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInputClavier { get; set; }
        InputControllerManager GestionInputManette { get; set; }
        SpriteFont ArialFont { get; set; }
        Color CouleurTexte1 { get; set; }
        Color CouleurTexte2 { get; set; }




        int CptChoix { get; set; }
        int CptCouleurs { get; set; }
        float Temps…coulÈDepuisMAJ { get; set; }
        float IntervalleMAJAnimation { get; set; }
        public bool Recommencer { get; set; }
        public bool RetournerMenuPrincipale { get; set; }
        public bool PasserMenuPause { get; set; }
        float temps { get; set; }
        float TempsAffichageMessageFin { get; set; }
        bool AnimationMessageFinPartie { get; set; }
        bool Fade { get; set; }
        string Gagnant { get; set; }
        string Message { get; set; }

        public MenuFinJeu(Game game, float intervalleMAJAnimation, float tempsAffichageMessageFin, string gagnant)
            : base(game)
        {
            IntervalleMAJAnimation = intervalleMAJAnimation;
            TempsAffichageMessageFin = tempsAffichageMessageFin;
            Gagnant = gagnant;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GestionnaireFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInputClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionInputManette = Game.Services.GetService(typeof(InputControllerManager)) as InputControllerManager;
            ArialFont = GestionnaireFonts.Find("Arial");
            CouleurTexte1 = Color.Black;
            CouleurTexte2 = new Color(0,0,0,0);
            CptChoix = 0;


            POSITION_R…SUMER_PARTIE = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(R…SUMER_PARTIE).X) / 2, 0);
            POSITION_MENU_PRINCIPAL = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(MENU_PRINCIPAL).X) / 2, ArialFont.MeasureString(MENU_PRINCIPAL).Y + ESPACE_ENTRE_OPTIONS);
            POSITION_OPTIONS = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(OPTIONS).X) / 2, POSITION_MENU_PRINCIPAL.Y + ArialFont.MeasureString(OPTIONS).Y + ESPACE_ENTRE_OPTIONS);
            POSITION_QUITTER = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(QUITTER).X) / 2, POSITION_OPTIONS.Y + ArialFont.MeasureString(QUITTER).Y + ESPACE_ENTRE_OPTIONS);

            Message = "   " + "Player "+ Gagnant + "\n" + "est le vainqueur";
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float temps…coulÈ = (float)gameTime.ElapsedGameTime.TotalSeconds;
            temps = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps…coulÈDepuisMAJ += temps…coulÈ;
            GÈrerEntrÈes();
            CalculeAnimation();
            

           
                if (Temps…coulÈDepuisMAJ >= IntervalleMAJAnimation)
                {
                    CouleurTexte1 = new Color(CouleurTexte1.R, CouleurTexte1.G, CouleurTexte1.B, CouleurTexte1.A - 1);
                if(CouleurTexte1.A == 0)
                {
                    CouleurTexte2 = new Color(CouleurTexte2.R, CouleurTexte2.G, CouleurTexte2.B, CouleurTexte2.A + 3);

                }
                ++CptCouleurs;
                    if (CptCouleurs == COULEURS.Length)
                    {
                        CptCouleurs = 0;
                    }
                    Temps…coulÈDepuisMAJ = 0;
                }
            
           
            

            base.Update(gameTime);
        }

        void GÈrerEntrÈes()
        {
            if (GestionInputClavier.EstClavierActivÈ || GestionInputManette.EstManetteActivÈe(PlayerIndex.One))
            {
                if (GestionInputClavier.EstNouvelleTouche(Keys.Up) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickUp))
                {
                    CptChoix -= 1;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Down) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickDown))
                {
                    CptChoix += 1;
                }

                if (CptChoix < 0)
                {
                    CptChoix = 0;
                }
                if (CptChoix > 2)
                {
                    CptChoix = 2;
                }

                switch (CptChoix)
                {
                    case 0: CHOIX = …TAT.RECOMMENCER; break;
                    case 1: CHOIX = …TAT.MENUPINCIPAL; break;
                    case 2: CHOIX = …TAT.QUITTER; break;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Enter) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A))
                {
                    if (CHOIX == …TAT.RECOMMENCER)
                    {
                        Recommencer = true;
                    }
                    if (CHOIX == …TAT.MENUPINCIPAL)
                    {
                        RetournerMenuPrincipale = true;
                    }

                    if (CHOIX == …TAT.QUITTER)
                    {
                        Game.Exit();
                    }
                }

            }
        }

        void CalculeAnimation()
        {
            AnimationMessageFinPartie = temps >= TempsAffichageMessageFin;
        }

        public override void Draw(GameTime gameTime)
        {
            DrawOrder = 100;
            GestionSprites.Begin();
            if(CouleurTexte1.A != 0)
            {
                GestionSprites.DrawString(ArialFont,Message, new Vector2(Game.Window.ClientBounds.Width/2 - 180, Game.Window.ClientBounds.Height / 2 - 100), CouleurTexte1);
            }
            if(CouleurTexte2.A != 255)
            {
                GestionSprites.DrawString(ArialFont, R…SUMER_PARTIE, POSITION_MENU_PRINCIPAL, CouleurTexte2);
                GestionSprites.DrawString(ArialFont, MENU_PRINCIPAL, POSITION_R…SUMER_PARTIE, CouleurTexte2);
                GestionSprites.DrawString(ArialFont, QUITTER, POSITION_QUITTER, CouleurTexte2);
            }
            else
            {
                GestionSprites.DrawString(ArialFont, MENU_PRINCIPAL, POSITION_R…SUMER_PARTIE, DÈterminerCouleur(…TAT.RECOMMENCER));
                GestionSprites.DrawString(ArialFont, R…SUMER_PARTIE, POSITION_MENU_PRINCIPAL, DÈterminerCouleur(…TAT.MENUPINCIPAL));
                GestionSprites.DrawString(ArialFont, QUITTER, POSITION_QUITTER, DÈterminerCouleur(…TAT.QUITTER));
            }

            


            GestionSprites.End();
            base.Draw(gameTime);
        }
        Color DÈterminerCouleur(…TAT un…tat)
        {
            if (un…tat == CHOIX)
            {
                return COULEURS[CptCouleurs];
            }
            else
            {
                return CouleurTexte2;
            }
        }
    }
}
