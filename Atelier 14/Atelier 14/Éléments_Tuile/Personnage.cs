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
using AtelierXNA.Éléments_Tuile;

namespace AtelierXNA
{
    public abstract class Personnage : Microsoft.Xna.Framework.DrawableGameComponent, IPause
    {

        #region Propriétés, constantes et initialisation.
        const float LARGEUR_HITBOX = 3f;
        const float HAUTEUR_HITBOX = 3f;
        const float PROFONDEUR_HITBOX = 3f;
        protected Keys[] CONTRÔLES { get; private set; }
        protected enum ORIENTATION { DROITE, GAUCHE };
        protected enum ÉTAT { COURRIR, SAUTER, ATTAQUER, LANCER, BLOQUER, MORT, IMMOBILE };

        protected ÉTAT ÉTAT_PERSO;
        protected ORIENTATION DIRECTION;



        protected int NbVies { get; set; }
        int VieEnPourcentage { get; set; }



        public BoundingSphere HitBox { get; private set; }

        protected float VitesseDéplacementGaucheDroite { get; set; }
        protected float VitesseMaximaleSaut { get; private set; }
        public float Masse { get; private set; }
        public float Force { get; set; }
        public int DommageAttaque { get; private set; }
        public bool EstEnAttaque { get; protected set; }
        bool EstBouclierActif { get; set; }

        //Copies de certains éléments de l'environnement importants pour le personnage.
        Map Carte { get; set; }
        protected List<Vector3> IntervallesSurfaces { get; set; }
        protected List<Vector3> IntervallesPossibles { get; set; }
        protected Vector3 IntervalleCourante { get; set; }



        //Données propres au personnages, qui seront variables.
        protected Vector3 Position { get; set; }
        public Vector3 GetPositionPersonnage
        {
            get { return new Vector3(Position.X, Position.Y, Position.Z); }
        }
        protected Vector3 PositionSpawn { get; set; }
        protected Vector3 AnciennePosition { get; set; }
        Vector3 VecteurGauche { get; set; }
        public Vector3 VecteurVitesse { get; set; }
        public abstract void DéplacerFrame();
        protected int CptSaut { get; set; }


        protected float IntervalleMAJ { get; set; }
        protected float TempsÉcouléDepuisMAJ { get; set; }


        protected InputControllerManager GestionInputManette { get; set; }
        protected InputManager GestionInputClavier { get; set; }

        public Personnage(Game game, float vitesseDéplacementGaucheDroite, float vitesseMaximaleSaut, float masse, Vector3 position, float intervalleMAJ, Keys[] contrôles)
            : base(game)
        {
            //Propriétés pour le combat (à définir dans le constructeur)
            DommageAttaque = 20;
            Force = 1000000;





            CONTRÔLES = contrôles;
            ÉTAT_PERSO = ÉTAT.IMMOBILE;
            DIRECTION = ORIENTATION.DROITE;

            IntervalleMAJ = intervalleMAJ;
            VitesseDéplacementGaucheDroite = vitesseDéplacementGaucheDroite;
            VitesseMaximaleSaut = vitesseMaximaleSaut;
            Masse = masse;
            Position = position;
            PositionSpawn = position;
            NbVies = 3;
            GénérerHitbox();
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
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;

            NbVies = EstMort() ? --NbVies : NbVies;
            VecteurVitesse = EstMort() ? Vector3.Zero : VecteurVitesse;
            Position = EstMort() ? PositionSpawn : Position;


            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                if (VecteurVitesse.Y == 0 && VecteurVitesse.X == 0 && ÉTAT_PERSO != ÉTAT.IMMOBILE) //Conditions ici pour gérer l'immobilité.
                {
                    ÉTAT_PERSO = ÉTAT.IMMOBILE;
                }
                else if (VecteurVitesse.Y < -1)
                {
                    ÉTAT_PERSO = ÉTAT.SAUTER;
                }
                else if (VecteurVitesse.Y != 0)
                {
                    ÉTAT_PERSO = ÉTAT.SAUTER;
                }

                AnciennePosition = new Vector3(Position.X, Position.Y, Position.Z);

                //Toutes les actions qui peuvent modifier le vecteur vitesse du personnage.
                GérerTouchesEnfoncées();
                GérerAccélérationGravitationnelle();
                GérerFriction();

                Position += VecteurVitesse * TempsÉcouléDepuisMAJ;
                GénérerHitbox();

                TempsÉcouléDepuisMAJ = 0;
            }
            GérerNouvellesTouches();
            if (!GestionInputClavier.EstClavierActivé && !GestionInputManette.EstManetteActivée(PlayerIndex.One))
            {
                if (VecteurVitesse.Y != 0)
                    ÉTAT_PERSO = ÉTAT.SAUTER;
                else
                    ÉTAT_PERSO = ÉTAT.IMMOBILE;
            }
        }

