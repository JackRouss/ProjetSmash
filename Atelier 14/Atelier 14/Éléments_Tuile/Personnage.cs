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

namespace AtelierXNA
{
    public abstract class Personnage : Microsoft.Xna.Framework.DrawableGameComponent, IPause
    {

        #region Propri�t�s, constantes et initialisation.

        const float LARGEUR_HITBOX = 300;
        const float HAUTEUR_HITBOX = 300;
        const float PROFONDEUR_HITBOX = 300;
        protected Keys[] CONTR�LES = { Keys.D, Keys.A, Keys.LeftShift, Keys.Space, Keys.P, Keys.J };
        protected enum ORIENTATION { DROITE, GAUCHE };
        protected enum �TAT { COURRIR, SAUTER, ATTAQUER, LANCER, BLOQUER, MORT, IMMOBILE };

        protected �TAT �TAT_PERSO;
        protected ORIENTATION DIRECTION;

        //�l�ments abstraits � d�finir.
        public abstract void D�placerFrame();

        protected int NbVies { get; set; }
        int VieEnPourcentage { get; set; }
        public BoundingBox HitBox { get; private set; }

        protected float VitesseD�placementGaucheDroite { get; set; }
        protected float VitesseD�placementSaut { get; set; }
        protected float AncienneVitesse { get; set; }
        protected float VitesseMaximaleSaut { get; private set; }
        float Masse { get; set; }
        protected bool EstEnAttaque { get; set; }

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

        Vector3 VecteurVitesse { get; set; }
        Vector3 VecteurGauche { get; set; }
        Vector3 VecteurQuantit�eDeMouvement { get; set; }
        protected int CptSaut { get; set; }

        protected float IntervalleMAJ { get; set; }
        protected float Temps�coul�DepuisMAJ { get; set; }

        protected InputControllerManager GestionInputManette { get; set; }
        protected InputManager GestionInputClavier { get; set; }

        public Personnage(Game game, float vitesseD�placementGaucheDroite, float vitesseMaximaleSaut, float masse, Vector3 position, float intervalleMAJ)
            : base(game)
        {
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
            VecteurQuantit�eDeMouvement = Vector3.Zero;

            base.Initialize();
        }
        #endregion

        #region Boucle de jeu.
        public override void Update(GameTime gameTime)
        {
            //Changements d'�tat ne n�cessitant pas d'input du joueur.

            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Temps�coul�DepuisMAJ += temps�coul�;

            NbVies = EstMort() ? --NbVies : NbVies;
            Position = EstMort() ? PositionSpawn : Position;

            if (Temps�coul�DepuisMAJ >= IntervalleMAJ)
            {
                if (VitesseD�placementSaut == 0 && AnciennePosition == Position && �TAT_PERSO != �TAT.IMMOBILE) //Conditions ici pour g�rer l'immobilit�.
                {
                    �TAT_PERSO = �TAT.IMMOBILE;
                }
                else if (VitesseD�placementSaut < -1)
                {
                    �TAT_PERSO = �TAT.SAUTER;
                }
                else if (VitesseD�placementSaut != 0)
                {
                    �TAT_PERSO = �TAT.SAUTER;
                }

                G�rerTouchesEnfonc�es();
                G�rerAcc�l�rationGravitationnelle();
                D�placerFrame();
                G�n�rerHitbox();
                Temps�coul�DepuisMAJ = 0;
            }
            G�rerNouvellesTouches();
            if (!GestionInputClavier.EstClavierActiv� && !GestionInputManette.EstManetteActiv�e(PlayerIndex.One))
            {
                if (VitesseD�placementSaut != 0)
                    �TAT_PERSO = �TAT.SAUTER;
                else
                    �TAT_PERSO = �TAT.IMMOBILE;
            }
        }
        private void G�n�rerHitbox()
        {
            HitBox = new BoundingBox(new Vector3(Position.X - LARGEUR_HITBOX / 2, Position.Y, Position.Z - PROFONDEUR_HITBOX), new Vector3(Position.X + LARGEUR_HITBOX / 2, Position.Y + HAUTEUR_HITBOX, Position.Z));
        }
        private void G�n�rerHitbox()
        {
            HitBox = new BoundingBox(new Vector3(Position.X - LARGEUR_HITBOX / 2, Position.Y, Position.Z - PROFONDEUR_HITBOX), new Vector3(Position.X + LARGEUR_HITBOX / 2, Position.Y + HAUTEUR_HITBOX, Position.Z));
        }
        protected void G�rerAcc�l�rationGravitationnelle()
        {
            AnciennePosition = new Vector3(Position.X, Position.Y, Position.Z);
            AncienneVitesse = VitesseD�placementSaut;
            Position += Vector3.Up * VitesseD�placementSaut * Temps�coul�DepuisMAJ;

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
                VitesseD�placementSaut = 0;
                CptSaut = 0;
            }
            else
            {
                VitesseD�placementSaut -= Atelier.ACC�L�RATION_GRAVITATIONNELLE * Temps�coul�DepuisMAJ;
            }
        }


