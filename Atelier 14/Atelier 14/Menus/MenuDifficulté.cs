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

namespace AtelierXNA
{
    public class MenuDifficulté : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const string DIFFICULTÉ = "Difficulté de l'I.A. :";
        const string FACILE = "Facile";
        const string NORMAL = "Normal";
        const string DIFFICILE = "Difficile";
        const float ESPACE_ENTRE_OPTIONS = 40;
        public enum ÉTAT {FACILE, NORMAL, DIFFICILE }
        public ÉTAT CHOIX;

        Vector2 POSITION_FACILE { get; set; }
        Vector2 POSITION_NORMAL { get; set; }
        Vector2 POSITION_DIFFICILE { get; set; }
        Vector2 POSITION_DIFFICULTÉ { get; set; }

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
        public bool PasserMenuSuivant { get; set; }

        public MenuDifficulté(Game jeu, float intervalleMAJAnimation)
            :base(jeu)
        {
            IntervalleMAJAnimation = intervalleMAJAnimation;
            CHOIX = ÉTAT.FACILE;
        }

        public override void Initialize()
        {
            GestionnaireDeFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInputClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionInputMannette = Game.Services.GetService(typeof(InputControllerManager)) as InputControllerManager;
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            ArialFont = GestionnaireDeFonts.Find("Arial");


            ArrièrePlan = new ArrièrePlanDéroulant(this.Game,"Fond4",Atelier.INTERVALLE_MAJ_STANDARD);
            ArrièrePlan.Initialize();
            CouleurTexte = Color.White;


            POSITION_DIFFICULTÉ = new Vector2((Game.Window.ClientBounds.Width-ArialFont.MeasureString(DIFFICULTÉ).X)/2,0);
            POSITION_FACILE = new Vector2((Game.Window.ClientBounds.Width-ArialFont.MeasureString(FACILE).X)/2,ArialFont.MeasureString(FACILE).Y + ESPACE_ENTRE_OPTIONS);
            POSITION_NORMAL = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(NORMAL).X) / 2, POSITION_FACILE.Y + ArialFont.MeasureString(NORMAL).Y + ESPACE_ENTRE_OPTIONS);
            POSITION_DIFFICILE = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(DIFFICILE).X) / 2, POSITION_NORMAL.Y + ArialFont.MeasureString(DIFFICILE).Y + ESPACE_ENTRE_OPTIONS);



            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;

            GérerEntrées();
            if(TempsÉcouléDepuisMAJ >= IntervalleMAJAnimation)
            {
                ++CptCouleur;
                if(CptCouleur == COULEURS.Length)
                {
                    CptCouleur = 0;
                }
                TempsÉcouléDepuisMAJ = 0;
            }
            ArrièrePlan.Update(gameTime);
        }

        private void GérerEntrées()
        {
            if(GestionInputClavier.EstClavierActivé)
            {
                if (GestionInputClavier.EstNouvelleTouche(Keys.Up))
                {
                    if (CHOIX == ÉTAT.NORMAL)
                    {
                        CHOIX = ÉTAT.FACILE;
                    }
                    else if (CHOIX == ÉTAT.DIFFICILE)
                    {
                        CHOIX = ÉTAT.NORMAL;
                    }
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Down))
                {
                    if (CHOIX == ÉTAT.FACILE)
                    {
                        CHOIX = ÉTAT.NORMAL;
                    }
                    else if (CHOIX == ÉTAT.NORMAL)
                    {
                        CHOIX = ÉTAT.DIFFICILE;
                    }
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Enter))
                {
                    PasserMenuSuivant = true;
                }
            }
        }


        public override void Draw(GameTime gameTime)
        {
            ArrièrePlan.Draw(gameTime);
            GestionSprites.Begin();

            if(CHOIX == ÉTAT.FACILE)
            {
                GestionSprites.DrawString(ArialFont, FACILE, POSITION_FACILE, COULEURS[CptCouleur]);
                GestionSprites.DrawString(ArialFont, NORMAL, POSITION_NORMAL, CouleurTexte);
                GestionSprites.DrawString(ArialFont, DIFFICILE, POSITION_DIFFICILE, CouleurTexte);
            }
            if(CHOIX == ÉTAT.NORMAL)
            {
                GestionSprites.DrawString(ArialFont, FACILE, POSITION_FACILE, CouleurTexte);
                GestionSprites.DrawString(ArialFont, NORMAL, POSITION_NORMAL, COULEURS[CptCouleur]);
                GestionSprites.DrawString(ArialFont, DIFFICILE, POSITION_DIFFICILE, CouleurTexte);
            }
            if(CHOIX == ÉTAT.DIFFICILE)
            {
                GestionSprites.DrawString(ArialFont, FACILE, POSITION_FACILE, CouleurTexte);
                GestionSprites.DrawString(ArialFont, NORMAL, POSITION_NORMAL, CouleurTexte);
                GestionSprites.DrawString(ArialFont, DIFFICILE, POSITION_DIFFICILE, COULEURS[CptCouleur]);
            }

            GestionSprites.DrawString(ArialFont, DIFFICULTÉ, POSITION_DIFFICULTÉ, CouleurTexte);
            GestionSprites.End();
            base.Draw(gameTime);
        }
    }
}
