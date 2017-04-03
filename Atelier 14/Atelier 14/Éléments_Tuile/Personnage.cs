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
using AtelierXNA.�l�ments_Tuile;
using AtelierXNA.Autres;

namespace AtelierXNA
{
    public abstract class Personnage : Microsoft.Xna.Framework.DrawableGameComponent, IPause
    {

        #region Propri�t�s, constantes et initialisation.
        const float LARGEUR_HITBOX = 5f;
        const float HAUTEUR_HITBOX = 5f;
        const float PROFONDEUR_HITBOX = 5f;


        //Ces constantes seront � ajouter en propri�t�s.
        const float DUR�E_BOUCLIER = 1f;
        const float ACC�L�RATION_SOL = 500f;
        const float ACC�L�RATION_AIR = 500f;
        const float VITESSE_MAXIMALE_GAUCHE_DROITE = 50f;
        const int DOMMAGE_ATTAQUE = 20;
        const float FORCE_COUP = 500f;
        



        protected Keys[] CONTR�LES { get; private set; }
        protected enum ORIENTATION { DROITE, GAUCHE };
        protected enum �TAT { COURRIR, SAUTER, ATTAQUER, LANCER, BLOQUER, MORT, IMMOBILE };

        protected �TAT �TAT_PERSO;
        protected ORIENTATION DIRECTION;



        protected int NbVies { get; set; }
        int VieEnPourcentage { get; set; }



        public BoundingSphere HitBox { get; private set; }
        Bouclier BouclierPersonnage { get; set; }

        protected float VitesseD�placementGaucheDroite { get; set; }
        protected float VitesseMaximaleSaut { get; private set; }
        public float Masse { get; private set; }
        public float ForceCoup { get; set; }
        public int DommageAttaque { get; private set; }
        public bool EstEnAttaque { get; protected set; }
        bool EstBouclierActif { get; set; }
        bool ASaut�DuneSurface { get; set; }

        //Copies de certains �l�ments de l'environnement importants pour le personnage.
        Map Carte { get; set; }
        protected List<Vector3> IntervallesSurfaces { get; set; }
        protected List<Vector3> IntervallesPossibles { get; set; }
        protected Vector3 IntervalleCourante { get; set; }



        //Donn�es propres au personnages, qui seront variables.
        protected Vector3 Position { get; set; }
        public Vector3 GetPositionPersonnage
        {
            get { return new Vector3(Position.X, Position.Y, Position.Z); }
        }
        protected Vector3 PositionSpawn { get; set; }
        protected Vector3 AnciennePosition { get; set; }
        Vector3 VecteurGauche { get; set; }
        protected Vector3 VecteurVitesse { get; private set; }
        protected Vector3 VecteurVitesseGaucheDroite { get; private set; }
        public abstract void D�placerFrame();
        protected int CptSaut { get; set; }


        protected float IntervalleMAJ { get; set; }
        protected float Temps�coul�DepuisMAJ { get; set; }


        protected InputControllerManager GestionInputManette { get; set; }
        protected InputManager GestionInputClavier { get; set; }

        public Personnage(Game game, float vitesseD�placementGaucheDroite, float vitesseMaximaleSaut, float masse, Vector3 position, float intervalleMAJ, Keys[] contr�les)
            : base(game)
        {
            //Propri�t�s pour le combat (� d�finir dans le constructeur)
            DommageAttaque = DOMMAGE_ATTAQUE;
            ForceCoup = FORCE_COUP;





            CONTR�LES = contr�les;
            �TAT_PERSO = �TAT.IMMOBILE;
            DIRECTION = ORIENTATION.DROITE;

            IntervalleMAJ = intervalleMAJ;
            VitesseD�placementGaucheDroite = vitesseD�placementGaucheDroite;
            VitesseMaximaleSaut = vitesseMaximaleSaut;
            Masse = masse;
            Position = position;
            PositionSpawn = position;
            NbVies = 3;
            G�n�rerHitbox();
        }