        private void GérerFriction()
        {
            float mu = 0.1f;
            if (VecteurVitesse.Y == 0 && VecteurVitesse.X != 0)
            {
                if (VecteurVitesse.X < 0 && (VecteurVitesse - VecteurGauche * mu * Atelier.ACCÉLÉRATION_GRAVITATIONNELLE).X < 0)
                    VecteurVitesse -= VecteurGauche * mu * Atelier.ACCÉLÉRATION_GRAVITATIONNELLE;
                else if (VecteurVitesse.X > 0 && (VecteurVitesse + VecteurGauche * mu * Atelier.ACCÉLÉRATION_GRAVITATIONNELLE).X > 0)
                    VecteurVitesse += VecteurGauche * mu * Atelier.ACCÉLÉRATION_GRAVITATIONNELLE;
                else
                    VecteurVitesse = new Vector3(0, VecteurVitesse.Y, VecteurVitesse.Z);
            }
        }


        private void GénérerHitbox()
        {
            HitBox = new BoundingSphere(new Vector3(Position.X, Position.Y + HAUTEUR_HITBOX / 2, Position.Z), HAUTEUR_HITBOX / 2);
        }
        protected void GérerAccélérationGravitationnelle()
        {
            AnciennePosition = new Vector3(Position.X, Position.Y, Position.Z);
            Position += Vector3.Up * VecteurVitesse.Y * TempsÉcouléDepuisMAJ;

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
                VecteurVitesse -= Vector3.Up * Atelier.ACCÉLÉRATION_GRAVITATIONNELLE * TempsÉcouléDepuisMAJ;
            }
            Position -= Vector3.Up * VecteurVitesse.Y * TempsÉcouléDepuisMAJ;
        }


        #region Méthodes évenementielles.
        protected void GérerTouchesEnfoncées()
        {


            if ((GestionInputClavier.EstEnfoncée(CONTRÔLES[0]) || GestionInputManette.EstToucheEnfoncée(PlayerIndex.One, Buttons.LeftThumbstickRight)) && ((!EstEnAttaque || VecteurVitesse.Y != 0)))
            {
                VecteurVitesse -= VecteurGauche * VitesseDéplacementGaucheDroite;
                DIRECTION = ORIENTATION.DROITE;
                if (VecteurVitesse.Y == 0)
                {
                    ÉTAT_PERSO = ÉTAT.COURRIR;
                }
            }
            if ((GestionInputClavier.EstEnfoncée(CONTRÔLES[1]) || GestionInputManette.EstToucheEnfoncée(PlayerIndex.One, Buttons.LeftThumbstickLeft)) && ((!EstEnAttaque || VecteurVitesse.Y != 0)))
            {
                VecteurVitesse += VecteurGauche * VitesseDéplacementGaucheDroite;
                DIRECTION = ORIENTATION.GAUCHE;
                if (VecteurVitesse.Y == 0)
                {
                    ÉTAT_PERSO = ÉTAT.COURRIR;
                }
            }
            if (GestionInputClavier.EstEnfoncée(CONTRÔLES[2]) || GestionInputManette.EstToucheEnfoncée(PlayerIndex.One, Buttons.RightShoulder))
            {
                Bloquer();
                if (VecteurVitesse.Y == 0)
                {
                    ÉTAT_PERSO = ÉTAT.BLOQUER;
                }
            }
        }
        private void GérerNouvellesTouches()
        {
            if ((GestionInputClavier.EstNouvelleTouche(CONTRÔLES[3]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.Y)) && CptSaut < 2 && !EstEnAttaque)
            {
                GérerSauts();
            }
            if (GestionInputClavier.EstNouvelleTouche(CONTRÔLES[4]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.X))
            {
                GérerLancer();
                ÉTAT_PERSO = ÉTAT.LANCER;
            }
            if (GestionInputClavier.EstNouvelleTouche(CONTRÔLES[5]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A))
            {
                GérerAttaque();
                ÉTAT_PERSO = ÉTAT.ATTAQUER;
            }
        }
        protected void GérerSauts()//Il faut éviter que si le personnage n'as pas initialement sauter **************À FIX*****************
        {
            if (CptSaut == 0)
            {
                ÉTAT_PERSO = ÉTAT.SAUTER;
                VecteurVitesse += Vector3.Up * VitesseMaximaleSaut;
                ++CptSaut;
            }
            else if (CptSaut == 1)
            {
                ÉTAT_PERSO = ÉTAT.SAUTER;
                VecteurVitesse += Vector3.Up * VitesseMaximaleSaut;
                ++CptSaut;
            }
        }
        private void GérerLancer()
        {

        }
        private void GérerAttaque()
        {

        }
        private void Bloquer()
        {
            EstBouclierActif = true;
        }

        public void EncaisserDégâts(Personnage p)
        {
            if (!EstBouclierActif)
            {
                    if (p.DIRECTION == ORIENTATION.DROITE)
                    {
                        VecteurVitesse += TempsÉcouléDepuisMAJ * Force * Vector3.Right / Masse;
                    }
                    else
                    {
                        VecteurVitesse += TempsÉcouléDepuisMAJ * Force * Vector3.Left / Masse;
                    }
                VieEnPourcentage += p.DommageAttaque;
            }
        }
        #endregion

        #endregion

        #region Booléens de la classe.
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
            return p.SphèreDeCollision.Intersects(HitBox);
        }
        protected abstract bool EstDansIntervalleSurface(Vector3 intervalle, Vector3 position);
        #endregion

    }
}