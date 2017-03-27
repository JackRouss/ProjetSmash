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
    public class MenuCarte : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int NB_FRAMES_PERSONNAGE = 10;
        const float …CHELLE_CARTE1 = 492;
        const float …CHELLE_CARTE2 = 400;
        const float INTERVALLE_MAJ_COULEUR = 1f / 10;
        const int BORDURE_HAUT = 50;
        const string TITRE = "Choix de la carte :";
        public enum …TAT { CARTE1, CARTE2 }
        public …TAT …tat;


        Vector2 POSITION_CARTE1 { get; set; }
        Vector2 POSITION_CARTE2 { get; set; }
        Vector2 POSITION_TITRE { get; set; }
        Color[] COULEURS = { Color.Firebrick, Color.Red, Color.OrangeRed, Color.Orange, Color.Gold, Color.Yellow, Color.YellowGreen, Color.LawnGreen, Color.Green, Color.DarkTurquoise, Color.DeepSkyBlue, Color.Blue, Color.DarkSlateBlue, Color.Indigo, Color.Purple };


        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        InputManager GestionInputClavier { get; set; }
        InputControllerManager GestionInputManette { get; set; }
        SpriteFont ArialFont { get; set; }
        ArriËrePlanDÈroulant Fond…cran { get; set; }

        Texture2D Carte1 { get; set; }
        Vector2 RatioCarte1 { get; set; }//W/H.
        Rectangle RÈgionCarte1 { get; set; }
        Texture2D Carte2 { get; set; }
        Vector2 RatioCarte2 { get; set; }//W/H.
        Rectangle RÈgionCarte2 { get; set; }
      

        public bool PasserMenuSuivant { get; set; }
        float IntervalleMAJAnimation { get; set; }
        float Temps…coulÈDepuisMAJ { get; set; }
        float Temps…coulÈDepuisMAJCouleurs { get; set; }
        int CptFrame { get; set; }
        int CptCouleurs { get; set; }

        public MenuCarte(Game game) 
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {

            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionnaireDeFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInputClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionInputManette = Game.Services.GetService(typeof(InputControllerManager)) as InputControllerManager;
            ArialFont = GestionnaireDeFonts.Find("Arial");
            Fond…cran = new ArriËrePlanDÈroulant(Game, "Fond4", Atelier.INTERVALLE_MAJ_STANDARD);
            Fond…cran.Initialize();

            Carte1 = GestionnaireDeTextures.Find("BackGround1");
            Carte2 = GestionnaireDeTextures.Find("BackGround2");
            RatioCarte1 = new Vector2((float)Carte1.Bounds.Width / Game.Window.ClientBounds.Width,(float)Carte1.Bounds.Height / Game.Window.ClientBounds.Height);
            RatioCarte2 = new Vector2((float)Carte2.Bounds.Width / Game.Window.ClientBounds.Width, (float)Carte2.Bounds.Height / Game.Window.ClientBounds.Height);


            POSITION_CARTE1 = new Vector2(Game.Window.ClientBounds.Width / 4, Game.Window.ClientBounds.Height / 4 - Carte1.Height/ RatioCarte1.Y / 2);
            POSITION_CARTE2 = new Vector2(Game.Window.ClientBounds.Width / 2 - Carte2.Width* RatioCarte2.X / 2 , Game.Window.ClientBounds.Height /1.5f - Carte2.Height*RatioCarte2.Y / 2);
            POSITION_TITRE = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(TITRE).X) / 2, 0);


           
            RÈgionCarte1 = new Rectangle((int)POSITION_CARTE1.X, (int)POSITION_CARTE1.Y, (int)(Game.Window.ClientBounds.Width / 2), (int)(Carte1.Height*RatioCarte1.Y));
            RÈgionCarte2 = new Rectangle((int)POSITION_CARTE2.X, (int)POSITION_CARTE2.Y, (int)(Game.Window.ClientBounds.Width / 2), (int)(Carte2.Height*RatioCarte2.Y));

            //RÈgionCarte1 = new Rectangle((int)POSITION_CARTE1.X, (int)POSITION_CARTE1.Y, (int)(RatioCarte1 * …CHELLE_CARTE2), (int)(1 * …CHELLE_CARTE2) + BORDURE_HAUT);
            //RÈgionCarte2 = new Rectangle((int)POSITION_CARTE2.X, (int)POSITION_CARTE2.Y, (int)(RatioCarte2 * …CHELLE_CARTE1), (int)(1 * …CHELLE_CARTE1) + BORDURE_HAUT);

            base.Initialize();
        }

        

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            float temps…coulÈe = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps…coulÈDepuisMAJ += temps…coulÈe;
            Temps…coulÈDepuisMAJCouleurs += temps…coulÈe;

            GÈrerEntrÈes();
            if (Temps…coulÈDepuisMAJ >= IntervalleMAJAnimation)
            {

                ++CptFrame;
                if (CptFrame == NB_FRAMES_PERSONNAGE)
                {
                    CptFrame = 0;
                }

                Temps…coulÈDepuisMAJ = 0;
            }
            if (Temps…coulÈDepuisMAJCouleurs >= INTERVALLE_MAJ_COULEUR)
            {
                ++CptCouleurs;
                if (CptCouleurs == COULEURS.Length)
                {
                    CptCouleurs = 0;
                }
                Temps…coulÈDepuisMAJCouleurs = 0;
            }
            Fond…cran.Update(gameTime);

        }
        private void GÈrerEntrÈes()//RAJOUTER POUR LA MANETTE.
        {
            if (GestionInputClavier.EstClavierActivÈ || GestionInputManette.EstManetteActivÈe(PlayerIndex.One))
            {
                if (GestionInputClavier.EstNouvelleTouche(Keys.Right) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickRight))
                {
                    …tat = …TAT.CARTE2;
                }
                else if (GestionInputClavier.EstNouvelleTouche(Keys.Left) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickLeft))
                {
                    …tat = …TAT.CARTE1;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Enter) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A))
                {
                    PasserMenuSuivant = true;
                }
            }
        }
        public override void Draw(GameTime gameTime)
        {
            Fond…cran.Draw(gameTime);
            GestionSprites.Begin();
            if (…tat == …TAT.CARTE2)
            {
                GestionSprites.Draw(Carte1, RÈgionCarte1, Color.White);
                GestionSprites.Draw(Carte2, RÈgionCarte2, Color.White);
            }
            else if (…tat == …TAT.CARTE1)
            {
                GestionSprites.Draw(Carte1, RÈgionCarte1, Color.White);
                GestionSprites.Draw(Carte2, RÈgionCarte2, Color.White);
            }
            GestionSprites.DrawString(ArialFont, TITRE, POSITION_TITRE, Color.White);
            GestionSprites.End();
            base.Draw(gameTime);
        }
    }
}