        #region M�thodes �venementielles.
        protected void G�rerTouchesEnfonc�es()
        {
            AnciennePosition = new Vector3(Position.X, Position.Y, Position.Z);

            if ((GestionInputClavier.EstEnfonc�e(CONTR�LES[0]) || GestionInputManette.EstToucheEnfonc�e(PlayerIndex.One, Buttons.LeftThumbstickRight)) && (!EstEnAttaque || VitesseD�placementSaut != 0))
            {
                Position -= VecteurGauche * VitesseD�placementGaucheDroite * Temps�coul�DepuisMAJ;
                DIRECTION = ORIENTATION.DROITE;
                if (VitesseD�placementSaut == 0)
                {
                    �TAT_PERSO = �TAT.COURRIR;
                }
            }
            if ((GestionInputClavier.EstEnfonc�e(CONTR�LES[1]) || GestionInputManette.EstToucheEnfonc�e(PlayerIndex.One, Buttons.LeftThumbstickLeft)) && (!EstEnAttaque || VitesseD�placementSaut != 0))
            {
                Position += VecteurGauche * VitesseD�placementGaucheDroite * Temps�coul�DepuisMAJ;
                DIRECTION = ORIENTATION.GAUCHE;
                if (VitesseD�placementSaut == 0)
                {
                    �TAT_PERSO = �TAT.COURRIR;
                }
            }
            if (GestionInputClavier.EstEnfonc�e(CONTR�LES[2]) || GestionInputManette.EstToucheEnfonc�e(PlayerIndex.One, Buttons.RightShoulder))
            {
                Bloquer();
                if (VitesseD�placementSaut == 0)
                {
                    �TAT_PERSO = �TAT.BLOQUER;
                }
            }
        }
        private void G�rerNouvellesTouches()
        {
            if ((GestionInputClavier.EstNouvelleTouche(CONTR�LES[3]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.Y)) && CptSaut < 2 && !EstEnAttaque)
            {
                G�rerSauts();
            }
            if (GestionInputClavier.EstNouvelleTouche(CONTR�LES[4]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.X))
            {
                G�rerLancer();
                �TAT_PERSO = �TAT.LANCER;
            }
            if (GestionInputClavier.EstNouvelleTouche(CONTR�LES[5]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A))
            {
                G�rerAttaque();
                �TAT_PERSO = �TAT.ATTAQUER;
            }
        }
        protected void G�rerSauts()//Il faut �viter que si le personnage n'as pas initialement sauter **************� FIX*****************
        {
           if(CptSaut == 0)
           {
               �TAT_PERSO = �TAT.SAUTER;
               VitesseD�placementSaut = VitesseMaximaleSaut;
               ++CptSaut;
           }
            else if(CptSaut == 1)
           {
               �TAT_PERSO = �TAT.SAUTER;
               VitesseD�placementSaut = VitesseMaximaleSaut;
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
            //� d�finir plus tard.
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