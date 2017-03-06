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
    class TypePersonnageInexistantException : ApplicationException { }
    public class PersonnageAnimé : Personnage
    {
        #region Propriétés, constantes et initialisation.


        #region Attributs et méthodes reliées à leur modification.
        string[] nomsSprites;
        string état;
        private void CalculerÉtatNum()
        {
            int cpt = -1;
            string redneck = "";
            while (redneck != État)
            {
                ++cpt;
                redneck = NomsSprites[cpt];
            }
            ÉtatNum = cpt;
        }
        #endregion



        #region Constantes.
        protected const int NB_FRAMES_PAR_ANIMATION = 10;
        public const int NB_ANIMATIONS = 11;
        protected const float ÉCHELLE_PERSONNAGE = 2;
        protected const int CONSTANTE_MAGIQUE = 300;
        protected enum ORIENTATION { DROITE, GAUCHE };
        #endregion



        #region Propriétés gérant les animations.
        //Données de base.
        protected override string[] NomsSprites
        {
            get
            {
                return nomsSprites;
            }
            set
            {
                nomsSprites = value;
            }
        }
        int[] NbFramesSprites { get; set; }
        string TypePersonnage { get; set; }
        List<List<TuileTexturée>> États { get; set; }



        //Données dynamiques.
        int CptFrameÉtat { get; set; }
        protected string État
        {
            get
            {
                return état;
            }
            set
            {
                état = value;
                CalculerÉtatNum();
            }
        }//L'état dans lequel se trouve le personnage.
        string ÉtatInitial { get; set; }
        protected int ÉtatNum { get; set; }//Ce même état sous forme numérique.
        protected ORIENTATION DIRECTION;


        float IntervalleMAJAnimation { get; set; }
        float TempsÉcouléDepuisMAJAnimation { get; set; }
        bool EstEnAttaque { get; set; }
        #endregion
       


        //Autres.
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }



        #region Initialisation.
        public PersonnageAnimé(Game game, float vitesseDéplacementGaucheDroite, float vitesseMaximaleSaut, float masse, Vector3 position,float intervalleMAJ ,float intervalleMAJAnimation,string[] nomSprites, string type, int[] nbFramesSprites)
            : base(game, vitesseDéplacementGaucheDroite, vitesseMaximaleSaut, masse, position,intervalleMAJ)
        {
            NomsSprites = nomSprites;
            NbFramesSprites = nbFramesSprites;
            TypePersonnage = type;
            IntervalleMAJAnimation = intervalleMAJAnimation;
        }

        protected override void LoadContent()
        {
            if (TypePersonnage == "Ninja")
            {
                LoadContentNinja();
            }
            else if (TypePersonnage == "Robot")
            {
                LoadContentRobot();
            }
            else
            {
                throw new TypePersonnageInexistantException();
            }
        }
        public override void Initialize()
        {
            État = NomsSprites[4];
            ÉtatInitial = État;
            États = new List<List<TuileTexturée>>();
            GestionnaireDeTextures = this.Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            base.Initialize();
        }
        private void LoadContentNinja()
        {
            TuileTexturée Frame;
            List<TuileTexturée> FramesAnimation;
            for (int j = 0; j < NB_ANIMATIONS; ++j)
            {
                FramesAnimation = new List<TuileTexturée>();
                for (int i = 0; i < NbFramesSprites[j]; ++i)
                {
                    Frame = new TuileTexturée(this.Game, ÉCHELLE_PERSONNAGE, Vector3.Zero, Position, new Vector2(2, 2), NomsSprites[j] + i, IntervalleMAJAnimation);
                    Frame.Initialize();
                    FramesAnimation.Add(Frame);
                }
                États.Add(FramesAnimation);
            }
        }
        private void LoadContentRobot()
        {
            TuileTexturée Frame;
            List<TuileTexturée> FramesAnimation;
            for (int j = 0; j < NB_ANIMATIONS; ++j)
            {
                FramesAnimation = new List<TuileTexturée>();
                for (int i = 1; i <= NbFramesSprites[j]; ++i)
                {
                    Frame = new TuileTexturée(this.Game, ÉCHELLE_PERSONNAGE, Vector3.Zero, Position, new Vector2(2, 2), NomsSprites[j] + "(" + i.ToString() + ")", IntervalleMAJAnimation);//Null error risk here.
                    Frame.Initialize();
                    FramesAnimation.Add(Frame);
                }
                États.Add(FramesAnimation);
            }
        }
        #endregion//Problemo: lors du load content, si on décide de rajouter deux personnages du même type, ça va planter.
       


        #endregion

        #region Boucle de jeu.
        public override void Update(GameTime gameTime)
        {
            ÉtatInitial = État;
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            TempsÉcouléDepuisMAJAnimation += tempsÉcoulé;
            Intervalle_StunAnimation -= tempsÉcoulé;

            NbVies = EstMort() ? --NbVies : NbVies;
            Position = EstMort() ? PositionSpawn : Position;



            if (TempsÉcouléDepuisMAJAnimation >= IntervalleMAJAnimation)
            {
                ++CptFrameÉtat;
                TempsÉcouléDepuisMAJAnimation = 0;
            }
            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerAccélérationGravitationnelle();
                base.GérerContrôles();
                for (int i = 0; i < États.Count; ++i)
                    for (int j = 0; j < États[i].Count; ++j)
                        États[i][j].DéplacerTuile(Position/ÉCHELLE_PERSONNAGE);
                TempsÉcouléDepuisMAJ = 0;
            }
            if(!EstEnAttaque)
            {
                GérerContrôles();
                GérerAttaques();
                GérerSauts();       
            }
            if (CptFrameÉtat >= NbFramesSprites[ÉtatNum])
            {
                CptFrameÉtat = 0;
            }
        }
        protected void GérerAccélérationGravitationnelle()
        {
            AnciennePosition = new Vector3(Position.X, Position.Y, Position.Z);
            AncienneVitesse = VitesseDéplacementSaut;

            Position += Vector3.Up * VitesseDéplacementSaut * TempsÉcouléDepuisMAJ;


            int redneck = 0;
            IntervallesPossibles = IntervallesSurfaces.FindAll(t => t.Z <= AnciennePosition.Y && EstDansIntervalleSurface(t, AnciennePosition)) as List<Vector3>;
            foreach (Vector3 intervalle in IntervallesPossibles)
            {
                if (redneck == 0)
                    IntervalleCourante = intervalle;
                else if (IntervalleCourante.Z <= intervalle.Z)
                    IntervalleCourante = intervalle;
                redneck++;
            }

            if (EstDansIntervalleSurface(IntervalleCourante, Position) && (IntervalleCourante.Z >= Position.Y) && (IntervalleCourante.Z <= AnciennePosition.Y) && (EstDansIntervalleSurface(IntervalleCourante, AnciennePosition)))
            {
                Position = AnciennePosition;
                VitesseDéplacementSaut = 0;
                CptSaut = 0;
            }
            else
            {
                VitesseDéplacementSaut -= Atelier.ACCÉLÉRATION_GRAVITATIONNELLE * TempsÉcouléDepuisMAJ;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            if(États.Count !=0)
            {
                if (CptFrameÉtat >= NbFramesSprites[ÉtatNum] || (État != ÉtatInitial))
                {
                    CptFrameÉtat = 0;
                }
                if (DIRECTION == ORIENTATION.GAUCHE)
                {
                    États[ÉtatNum][CptFrameÉtat].Mirroir();
                    États[ÉtatNum][CptFrameÉtat].Draw(gameTime);
                    États[ÉtatNum][CptFrameÉtat].Mirroir();
                }
                else
                {
                    États[ÉtatNum][CptFrameÉtat].Draw(gameTime);
                }
            }
           
        }

#endregion

        #region Gestion des transitions d'animations.
        protected override void GérerContrôles()
        {
            
            if (VitesseDéplacementSaut < -5)
            {
                État = NomsSprites[3];
            }
            if (!GestionInput.EstClavierActivé && VitesseDéplacementSaut == 0)//Idle
            {
                État = NomsSprites[4];
            }
        }
        protected void GérerAttaques()
        {
            GérerLancer();
            if (GestionInput.EstNouvelleTouche(Keys.J))
            {
                if(VitesseDéplacementGaucheDroite != 0)
                {
                    État = NomsSprites[10];
                }
                État = NomsSprites[0];
                if (VitesseDéplacementSaut != 0)
                {
                    État = NomsSprites[6];
                }
              
            }
            //Autres lignes pour le dégâts pis d'autres shit random.
        }
        protected void GérerSauts()
        {
            if (GestionInput.EstNouvelleTouche(Keys.Space))//Sauter ou double saut (si le cas est échéant). Le vecteur "haut" se définira par la position de la carte. 
            {
                if (CptSaut < 2)
                {
                    VitesseDéplacementSaut = VitesseMaximaleSaut;
                    ++CptSaut;
                }              
               État = NomsSprites[5];    
            }
        }
        protected void GérerLancer()
        {
            if (GestionInput.EstNouvelleTouche(Keys.P))
            {
                État = NomsSprites[10];
                if (VitesseDéplacementSaut != 0)
                {
                    État = NomsSprites[7];
                }
            }

        }
        protected override void DroiteA()
        {
            base.DroiteA();
            DIRECTION = ORIENTATION.DROITE;

            if (VitesseDéplacementSaut == 0)
                État = NomsSprites[8];
        }
        protected override void GaucheA()
        {
            base.GaucheA();
            DIRECTION = ORIENTATION.GAUCHE;

            if (VitesseDéplacementSaut == 0)
                État = NomsSprites[8];
        }
        protected override void BloquerA()
        {
            base.BloquerA();
        }
        #endregion
    }
}
