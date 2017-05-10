using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    public class MenuPersonnage : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int NB_FRAMES_PERSONNAGE = 10;
        const float ÉCHELLE_ROBOT = 492;
        const float ÉCHELLE_NINJA = 400;
        public const float INTERVALLE_MAJ_COULEUR = 1f / 10;
        const int BORDURE_HAUT = 50;
        const string TITRE = "Choix du personnage \n pour Joueur";
        public enum ÉTAT { NINJA, ROBOT}
        public ÉTAT État;



        Vector2 POSITION_NINJA { get; set; }
        Vector2 POSITION_ROBOT { get; set; }
        Vector2 POSITION_TITRE { get; set; }
        Color[] COULEURS = { Color.Firebrick, Color.Red, Color.OrangeRed, Color.Orange,Color.Gold, Color.Yellow,Color.YellowGreen,Color.LawnGreen, Color.Green, Color.DarkTurquoise,Color.DeepSkyBlue, Color.Blue, Color.DarkSlateBlue, Color.Indigo, Color.Purple};


        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        InputManager GestionInputClavier { get; set; }
        InputControllerManager GestionInputManette { get; set; }

        List<Texture2D> Ninja { get; set; }
        float RatioNinja { get; set; }//W/H.
        Rectangle RégionNinja { get; set; }
        List<Texture2D> Robot { get; set; }
        float RatioRobot { get; set; }//W/H.
        Rectangle RégionRobot { get; set; }
        SpriteFont ArialFont { get; set; }
        ArrièrePlanDéroulant FondÉcran { get; set; }

        public bool PasserMenuSuivant { get; set; }
        float IntervalleMAJAnimation { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float TempsÉcouléDepuisMAJCouleurs { get; set; }
        int CptFrame { get; set; }
        int CptCouleurs { get; set; }
        PlayerIndex NumJoueur { get; set; }
        string Message { get; set; }

        public MenuPersonnage(Game jeu, float intervalleMAJAnimation, PlayerIndex numJoueur)
            : base(jeu) 
        {
            IntervalleMAJAnimation = intervalleMAJAnimation;
            NumJoueur = numJoueur;
            État = ÉTAT.NINJA;
            
        }

        public override void Initialize()
        {
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionnaireDeFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            GestionInputClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionInputManette = Game.Services.GetService(typeof(InputControllerManager)) as InputControllerManager;
            ArialFont = GestionnaireDeFonts.Find("Arial");
            FondÉcran = new ArrièrePlanDéroulant(Game,"Fond4",Atelier.INTERVALLE_MAJ_STANDARD);
            FondÉcran.Initialize();

            POSITION_NINJA = new Vector2(Game.Window.ClientBounds.Width / 4 - GestionnaireDeTextures.Find("Idle__000").Width / 2, Game.Window.ClientBounds.Height / 2 - GestionnaireDeTextures.Find("Idle__000").Height / 2);
            POSITION_ROBOT = new Vector2(3 * Game.Window.ClientBounds.Width / 4 - GestionnaireDeTextures.Find("Idle (1)").Width / 2, Game.Window.ClientBounds.Height / 2 - GestionnaireDeTextures.Find("Idle (1)").Height / 2);
            POSITION_TITRE = new Vector2((Game.Window.ClientBounds.Width - ArialFont.MeasureString(TITRE).X) / 2, 0);


            RatioNinja = (float)GestionnaireDeTextures.Find("Idle__000").Bounds.Width / GestionnaireDeTextures.Find("Idle__000").Bounds.Height;
            RatioRobot = (float)GestionnaireDeTextures.Find("Idle (1)").Bounds.Width / GestionnaireDeTextures.Find("Idle (1)").Bounds.Height;
            RégionNinja = new Rectangle((int)POSITION_NINJA.X,(int)POSITION_NINJA.Y,(int)(RatioNinja*ÉCHELLE_NINJA),(int)(1*ÉCHELLE_NINJA) + BORDURE_HAUT);
            RégionRobot = new Rectangle((int)POSITION_ROBOT.X, (int)POSITION_ROBOT.Y, (int)(RatioRobot * ÉCHELLE_ROBOT), (int)(1 * ÉCHELLE_ROBOT) + BORDURE_HAUT);

            Message = TITRE + NumJoueur.ToString() + " :";
            
            CréerTuilesNinja();
            CréerTuilesRobot();
            base.Initialize();
        }

        private void CréerTuilesNinja()
        {
            Texture2D Frame;
            Ninja = new List<Texture2D>();
            for (int i = 0; i < NB_FRAMES_PERSONNAGE; ++i)
            {
                Frame = GestionnaireDeTextures.Find("Idle__00" + i);
                Ninja.Add(Frame);
            }
        }

        private void CréerTuilesRobot()
        {
            Texture2D Frame;
            Robot = new List<Texture2D>();
            for (int i = 1; i <= NB_FRAMES_PERSONNAGE; ++i)
            {
                Frame = GestionnaireDeTextures.Find("Idle " + "(" + i + ")");
                Robot.Add(Frame);
            }
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulée = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulée;
            TempsÉcouléDepuisMAJCouleurs += tempsÉcoulée;

            GérerEntrées();
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJAnimation)
            {

                ++CptFrame;
                if (CptFrame == NB_FRAMES_PERSONNAGE)
                {
                    CptFrame = 0;
                }

                TempsÉcouléDepuisMAJ = 0;
            }
            if (TempsÉcouléDepuisMAJCouleurs >= INTERVALLE_MAJ_COULEUR)
            {
                ++CptCouleurs;
                if (CptCouleurs == COULEURS.Length)
                {
                    CptCouleurs = 0;
                }
                TempsÉcouléDepuisMAJCouleurs = 0;
            }
            FondÉcran.Update(gameTime);
        }

        private void GérerEntrées()//RAJOUTER POUR LA MANETTE.
        {
            if(GestionInputClavier.EstClavierActivé || GestionInputManette.EstManetteActivée(NumJoueur))
            {
                if (GestionInputClavier.EstNouvelleTouche(Keys.Right) || GestionInputManette.EstNouvelleTouche(NumJoueur, Buttons.LeftThumbstickRight))
                {
                    État = ÉTAT.ROBOT;
                }
                else if (GestionInputClavier.EstNouvelleTouche(Keys.Left) || GestionInputManette.EstNouvelleTouche(NumJoueur, Buttons.LeftThumbstickLeft))
                {
                    État = ÉTAT.NINJA;
                }
                if (GestionInputClavier.EstNouvelleTouche(Keys.Enter) || GestionInputManette.EstNouvelleTouche(NumJoueur, Buttons.A))
                {
                    PasserMenuSuivant = true;
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            FondÉcran.Draw(gameTime);
            GestionSprites.Begin();
            if (État == ÉTAT.ROBOT)
            {
                GestionSprites.Draw(Ninja[CptFrame], RégionNinja, Color.White);
                GestionSprites.Draw(Robot[CptFrame], RégionRobot, COULEURS[CptCouleurs]);
            }
            else if (État == ÉTAT.NINJA)
            {
                GestionSprites.Draw(Ninja[CptFrame], RégionNinja, COULEURS[CptCouleurs]);
                GestionSprites.Draw(Robot[CptFrame], RégionRobot, Color.White);
            }
            GestionSprites.DrawString(ArialFont,Message,POSITION_TITRE,Color.White);
            GestionSprites.End();
            base.Draw(gameTime);
        }


    }
}
