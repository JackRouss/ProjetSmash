﻿using System;
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
    public class MenuPause : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const string RÉSUMER_PARTIE = "Résumer la partie";
        const string MENU_PRINCIPAL = "Retour au menu principal";
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
        ArrièrePlanDéroulant ArrièrePlan { get; set; }
        SpriteFont ArialFont { get; set; }
        Color CouleurTexte { get; set; }




        int CptCouleurs { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float IntervalleMAJAnimation { get; set; }
        public bool RésumerLaPartie { get; set; }



        public MenuPause(Game jeu, float intervalleMAJAnimation)
            :base(jeu)
        {
            CHOIX = ÉTAT.RÉSUMER_PARTIE;
            IntervalleMAJAnimation = intervalleMAJAnimation;
        }

        public override void Initialize()
        {
            GestionnaireFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInputClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionInputManette = Game.Services.GetService(typeof(InputControllerManager)) as InputControllerManager;
            ArialFont = GestionnaireFonts.Find("Arial");
            ArrièrePlan = new ArrièrePlanDéroulant(Game,"Fond4",Atelier.INTERVALLE_MAJ_STANDARD);
            ArrièrePlan.Initialize();
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
            }
            ArrièrePlan.Update(gameTime);
            base.Update(gameTime);
        }

        void GérerEntrées()
        {
            if(GestionInputClavier.EstClavierActivé)
            {
                if(GestionInputClavier.EstNouvelleTouche(Keys.Up))
                {
                    if(CHOIX == ÉTAT.MENU_PRINCIPAL)
                    {
                        CHOIX = ÉTAT.RÉSUMER_PARTIE;
                    }
                    else if(CHOIX == ÉTAT.OPTIONS)
                    {
                        CHOIX = ÉTAT.MENU_PRINCIPAL;
                    }
                    else if(CHOIX == ÉTAT.QUITTER)
                    {
                        CHOIX = ÉTAT.OPTIONS;
                    }
                }
                if(GestionInputClavier.EstNouvelleTouche(Keys.Down))
                {
                    if (CHOIX == ÉTAT.RÉSUMER_PARTIE)
                    {
                        CHOIX = ÉTAT.MENU_PRINCIPAL;
                    }
                    else if (CHOIX == ÉTAT.MENU_PRINCIPAL)
                    {
                        CHOIX = ÉTAT.OPTIONS;
                    }
                    else if (CHOIX == ÉTAT.OPTIONS)
                    {
                        CHOIX = ÉTAT.QUITTER;
                    }
                }
                if(GestionInputClavier.EstNouvelleTouche(Keys.Enter) && CHOIX == ÉTAT.RÉSUMER_PARTIE)
                {
                    RésumerLaPartie = true;
                }
                else if(GestionInputClavier.EstNouvelleTouche(Keys.Enter)&&CHOIX == ÉTAT.QUITTER)
                {
                    Game.Exit();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ArrièrePlan.Draw(gameTime);
            GestionSprites.Begin();
            if(CHOIX == ÉTAT.RÉSUMER_PARTIE)
            {
                GestionSprites.DrawString(ArialFont,RÉSUMER_PARTIE,POSITION_RÉSUMER_PARTIE,COULEURS[CptCouleurs]);
                GestionSprites.DrawString(ArialFont,MENU_PRINCIPAL,POSITION_MENU_PRINCIPAL,CouleurTexte);
                GestionSprites.DrawString(ArialFont,OPTIONS,POSITION_OPTIONS, CouleurTexte);
                GestionSprites.DrawString(ArialFont,QUITTER,POSITION_QUITTER, CouleurTexte);
            }
            else if(CHOIX == ÉTAT.MENU_PRINCIPAL)
            {
                GestionSprites.DrawString(ArialFont, RÉSUMER_PARTIE, POSITION_RÉSUMER_PARTIE, CouleurTexte);
                GestionSprites.DrawString(ArialFont, MENU_PRINCIPAL, POSITION_MENU_PRINCIPAL, COULEURS[CptCouleurs]);
                GestionSprites.DrawString(ArialFont, OPTIONS, POSITION_OPTIONS, CouleurTexte);
                GestionSprites.DrawString(ArialFont, QUITTER, POSITION_QUITTER, CouleurTexte);
            }
            else if(CHOIX == ÉTAT.OPTIONS)
            {
                GestionSprites.DrawString(ArialFont, RÉSUMER_PARTIE, POSITION_RÉSUMER_PARTIE, CouleurTexte);
                GestionSprites.DrawString(ArialFont, MENU_PRINCIPAL, POSITION_MENU_PRINCIPAL, CouleurTexte);
                GestionSprites.DrawString(ArialFont, OPTIONS, POSITION_OPTIONS, COULEURS[CptCouleurs]);
                GestionSprites.DrawString(ArialFont, QUITTER, POSITION_QUITTER, CouleurTexte);
            }
            else if(CHOIX == ÉTAT.QUITTER)
            {
                GestionSprites.DrawString(ArialFont, RÉSUMER_PARTIE, POSITION_RÉSUMER_PARTIE, CouleurTexte);
                GestionSprites.DrawString(ArialFont, MENU_PRINCIPAL, POSITION_MENU_PRINCIPAL, CouleurTexte);
                GestionSprites.DrawString(ArialFont, OPTIONS, POSITION_OPTIONS, CouleurTexte);
                GestionSprites.DrawString(ArialFont, QUITTER, POSITION_QUITTER, COULEURS[CptCouleurs]);
            }
            GestionSprites.End();
            
            base.Draw(gameTime);
        }
    }
}
