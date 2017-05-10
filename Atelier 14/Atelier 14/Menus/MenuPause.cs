using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA.Menus
{
    public class MenuPause : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const string RÉSUMER_PARTIE = "Résumer la partie";
        const string MENU_PRINCIPAL = "Recommencer";
        const string QUITTER = "Quitter le jeu";
        const string OPTIONS = "Options";
        const float ESPACE_ENTRE_OPTIONS = 40;
        public enum ÉTAT { RÉSUMER_PARTIE,MENU_PRINCIPAL, OPTIONS, QUITTER};
        public ÉTAT CHOIX;
        Color[] COULEURS = { Color.Firebrick, Color.Red, Color.OrangeRed, Color.Orange, Color.Gold, Color.Yellow, Color.YellowGreen, Color.LawnGreen, Color.Green, Color.DarkTurquoise, Color.DeepSkyBlue, Color.Blue, Color.DarkSlateBlue, Color.Indigo, Color.Purple };

        Vector2 POSITION_RÉSUMER_PARTIE { get; set; }
        Vector2 POSITION_MENU_PRINCIPAL { get; set; }
        Vector2 POSITION_QUITTER { get; set; }
        Vector2 POSITION_OPTIONS { get; set; }

       
        RessourcesManager<SpriteFont> GestionnaireFonts { get; set; }
        SpriteBatch GestionSprites { get; set; }
        InputManager GestionInputClavier { get; set; }
        InputControllerManager GestionInputManette { get; set; }
        SpriteFont ArialFont { get; set; }
        Color CouleurTexte { get; set; }



        int CptChoix { get; set; }
        int CptCouleurs { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float IntervalleMAJAnimation { get; set; }
        public bool RésumerLaPartie { get; set; }
        public bool RetournerMenuPrincipale { get; set; }
        public bool PasserMenuPause { get; set; }



        public MenuPause(Game jeu, float intervalleMAJAnimation)
            :base(jeu)
        {
            CHOIX = ÉTAT.RÉSUMER_PARTIE;
            IntervalleMAJAnimation = intervalleMAJAnimation;
        }

        public override void Initialize()
        {
            DrawOrder = 6;
            GestionnaireFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInputClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionInputManette = Game.Services.GetService(typeof(InputControllerManager)) as InputControllerManager;
            ArialFont = GestionnaireFonts.Find("Arial");
            CouleurTexte = Color.White;

            POSITION_RÉSUMER_PARTIE = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(RÉSUMER_PARTIE).X) / 2, 0);
            POSITION_MENU_PRINCIPAL = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(MENU_PRINCIPAL).X) / 2, ArialFont.MeasureString(MENU_PRINCIPAL).Y + ESPACE_ENTRE_OPTIONS);
            POSITION_OPTIONS = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(OPTIONS).X) / 2, POSITION_MENU_PRINCIPAL.Y + ArialFont.MeasureString(OPTIONS).Y + ESPACE_ENTRE_OPTIONS);
            POSITION_QUITTER = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(QUITTER).X) / 2, POSITION_OPTIONS.Y + ArialFont.MeasureString(QUITTER).Y + ESPACE_ENTRE_OPTIONS);

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;

            GérerEntrées();
            if(TempsÉcouléDepuisMAJ >= IntervalleMAJAnimation)
            {
                ++CptCouleurs;
                if(CptCouleurs == COULEURS.Length)
                {
                    CptCouleurs = 0;
                }
                TempsÉcouléDepuisMAJ = 0;
            }

            base.Update(gameTime);
        }

        void GérerEntrées()
        {
            if(GestionInputClavier.EstClavierActivé || GestionInputManette.EstManetteActivée(PlayerIndex.One))
            {
                if (GestionInputClavier.EstNouvelleTouche(Keys.Up) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickUp))
                {
                    CptChoix -= 1;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Down) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickDown))
                {
                    CptChoix += 1;
                }
                switch (CptChoix)
                {
                    case 0: CHOIX = ÉTAT.RÉSUMER_PARTIE; break;
                    case 1: CHOIX = ÉTAT.MENU_PRINCIPAL; break;
                    case 2: CHOIX = ÉTAT.OPTIONS; break;
                    case 3: CHOIX = ÉTAT.QUITTER;break;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Enter) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A))
                {
                    if(CHOIX == ÉTAT.RÉSUMER_PARTIE)
                    {
                        RésumerLaPartie = true;
                    }

                    if(CHOIX == ÉTAT.MENU_PRINCIPAL)
                    {
                        RetournerMenuPrincipale = true;
                    }

                    if(CHOIX == ÉTAT.OPTIONS)
                    {
                        PasserMenuPause = true;
                    }

                    if(CHOIX == ÉTAT.QUITTER)
                    {
                        Game.Exit();
                    }
                }
                
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
           
            GestionSprites.DrawString(ArialFont, RÉSUMER_PARTIE, POSITION_RÉSUMER_PARTIE, DéterminerCouleur(ÉTAT.RÉSUMER_PARTIE));
            GestionSprites.DrawString(ArialFont, MENU_PRINCIPAL, POSITION_MENU_PRINCIPAL, DéterminerCouleur(ÉTAT.MENU_PRINCIPAL));
            GestionSprites.DrawString(ArialFont, OPTIONS, POSITION_OPTIONS, DéterminerCouleur(ÉTAT.OPTIONS));
            GestionSprites.DrawString(ArialFont, QUITTER, POSITION_QUITTER, DéterminerCouleur(ÉTAT.QUITTER));

            GestionSprites.End();
            
            base.Draw(gameTime);
        }

        Color DéterminerCouleur(ÉTAT unÉtat)
        {
            if(unÉtat == CHOIX)
            {
                return COULEURS[CptCouleurs];
            }
            else
            {
                return CouleurTexte;
            }
        }

    }

       
}
