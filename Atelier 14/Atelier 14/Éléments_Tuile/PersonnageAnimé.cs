using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AtelierXNA.Éléments_Tuile;
using AtelierXNA.Autres;
using Microsoft.Xna.Framework.Media;
//using AtelierXNA.Autres;

namespace AtelierXNA
{
    class TypePersonnageInexistantException : ApplicationException { }
    public class PersonnageAnimé : Personnage
    {
        public const int NB_ANIMATIONS = 11;
        protected const float ÉCHELLE_PERSONNAGE = 4;
        int FrameCourir { get; set; }
        //Données de base.
        string état;
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
        }//L'état dans lequel se trouve le personnage. Donne le nom du sprite à utiliser.
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
        protected int ÉtatNum { get; set; }//Ce même état sous forme numérique (indice du tableau de NomsSprites).
        public string[] NomsSprites { get; private set; }
        int[] NbFramesSprites { get; set; }
        protected TuileTexturéeAnimée Frame { get; set; }
        public Vector2 ZoneAffichageDimensions { get; private set; }


        public PersonnageAnimé(Game game, float vitesseDéplacementGaucheDroite, float vitesseMaximaleSaut, float masse, Vector3 position, float intervalleMAJ, Keys[] contrôles, float intervalleMAJAnimation, string[] nomSprites, string type, int[] nbFramesSprites, PlayerIndex numManette)
            : base(game, vitesseDéplacementGaucheDroite, vitesseMaximaleSaut, masse, position, intervalleMAJ, contrôles, numManette)
        {
            ZoneAffichageDimensions = new Vector2(5, 10);
            Frame = new TuileTexturéeAnimée(Game, 1, Vector3.Zero, position, new Vector2(2, 2), "Idle__000", intervalleMAJ, ZoneAffichageDimensions, nomSprites, nbFramesSprites, type, intervalleMAJAnimation);
            TypePersonnage = type;
            NomsSprites = nomSprites;
            NbFramesSprites = nbFramesSprites;
            État = NomsSprites[4];
        }

        public override void Initialize()
        {
            base.Initialize();
            Frame.Initialize();
        }

        #region Boucle de jeu.
        public override void Update(GameTime gameTime)
        {
            
            if (Frame.CptFrame == NbFramesSprites[ÉtatNum] - 1 && EstEnAttaque)
            {
                ÉTAT_PERSO = ÉTAT.IMMOBILE;
                EstEnAttaque = false;
            }
            base.Update(gameTime);
            GérerTransitionsAnimations();

            Frame.Update(gameTime);
            DéplacerFrame();

        }
        private void GérerTransitionsAnimations()//"Attack__00", "Climb_00", "Dead__00", "Glide_00", "Idle__00", "Jump__00", "Jump_Attack__00", "Jump_Throw__00", "Run__00", "Slide__00", "Throw__00"
        {
            if (!EstEnAttaque)
            {
                if (ÉTAT_PERSO == ÉTAT.COURRIR)
                {
                    État = NomsSprites[8];
                    if(FrameCourir <= 0)
                    {
                        GestionnaireDeSon.Find("Run").Play(); 
                        FrameCourir = 15;
                    }
                }
                else if (ÉTAT_PERSO == ÉTAT.BLOQUER)
                {
                    État = NomsSprites[4];
                }
                else if (ÉTAT_PERSO == ÉTAT.ATTAQUER)
                {
                    if (VecteurVitesse.Y != 0)
                    {
                        État = NomsSprites[6];
                    }          
                    else
                    {
                        État = NomsSprites[0];
                        if (VecteurVitesse.X != 0)
                        {
                            État = NomsSprites[9];
                        }
                    }
                    if (TypePersonnage == "Ninja")
                        GestionnaireDeSon.Find("steelsword").Play();
                    else if (TypePersonnage == "Robot")
                        GestionnaireDeSon.Find("ROBOTATTAQUE").Play();
                    EstEnAttaque = true;
                }
                else if (ÉTAT_PERSO == ÉTAT.IMMOBILE)
                {
                    État = NomsSprites[4];
                }
                else if (ÉTAT_PERSO == ÉTAT.LANCER)
                {
                    if (VecteurVitesse.Y != 0)
                    {
                        État = NomsSprites[7];
                    }
                    else
                    {
                        État = NomsSprites[10];
                    }
                    if (TypePersonnage == "Ninja")
                        GestionnaireDeSon.Find("Arrow").Play();
                    else if (TypePersonnage == "Robot")
                        GestionnaireDeSon.Find("LaserBlasts").Play();
               
                    EstEnAttaque = true;
                }
                else if (ÉTAT_PERSO == ÉTAT.MORT)
                {
                    État = NomsSprites[2];
                }
                else if (ÉTAT_PERSO == ÉTAT.SAUTER)
                {
                    if (VecteurVitesse.Y < -1)
                        État = NomsSprites[3];
                    else
                        État = NomsSprites[5];
                }
                Frame.ChangerÉtat(État);
            }
        }
        protected override void AjouterBouclier()
        {
            BouclierPersonnage = new Bouclier(Game, 1, Vector3.Zero, Position + Vector3.Up * ZoneAffichageDimensions.Y / 2, RayonDuBouclier, new Vector2(2, 30), "BouclierNinja", Atelier.INTERVALLE_MAJ_STANDARD, NumManette);
            Game.Components.Add(BouclierPersonnage);
        }
        public override void Draw(GameTime gameTime)
        {
            if (DIRECTION == ORIENTATION.GAUCHE)
            {
                Frame.Mirroir();
                Frame.Draw(gameTime);
                Frame.Mirroir();
            }
            else
            {
                Frame.Draw(gameTime);
            }
        }
        public override void DéplacerFrame()
        {
            Frame.DéplacerTuile(Position);
        }
        protected override bool EstDansIntervalleSurface(Vector3 intervalle, Vector3 position)
        {
            return (intervalle.X <= position.X) && (intervalle.Y >= position.X);
        }
        #endregion
    }
}