        public override void Initialize()
        {
            GestionInputClavier = Game.Services.GetService(typeof(InputManager)) as InputManager;
            GestionInputManette = Game.Services.GetService(typeof(InputControllerManager)) as InputControllerManager;

            Carte = Game.Components.First(t => t is Map) as Map;
            IntervallesSurfaces = Carte.IntervallesSurfaces;

            VecteurGauche = Vector3.Normalize(Carte.VecteurGauche);
            VecteurVitesse = Vector3.Zero;

            base.Initialize();
        }
        #endregion

        #region Boucle de jeu.
        public override void Update(GameTime gameTime)
        {
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += temps�coul�;

            NbVies = EstMort() ? --NbVies : NbVies;
            VieEnPourcentage = EstMort() ? 0 : VieEnPourcentage;
            VecteurVitesse = EstMort() ? Vector3.Zero : VecteurVitesse;
            VecteurVitesseGaucheDroite = EstMort() ? Vector3.Zero : VecteurVitesseGaucheDroite;
            Position = EstMort() ? PositionSpawn : Position;


            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                if (VecteurVitesse.Y == 0 && VecteurVitesse.X == 0 && �TAT_PERSO != �TAT.IMMOBILE) //Conditions ici pour g�rer l'immobilit�.
                {
                    �TAT_PERSO = �TAT.IMMOBILE;
                }
                else if (VecteurVitesse.Y < -1)
                {
                    �TAT_PERSO = �TAT.SAUTER;
                }
                else if (VecteurVitesse.Y != 0)
                {
                    �TAT_PERSO = �TAT.SAUTER;
                }

                AnciennePosition = new Vector3(Position.X, Position.Y, Position.Z);

                //Toutes les actions qui peuvent modifier le vecteur vitesse du personnage.
                G�rerTouchesEnfonc�es();
                G�rerAcc�l�rationGravitationnelle();
                G�rerFriction();

                Position += (VecteurVitesse + VecteurVitesseGaucheDroite) * Temps�coul�DepuisMAJ;
                G�n�rerHitbox();

                Temps�coul�DepuisMAJ = 0;
            }
            G�rerNouvellesTouches();
            if (!GestionInputClavier.EstClavierActiv� && !GestionInputManette.EstManetteActiv�e(PlayerIndex.One))
            {
                if (VecteurVitesse.Y != 0)
                    �TAT_PERSO = �TAT.SAUTER;
                else
                    �TAT_PERSO = �TAT.IMMOBILE;
            }
        }

        private void G�rerFriction()
        {
            float mu = 0.9f;
            if (VecteurVitesse.Y == 0 && VecteurVitesse.X != 0)
            {
                Vector3 AncienVecteurVitesse = VecteurVitesse;
                if (VecteurVitesse.X < 0)
                    VecteurVitesse -= VecteurGauche * mu * Atelier.ACC�L�RATION_GRAVITATIONNELLE;
                else if (VecteurVitesse.X > 0)
                    VecteurVitesse += VecteurGauche * mu * Atelier.ACC�L�RATION_GRAVITATIONNELLE;
                if (Math.Sign(VecteurVitesse.X) != Math.Sign(AncienVecteurVitesse.X))
                    VecteurVitesse = new Vector3(0, VecteurVitesse.Y, VecteurVitesse.Z);
            }
            if (VecteurVitesseGaucheDroite.X != 0 && VecteurVitesse.Y == 0)
            {
                Vector3 AncienVecteurVitesseGaucheDroite = VecteurVitesseGaucheDroite;
                if (VecteurVitesseGaucheDroite.X < 0)
                    VecteurVitesseGaucheDroite -= VecteurGauche * 0.2f * Atelier.ACC�L�RATION_GRAVITATIONNELLE;
                else if (VecteurVitesseGaucheDroite.X > 0)
                    VecteurVitesseGaucheDroite += VecteurGauche * 0.2f * Atelier.ACC�L�RATION_GRAVITATIONNELLE;
                if (Math.Sign(VecteurVitesseGaucheDroite.X) != Math.Sign(AncienVecteurVitesseGaucheDroite.X))
                    VecteurVitesseGaucheDroite = new Vector3(0, VecteurVitesseGaucheDroite.Y, VecteurVitesseGaucheDroite.Z);
            }
        }


