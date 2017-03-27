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
    public class MenuCartes : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const string TITRE = "Choix de la carte :";
        const int marge = 30;
        public bool PasserMenuSuivant { get; set; }
        float HauteurRectangle { get; set; }
        float LongueurRectangle { get; set; }
        int nbCarte { get; set; }
        int redneck { get; set; }

        String[] NomDesCartes { get; set; }
        Texture2D[] Cartes { get; set; }
        Vector2[,] PositionsCartes { get; set; }
        Rectangle[,] EmplacementDesCartres { get; set; }
        Vector2 POSITION_TITRE { get; set; }
        Color[] COULEURS = { Color.Firebrick, Color.Red, Color.OrangeRed, Color.Orange, Color.Gold, Color.Yellow, Color.YellowGreen, Color.LawnGreen, Color.Green, Color.DarkTurquoise, Color.DeepSkyBlue, Color.Blue, Color.DarkSlateBlue, Color.Indigo, Color.Purple };


        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        InputManager GestionInputClavier { get; set; }
        InputControllerManager GestionInputManette { get; set; }
        SpriteFont ArialFont { get; set; }
        ArrièrePlanDéroulant FondÉcran { get; set; }




        public MenuCartes(Game game, String[] cartes)
            : base(game)
        {
            NomDesCartes = cartes;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            redneck = 0;
            CalculerDimensionRectangle();
            Cartes = new Texture2D[nbCarte];
            PositionsCartes = new Vector2[nbCarte/2, 2];
            EmplacementDesCartres = new Rectangle[nbCarte/2, 2];
            FondÉcran = new ArrièrePlanDéroulant(Game, "Fond4", Atelier.INTERVALLE_MAJ_STANDARD);
            FondÉcran.Initialize();


            ChargerCartes();

            CalculerPositionCartes();
            CréerEmplacementCarte();
        }
        protected override void LoadContent()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionnaireDeFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInputClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionInputManette = Game.Services.GetService(typeof(InputControllerManager)) as InputControllerManager;
            ArialFont = GestionnaireDeFonts.Find("Arial");
        }

        void CalculerDimensionRectangle()
        {
            nbCarte = DivisionDuMenuSelonLeNombreDeCarte();

            LongueurRectangle = (Game.Window.ClientBounds.X - marge * (nbCarte / 2 + 1)) / (nbCarte / 2);
            HauteurRectangle = (Game.Window.ClientBounds.Y - marge * 2) / 2;
        }

        int DivisionDuMenuSelonLeNombreDeCarte()
        {
            if (NomDesCartes.Length % 2 != 0)
            {
                return NomDesCartes.Length + 1;
            }
            else
            {
                return NomDesCartes.Length;
            }
        }

        void ChargerCartes()
        {
            for (int i = 0; i < NomDesCartes.Length; i++)
            {
                Cartes[i] = GestionnaireDeTextures.Find(NomDesCartes[i]);
            }
        }

        void CalculerPositionCartes()
        {
            for (int i = 0; i < PositionsCartes.GetLength(1); i++)
            {
                for (int j = 0; j < PositionsCartes.GetLength(0); j++)
                {
                    PositionsCartes[j, i] = new Vector2(marge * (j + 1) + LongueurRectangle * j, marge * 4+ i*marge+ HauteurRectangle*i);
                }
            }
           

        }

        void CréerEmplacementCarte()
        {
            for (int i = 0; i < PositionsCartes.GetLength(1); i++)
            {
                for (int j = 0; j < PositionsCartes.GetLength(0); j++)
                {
                    EmplacementDesCartres[j, i] = new Rectangle((int)PositionsCartes[j, i].X, (int)PositionsCartes[j, i].Y, (int)LongueurRectangle, (int)HauteurRectangle);
                }
            }
            
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            FondÉcran.Update(gameTime);
            GérerEntrées();
            base.Update(gameTime);
        }

        private void GérerEntrées()//RAJOUTER POUR LA MANETTE.
        {
            if (GestionInputClavier.EstClavierActivé || GestionInputManette.EstManetteActivée(PlayerIndex.One))
            {
                if (GestionInputClavier.EstNouvelleTouche(Keys.Right) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickRight))
                {
                    //État = ÉTAT.ROBOT;
                }
                else if (GestionInputClavier.EstNouvelleTouche(Keys.Left) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickLeft))
                {
                    //État = ÉTAT.NINJA;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Enter) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A))
                {
                    PasserMenuSuivant = true;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            FondÉcran.Draw(gameTime);
            GestionSprites.Begin();
            for (int i = 0; i < EmplacementDesCartres.GetLength(1); i++)
            {
                for (int j = 0; j < EmplacementDesCartres.GetLength(0); j++)
                {
                    GestionSprites.Draw(Cartes[redneck], EmplacementDesCartres[j,i], Color.White);
                    redneck++;
                }
            }
            redneck = 0;
           
            GestionSprites.DrawString(ArialFont, TITRE, POSITION_TITRE, Color.White);
            GestionSprites.End();
            base.Draw(gameTime);
        }
    }
}
