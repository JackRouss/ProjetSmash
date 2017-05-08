using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


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
        public int ChoixCarte { get; private set; }
        int NumChoixCarte { get; set; }
        float IntervalleMAJAnimation { get; set; }
        float Temps…coulÈDepuisMAJ { get; set; }
        int CptCouleur { get; set; }


        String[] NomDesCartes { get; set; }
        Texture2D[] Cartes { get; set; }
        Texture2D BackGroundChoix { get; set; }
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
        ArriËrePlanDÈroulant Fond…cran { get; set; }




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
            Fond…cran = new ArriËrePlanDÈroulant(Game, "Fond4", Atelier.INTERVALLE_MAJ_STANDARD);
            Fond…cran.Initialize();
            NumChoixCarte = 0;

            ChargerCartes();

            CalculerPositionCartes();
            CrÈerEmplacementCarte();
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

            LongueurRectangle = (Game.Window.ClientBounds.Width - marge * (nbCarte / 2 + 1)) / (nbCarte / 2);
            HauteurRectangle = (Game.Window.ClientBounds.Height - marge * 4) / 2;
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
            BackGroundChoix = GestionnaireDeTextures.Find("Fond_blanc.svg");
        }

        void CalculerPositionCartes()
        {
            for (int i = 0; i < PositionsCartes.GetLength(1); i++)
            {
                for (int j = 0; j < PositionsCartes.GetLength(0); j++)
                {
                    PositionsCartes[j, i] = new Vector2(marge * (j + 1) + LongueurRectangle * j, marge *(i+3) + HauteurRectangle * i);
                }

            }
           

        }

        void CrÈerEmplacementCarte()
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
            Fond…cran.Update(gameTime);
            GÈrerEntrÈes();
            float temps…coulÈe = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps…coulÈDepuisMAJ += temps…coulÈe;
            if (Temps…coulÈDepuisMAJ >= MenuPersonnage.INTERVALLE_MAJ_COULEUR)
            {
                ++CptCouleur;
                if (CptCouleur == COULEURS.Length)
                {
                    CptCouleur = 0;
                }
                Temps…coulÈDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private void GÈrerEntrÈes()//RAJOUTER POUR LA MANETTE.
        {
            if (GestionInputClavier.EstClavierActivÈ || GestionInputManette.EstManetteActivÈe(PlayerIndex.One))
            {
                if (GestionInputClavier.EstNouvelleTouche(Keys.Right) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickRight))
                {
                    NumChoixCarte += 1;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Left) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickLeft))
                {
                    NumChoixCarte -= 1;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Down) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickDown))
                {
                    NumChoixCarte += nbCarte / 2;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Up) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.LeftThumbstickUp))
                {
                    NumChoixCarte -= nbCarte / 2;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Enter) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A))
                {
                    PasserMenuSuivant = true;
                }
            }
            TestMaximunCpt();
            ChoixCarte = NumChoixCarte;
        }

        private void TestMaximunCpt()
        {
            if(NumChoixCarte >= nbCarte - 1)
            {
                NumChoixCarte = nbCarte - 1;
            }
            if(NumChoixCarte < 0)
            {
                NumChoixCarte = 0;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Fond…cran.Draw(gameTime);
            GestionSprites.Begin();
            GestionSprites.Draw(BackGroundChoix, new Rectangle((int)PositionsCartes[TranspositionTableau(), autre()].X-4, (int)PositionsCartes[TranspositionTableau(), autre()].Y-4, (int)LongueurRectangle + 8, (int)HauteurRectangle + 8), COULEURS[CptCouleur]);
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
        int TranspositionTableau()
        {
            if((NumChoixCarte - nbCarte/2) >= 0)
            {
                return NumChoixCarte - nbCarte / 2;
            }
            else
            {
                return NumChoixCarte;
            }
                
        }

        int autre()
        {
            return (int)Math.Round((double)NumChoixCarte / nbCarte + 0.05);
        }
    }
}
