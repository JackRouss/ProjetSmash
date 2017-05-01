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


namespace AtelierXNA.Menus
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MenuFinJeu : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const string RÉSUMER_PARTIE = "Résumer la partie";
        const string MENU_PRINCIPAL = "Recommencer";
        const string QUITTER = "Quitter le jeu";
        const string OPTIONS = "Options";
        const float ESPACE_ENTRE_OPTIONS = 40;
        public enum ÉTAT { RECOMMENCER, QUITTER };
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
        Color COuleurTexteChoix { get; set; }



        int CptChoix { get; set; }
        int CptCouleurs { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float IntervalleMAJAnimation { get; set; }
        public bool Recommencer { get; set; }
        float temps { get; set; }
        float TempsAffichageMessageFin { get; set; }
        bool AnimationMessageFinPartie { get; set; }
        bool Fade { get; set; }
        int cptFade { get; set; }
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
            CouleurTexte = Color.White;

            POSITION_RÉSUMER_PARTIE = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(RÉSUMER_PARTIE).X) / 2, ESPACE_ENTRE_OPTIONS);
            POSITION_QUITTER = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(QUITTER).X) / 2,  ArialFont.MeasureString(QUITTER).Y + ESPACE_ENTRE_OPTIONS);

            Message = "       " + Gagnant + "\n" + "est le vainqueur";
            Fade = true;
            AnimationMessageFinPartie = true;
            cptFade = 0;
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            temps += (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            GérerEntrées();
            CalculeAnimation();

            if (AnimationMessageFinPartie)
            {
                if (TempsÉcouléDepuisMAJ >= IntervalleMAJAnimation)
                {
                    cptFade++;
                    ++CptCouleurs;
                    if (CptCouleurs == COULEURS.Length)
                    {
                        CptCouleurs = 0;
                    }
                    TempsÉcouléDepuisMAJ = 0;
                    //CouleurTexte = new Color(new Vector4(CouleurTexte.R, CouleurTexte.G, CouleurTexte.B, CouleurTexte.A - cptFade));
                    //COuleurTexteChoix = new Color(new Vector4(CouleurTexte.R, CouleurTexte.G, CouleurTexte.B, CouleurTexte.A + cptFade));
                    //if (CouleurTexte.A <= 0)
                    //{
                    //    Fade = false;
                    //}
                }
            }
            

            base.Update(gameTime);
        }

        void GérerEntrées()
        {
            if (GestionInputClavier.EstClavierActivé || GestionInputManette.EstManetteActivée(PlayerIndex.One))
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
                    case 0: CHOIX = ÉTAT.RECOMMENCER; break;
                    case 1: CHOIX = ÉTAT.QUITTER; break;
                }

                CptAuBorne();

                if (GestionInputClavier.EstNouvelleTouche(Keys.Enter) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A))
                {
                    if (CHOIX == ÉTAT.RECOMMENCER)
                    {
                        Recommencer = true;
                    }

                    if (CHOIX == ÉTAT.QUITTER)
                    {
                        Game.Exit();
                    }
                }

            }
        }

        void CptAuBorne()
        {
            if(CptChoix > 1)
            { 
                CptChoix = 1;
            }

            if (CptChoix < 0)
            {
                CptChoix = 0;
            }
        }

        void CalculeAnimation()
        {
            AnimationMessageFinPartie = temps >= TempsAffichageMessageFin;
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            if(!AnimationMessageFinPartie)
            {
                GestionSprites.DrawString(ArialFont,Message, new Vector2(Game.Window.ClientBounds.Width/2 - ArialFont.MeasureString(Message).X/2, Game.Window.ClientBounds.Height / 2 - ArialFont.MeasureString(Message).Y/2), CouleurTexte);
            }
            else
            {
                GestionSprites.DrawString(ArialFont, RÉSUMER_PARTIE, POSITION_RÉSUMER_PARTIE, DéterminerCouleur(ÉTAT.RECOMMENCER));
                GestionSprites.DrawString(ArialFont, QUITTER, POSITION_QUITTER, DéterminerCouleur(ÉTAT.QUITTER));
            }
            GestionSprites.End();
            base.Draw(gameTime);
        }
        Color DéterminerCouleur(ÉTAT unÉtat)
        {
            if (unÉtat == CHOIX)
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
