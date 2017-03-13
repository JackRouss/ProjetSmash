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

        const float LARGEUR_HITBOX = 300;
        const float HAUTEUR_HITBOX = 300;
        const float PROFONDEUR_HITBOX = 300;
        protected Keys[] CONTRÔLES = { Keys.D, Keys.A, Keys.LeftShift, Keys.Space, Keys.P, Keys.J };
        protected enum ORIENTATION { DROITE, GAUCHE };
        protected enum ÉTAT { COURRIR, SAUTER, ATTAQUER, LANCER, BLOQUER, MORT, IMMOBILE };

        protected ÉTAT ÉTAT_PERSO;
        protected ORIENTATION DIRECTION;

        //Éléments abstraits à définir.
        public abstract void DéplacerFrame();

        protected int NbVies { get; set; }
        int VieEnPourcentage { get; set; }
        public BoundingBox HitBox { get; private set; }

        protected float VitesseDéplacementGaucheDroite { get; set; }
        protected float VitesseDéplacementSaut { get; set; }
        protected float AncienneVitesse { get; set; }
        protected float VitesseMaximaleSaut { get; private set; }
        float Masse { get; set; }
        protected bool EstEnAttaque { get; set; }

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

        Vector3 VecteurVitesse { get; set; }
        Vector3 VecteurGauche { get; set; }
        Vector3 VecteurQuantitéeDeMouvement { get; set; }
        protected int CptSaut { get; set; }

        protected float IntervalleMAJ { get; set; }
        protected float TempsÉcouléDepuisMAJ { get; set; }

        protected InputControllerManager GestionInputManette { get; set; }
        protected InputManager GestionInputClavier { get; set; }

        public Personnage(Game game, float vitesseDéplacementGaucheDroite, float vitesseMaximaleSaut, float masse, Vector3 position, float intervalleMAJ)
            : base(game)
        {
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
            VecteurQuantitéeDeMouvement = Vector3.Zero;

            base.Initialize();
        }
        #endregion

        #region Boucle de jeu.
        public override void Update(GameTime gameTime)
        {
            //Changements d'état ne nécessitant pas d'input du joueur.

            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;

            NbVies = EstMort() ? --NbVies : NbVies;
            Position = EstMort() ? PositionSpawn : Position;

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                if (VitesseDéplacementSaut == 0 && AnciennePosition == Position && ÉTAT_PERSO != ÉTAT.IMMOBILE) //Conditions ici pour gérer l'immobilité.
                {
                    ÉTAT_PERSO = ÉTAT.IMMOBILE;
                }
                else if (VitesseDéplacementSaut < -1)
                {
                    ÉTAT_PERSO = ÉTAT.SAUTER;
                }
                else if (VitesseDéplacementSaut != 0)
                {
                    ÉTAT_PERSO = ÉTAT.SAUTER;
                }

                GérerTouchesEnfoncées();
                GérerAccélérationGravitationnelle();
                DéplacerFrame();
                GénérerHitbox();
                TempsÉcouléDepuisMAJ = 0;
            }
            GérerNouvellesTouches();
            if (!GestionInputClavier.EstClavierActivé && !GestionInputManette.EstManetteActivée(PlayerIndex.One))
            {
                if (VitesseDéplacementSaut != 0)
                    ÉTAT_PERSO = ÉTAT.SAUTER;
                else
                    ÉTAT_PERSO = ÉTAT.IMMOBILE;
            }
        }
        private void GénérerHitbox()
        {
            HitBox = new BoundingBox(new Vector3(Position.X - LARGEUR_HITBOX / 2, Position.Y, Position.Z - PROFONDEUR_HITBOX), new Vector3(Position.X + LARGEUR_HITBOX / 2, Position.Y + HAUTEUR_HITBOX, Position.Z));
        }
        private void GénérerHitbox()
        {
            HitBox = new BoundingBox(new Vector3(Position.X - LARGEUR_HITBOX / 2, Position.Y, Position.Z - PROFONDEUR_HITBOX), new Vector3(Position.X + LARGEUR_HITBOX / 2, Position.Y + HAUTEUR_HITBOX, Position.Z));
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
                Position = new Vector3(AnciennePosition.X, IntervalleCourante.Z, AnciennePosition.Z);
                VitesseDéplacementSaut = 0;
                CptSaut = 0;
            }
            else
            {
                VitesseDéplacementSaut -= Atelier.ACCÉLÉRATION_GRAVITATIONNELLE * TempsÉcouléDepuisMAJ;
            }
        }


        #region Méthodes évenementielles.
        protected void GérerTouchesEnfoncées()
        {
            AnciennePosition = new Vector3(Position.X, Position.Y, Position.Z);

            if ((GestionInputClavier.EstEnfoncée(CONTRÔLES[0]) || GestionInputManette.EstToucheEnfoncée(PlayerIndex.One, Buttons.LeftThumbstickRight)) && (!EstEnAttaque || VitesseDéplacementSaut != 0))
            {
                Position -= VecteurGauche * VitesseDéplacementGaucheDroite * TempsÉcouléDepuisMAJ;
                DIRECTION = ORIENTATION.DROITE;
                if (VitesseDéplacementSaut == 0)
                {
                    ÉTAT_PERSO = ÉTAT.COURRIR;
                }
            }
            if ((GestionInputClavier.EstEnfoncée(CONTRÔLES[1]) || GestionInputManette.EstToucheEnfoncée(PlayerIndex.One, Buttons.LeftThumbstickLeft)) && (!EstEnAttaque || VitesseDéplacementSaut != 0))
            {
                Position += VecteurGauche * VitesseDéplacementGaucheDroite * TempsÉcouléDepuisMAJ;
                DIRECTION = ORIENTATION.GAUCHE;
                if (VitesseDéplacementSaut == 0)
                {
                    ÉTAT_PERSO = ÉTAT.COURRIR;
                }
            }
            if (GestionInputClavier.EstEnfoncée(CONTRÔLES[2]) || GestionInputManette.EstToucheEnfoncée(PlayerIndex.One, Buttons.RightShoulder))
            {
                Bloquer();
                if (VitesseDéplacementSaut == 0)
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
           if(CptSaut == 0)
           {
               ÉTAT_PERSO = ÉTAT.SAUTER;
               VitesseDéplacementSaut = VitesseMaximaleSaut;
               ++CptSaut;
           }
            else if(CptSaut == 1)
           {
               ÉTAT_PERSO = ÉTAT.SAUTER;
               VitesseDéplacementSaut = VitesseMaximaleSaut;
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
            //À définir plus tard.
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