        private void G�n�rerHitbox()
        {
            HitBox = new BoundingSphere(new Vector3(Position.X, Position.Y + HAUTEUR_HITBOX / 2, Position.Z), HAUTEUR_HITBOX / 2);
        }
        protected void G�rerAcc�l�rationGravitationnelle()
        {
            AnciennePosition = new Vector3(Position.X, Position.Y, Position.Z);
            Position += Vector3.Up * VecteurVitesse.Y * Temps�coul�DepuisMAJ;

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
                Position = new Vector3(AnciennePosition.X, IntervalleCourante.Z, AnciennePosition.Z);
                VecteurVitesse = new Vector3(VecteurVitesse.X, 0, VecteurVitesse.Z);
                CptSaut = 0;
            }
            else
            {
                VecteurVitesse -= Vector3.Up * Atelier.ACC�L�RATION_GRAVITATIONNELLE * Temps�coul�DepuisMAJ;
            }
            Position -= Vector3.Up * VecteurVitesse.Y * Temps�coul�DepuisMAJ;
        }
        #region M�thodes �venementielles.
        protected void G�rerTouchesEnfonc�es()
        {
            if ((GestionInputClavier.EstEnfonc�e(CONTR�LES[0]) || GestionInputManette.EstToucheEnfonc�e(PlayerIndex.One, Buttons.LeftThumbstickRight)) && ((!EstEnAttaque || VecteurVitesse.Y != 0)) && !EstBouclierActif)
            {
                DIRECTION = ORIENTATION.DROITE;
                if (VecteurVitesse.Y == 0)
                {
                    �TAT_PERSO = �TAT.COURRIR;
                    if (Math.Abs(VecteurVitesseGaucheDroite.X + ACC�L�RATION_SOL * Temps�coul�DepuisMAJ) <= VITESSE_MAXIMALE_GAUCHE_DROITE)//BUG � CAUSE DE �A.
                    {
                        VecteurVitesseGaucheDroite += new Vector3(ACC�L�RATION_SOL*Temps�coul�DepuisMAJ, 0, 0);
                    }
                }
                else
                {
                    if (Math.Abs(VecteurVitesseGaucheDroite.X + ACC�L�RATION_AIR * Temps�coul�DepuisMAJ) <= VITESSE_MAXIMALE_GAUCHE_DROITE)
                    {
                        VecteurVitesseGaucheDroite += new Vector3(ACC�L�RATION_AIR * Temps�coul�DepuisMAJ, 0, 0);
                    }
                }
            }
            if ((GestionInputClavier.EstEnfonc�e(CONTR�LES[1]) || GestionInputManette.EstToucheEnfonc�e(PlayerIndex.One, Buttons.LeftThumbstickLeft)) && ((!EstEnAttaque || VecteurVitesse.Y != 0)) && !EstBouclierActif)
            {

                DIRECTION = ORIENTATION.GAUCHE;
                if (VecteurVitesse.Y == 0)
                {
                    �TAT_PERSO = �TAT.COURRIR;
                    if (Math.Abs(VecteurVitesseGaucheDroite.X + ACC�L�RATION_SOL * Temps�coul�DepuisMAJ) <= VITESSE_MAXIMALE_GAUCHE_DROITE)//BUG � CAUSE DE �A.
                    {
                        VecteurVitesseGaucheDroite -= new Vector3(ACC�L�RATION_SOL * Temps�coul�DepuisMAJ, 0, 0);
                    }
                }
                else
                {
                    if (Math.Abs(VecteurVitesseGaucheDroite.X + ACC�L�RATION_AIR * Temps�coul�DepuisMAJ) <= VITESSE_MAXIMALE_GAUCHE_DROITE)
                    {
                        VecteurVitesseGaucheDroite -= new Vector3(ACC�L�RATION_AIR * Temps�coul�DepuisMAJ, 0, 0);
                    }
                }
            }

            if (GestionInputClavier.EstEnfonc�e(CONTR�LES[2]) || GestionInputManette.EstToucheEnfonc�e(PlayerIndex.One, Buttons.RightShoulder))
            {
                if (VecteurVitesse.Y == 0)
                {
                    Bloquer();
                    �TAT_PERSO = �TAT.BLOQUER;
                }
            }
            else
            {
                EstBouclierActif = false;
                Game.Components.Remove(BouclierPersonnage);
            }
        }
        private void G�rerNouvellesTouches()
        {
            if ((GestionInputClavier.EstNouvelleTouche(CONTR�LES[3]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.Y)) && CptSaut < 2 && !EstEnAttaque&&!EstBouclierActif)
            {
                G�rerSauts();
            }
            if (GestionInputClavier.EstNouvelleTouche(CONTR�LES[4]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.X)&&!EstBouclierActif)
            {
                G�rerLancer();
                �TAT_PERSO = �TAT.LANCER;
            }
            if (GestionInputClavier.EstNouvelleTouche(CONTR�LES[5]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A)&&!EstBouclierActif)
            {
                G�rerAttaque();
                �TAT_PERSO = �TAT.ATTAQUER;
            }
        }
        protected void G�rerSauts()
        {
            if (CptSaut == 0)
            {
                �TAT_PERSO = �TAT.SAUTER;
                VecteurVitesse += (-VecteurVitesse.Y+VitesseMaximaleSaut)*Vector3.Up;
                ++CptSaut;
                if(CptSaut == 1)
                {
                    ASaut�DuneSurface = Position.Y == AnciennePosition.Y;
                }
            }
            else if(CptSaut == 1 && ASaut�DuneSurface)
            {
                �TAT_PERSO = �TAT.SAUTER;
                VecteurVitesse += (-VecteurVitesse.Y + VitesseMaximaleSaut) * Vector3.Up;
                ++CptSaut;
            }
        }
        private void G�rerLancer()
        {

        }
        private void G�rerAttaque()
        {

        }
        private void Bloquer()
        {
            EstBouclierActif = true;
            if(BouclierPersonnage == null || !Game.Components.Contains(BouclierPersonnage))
            {
                BouclierPersonnage = new Bouclier(Game, 1, Vector3.Zero, Position + Vector3.Up * 6, 6, new Vector2(2, 30), "BouclierNinja", Atelier.INTERVALLE_MAJ_STANDARD, DUR�E_BOUCLIER);
                Game.Components.Add(BouclierPersonnage);
            }
        }
        public void EncaisserD�g�ts(Personnage p)
        {
            if (!EstBouclierActif)
            {
                    if (p.DIRECTION == ORIENTATION.DROITE)
                    {
                        VecteurVitesse += ForceCoup *Vector3.Normalize((new Vector3(2,1,0)))*Temps�coul�DepuisMAJ * (VieEnPourcentage / 100f) / Masse;
                    }
                    else
                    {
                        VecteurVitesse += ForceCoup * Vector3.Normalize((new Vector3(-2, 1, 0))) * Temps�coul�DepuisMAJ*(VieEnPourcentage/100f) / Masse;
                    }
                VieEnPourcentage += p.DommageAttaque;
            }
        }
        public void G�rerRecul(Personnage p)
        {
            if(!EstBouclierActif)
                VecteurVitesse += (p.VecteurVitesseGaucheDroite + p.VecteurVitesse) * p.Masse / Masse;
        }
        #endregion

        #endregion

        #region Bool�ens de la classe.
        protected bool EstMort()
        {
            return Position.X < -100 || Position.X > 100 || Position.Y < -50;
        }//Mettre des constantes en haut.
        public bool EstEnCollision(Personnage p)
        {
            return p.HitBox.Intersects(HitBox);
        }
        public bool EstEnCollision(Projectile p)
        {
            return p.Sph�reDeCollision.Intersects(HitBox);
        }
        protected abstract bool EstDansIntervalleSurface(Vector3 intervalle, Vector3 position);
        #endregion

    }
}