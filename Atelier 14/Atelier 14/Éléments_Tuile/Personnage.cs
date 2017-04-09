
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
using AtelierXNA.Autres;

namespace AtelierXNA
{
    public abstract class Personnage : Microsoft.Xna.Framework.DrawableGameComponent, IPause
    {

        #region Propriétés, constantes et initialisation.
        const float LARGEUR_HITBOX = 5f;
        const float HAUTEUR_HITBOX = 5f;
        const float PROFONDEUR_HITBOX = 5f;


        //Ces constantes seront à ajouter en propriétés.
        const float VITESSE_MAX_GAUCHE_DROITE = 50f;
        const float DURÉE_BOUCLIER = 1f;
        const float ACCÉLÉRATION_SOL = 500f;
        const float ACCÉLÉRATION_AIR = 500f;
        const int DOMMAGE_ATTAQUE = 20;
        const float FORCE_COUP = 500000f;




        protected Keys[] CONTRÔLES { get; private set; }
        public enum ORIENTATION { DROITE, GAUCHE };
        protected enum ÉTAT { COURRIR, SAUTER, ATTAQUER, LANCER, BLOQUER, MORT, IMMOBILE };

        protected ÉTAT ÉTAT_PERSO;
        protected ORIENTATION DIRECTION;


        protected string TypePersonnage { get; set; }
        public int NbVies { get; private set; }
        int VieEnPourcentage { get; set; }



        public BoundingSphere HitBox { get; private set; }
        //public BoundingBox HitBox { get; private set; }
        public Bouclier BouclierPersonnage { get; private set; }

        float TempsEntreProjectile { get; set; }
        protected float VitesseDéplacementGaucheDroite { get; set; }
        protected float VitesseMaximaleSaut { get; private set; }
        public float Masse { get; private set; }
        public float ForceCoup { get; set; }
        public int DommageAttaque { get; private set; }
        public bool EstEnAttaque { get; protected set; }
        bool EstBouclierActif { get; set; }
        bool ASautéDuneSurface { get; set; }

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
        protected Vector3 VecteurVitesse { get; private set; }
        protected Vector3 VecteurVitesseGaucheDroite { get; private set; }
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
            DommageAttaque = DOMMAGE_ATTAQUE;
            ForceCoup = FORCE_COUP;



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
            VieEnPourcentage = EstMort() ? 0 : VieEnPourcentage;
            VecteurVitesse = EstMort() ? Vector3.Zero : VecteurVitesse;
            VecteurVitesseGaucheDroite = EstMort() ? Vector3.Zero : VecteurVitesseGaucheDroite;
            Position = EstMort() ? PositionSpawn : Position;


            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                TempsEntreProjectile--;
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

                Position += (VecteurVitesse + VecteurVitesseGaucheDroite) * TempsÉcouléDepuisMAJ;
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
            float mu_air = 0.1f;
            float mu_sol = 0.2f;
            if (VecteurVitesse.X != 0)
            {
                Vector3 AncienVecteurVitesse = VecteurVitesse;
                if (VecteurVitesse.X < 0)
                    VecteurVitesse -= VecteurGauche * mu_air * Atelier.ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE;
                else if (VecteurVitesse.X > 0)
                    VecteurVitesse += VecteurGauche * mu_air * Atelier.ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE;
                if (Math.Sign(VecteurVitesse.X) != Math.Sign(AncienVecteurVitesse.X))
                    VecteurVitesse = new Vector3(0, VecteurVitesse.Y, VecteurVitesse.Z);
            }
            if (VecteurVitesseGaucheDroite.X != 0)
            {
                Vector3 AncienVecteurVitesseGaucheDroite = VecteurVitesseGaucheDroite;
                if (VecteurVitesseGaucheDroite.X < 0)
                    VecteurVitesseGaucheDroite -= VecteurGauche * mu_sol * Atelier.ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE;
                else if (VecteurVitesseGaucheDroite.X > 0)
                    VecteurVitesseGaucheDroite += VecteurGauche * mu_sol * Atelier.ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE;
                if (Math.Sign(VecteurVitesseGaucheDroite.X) != Math.Sign(AncienVecteurVitesseGaucheDroite.X))
                    VecteurVitesseGaucheDroite = new Vector3(0, VecteurVitesseGaucheDroite.Y, VecteurVitesseGaucheDroite.Z);
            }
        }
        private void GénérerHitbox()
        {
            HitBox = new BoundingSphere(new Vector3(Position.X, Position.Y + 5, Position.Z), 3);
            //HitBox = new BoundingBox(new Vector3(Position.X - LARGEUR_HITBOX/2,Position.Y+HAUTEUR_HITBOX/2,Position.Z + PROFONDEUR_HITBOX/2),new Vector3(Position.X + LARGEUR_HITBOX/2,Position.Y,Position.Z - PROFONDEUR_HITBOX/2));
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
                VecteurVitesse -= Vector3.Up * Atelier.ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE * TempsÉcouléDepuisMAJ;
            }
            Position -= Vector3.Up * VecteurVitesse.Y * TempsÉcouléDepuisMAJ;
        }
        #region Méthodes évenementielles.
        protected void GérerTouchesEnfoncées()
        {
            if ((GestionInputClavier.EstEnfoncée(CONTRÔLES[0]) || GestionInputManette.EstToucheEnfoncée(PlayerIndex.One, Buttons.LeftThumbstickRight)) && ((!EstEnAttaque || VecteurVitesse.Y != 0)) && !EstBouclierActif)
            {
                DIRECTION = ORIENTATION.DROITE;
                if (VecteurVitesse.Y == 0)
                {
                    ÉTAT_PERSO = ÉTAT.COURRIR;
                    if (Math.Abs(VecteurVitesseGaucheDroite.X + ACCÉLÉRATION_SOL * TempsÉcouléDepuisMAJ) <= VITESSE_MAX_GAUCHE_DROITE)
                    {
                        VecteurVitesseGaucheDroite += new Vector3(ACCÉLÉRATION_SOL * TempsÉcouléDepuisMAJ, 0, 0);
                    }

                }
                else if (Math.Abs(VecteurVitesseGaucheDroite.X + ACCÉLÉRATION_AIR * TempsÉcouléDepuisMAJ) <= VITESSE_MAX_GAUCHE_DROITE)
                {
                    VecteurVitesseGaucheDroite += new Vector3(ACCÉLÉRATION_AIR * TempsÉcouléDepuisMAJ, 0, 0);
                }
            }
            if ((GestionInputClavier.EstEnfoncée(CONTRÔLES[1]) || GestionInputManette.EstToucheEnfoncée(PlayerIndex.One, Buttons.LeftThumbstickLeft)) && ((!EstEnAttaque || VecteurVitesse.Y != 0)) && !EstBouclierActif)
            {

                DIRECTION = ORIENTATION.GAUCHE;
                if (VecteurVitesse.Y == 0)
                {
                    ÉTAT_PERSO = ÉTAT.COURRIR;
                    if (Math.Abs(VecteurVitesseGaucheDroite.X - ACCÉLÉRATION_SOL * TempsÉcouléDepuisMAJ) <= VITESSE_MAX_GAUCHE_DROITE)
                    {
                        VecteurVitesseGaucheDroite -= new Vector3(ACCÉLÉRATION_SOL * TempsÉcouléDepuisMAJ, 0, 0);
                    }
                }
                else if (Math.Abs(VecteurVitesseGaucheDroite.X - ACCÉLÉRATION_AIR * TempsÉcouléDepuisMAJ) <= VITESSE_MAX_GAUCHE_DROITE)
                {
                    VecteurVitesseGaucheDroite -= new Vector3(ACCÉLÉRATION_AIR * TempsÉcouléDepuisMAJ, 0, 0);
                }
            }

            if (GestionInputClavier.EstEnfoncée(CONTRÔLES[2]) || GestionInputManette.EstToucheEnfoncée(PlayerIndex.One, Buttons.RightShoulder))
            {
                if (VecteurVitesse.Y == 0)
                {
                    Bloquer();
                    ÉTAT_PERSO = ÉTAT.BLOQUER;
                }
            }
            else
            {
                EstBouclierActif = false;
                Game.Components.Remove(BouclierPersonnage);
            }
        }

        private void GérerNouvellesTouches()
        {
            if ((GestionInputClavier.EstNouvelleTouche(CONTRÔLES[3]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.Y)) && CptSaut < 2 && !EstEnAttaque && !EstBouclierActif)
            {
                GérerSauts();
            }
            if (GestionInputClavier.EstNouvelleTouche(CONTRÔLES[4]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.X) && !EstBouclierActif)
            {
                GérerLancer();
                ÉTAT_PERSO = ÉTAT.LANCER;
            }
            if (GestionInputClavier.EstNouvelleTouche(CONTRÔLES[5]) || GestionInputManette.EstNouvelleTouche(PlayerIndex.One, Buttons.A) && !EstBouclierActif)
            {
                GérerAttaque();
                ÉTAT_PERSO = ÉTAT.ATTAQUER;
            }
        }
        protected void GérerSauts()
        {
            if (CptSaut == 0)
            {
                ÉTAT_PERSO = ÉTAT.SAUTER;
                VecteurVitesse += (-VecteurVitesse.Y + VitesseMaximaleSaut) * Vector3.Up;
                ++CptSaut;
                if (CptSaut == 1)
                {
                    ASautéDuneSurface = Position.Y == AnciennePosition.Y;
                }
            }
            else if (CptSaut == 1 && ASautéDuneSurface)
            {
                ÉTAT_PERSO = ÉTAT.SAUTER;
                VecteurVitesse += (-VecteurVitesse.Y + VitesseMaximaleSaut) * Vector3.Up;
                ++CptSaut;
            }
        }
        private void GérerLancer()
        {
            if (TempsEntreProjectile <= 0)
            {
                if (this.TypePersonnage == "Ninja")
                {
                    Projectile p = new Projectile(Game, 1f, new Vector3(0, 0, -MathHelper.Pi / 2), Position, new Vector2(2, 4), "Ninja/Kunai", AtelierXNA.Atelier.INTERVALLE_MAJ_STANDARD, DIRECTION, 0.75f, true, 4);
                    Game.Components.Add(p);
                    TempsEntreProjectile = 30;
                }
                if (this.TypePersonnage == "Robot")
                {
                    Projectile p = new Projectile(Game, 1f, new Vector3(0, 0, 0), Position, new Vector2(4, 2), "Robot/laser", AtelierXNA.Atelier.INTERVALLE_MAJ_STANDARD, DIRECTION, 1f, false, 3);
                    Game.Components.Add(p);
                    TempsEntreProjectile = 40;
                }
            }
        }
        private void GérerAttaque()
        {

        }
        private void Bloquer()
        {
            EstBouclierActif = true;
            if (BouclierPersonnage == null || !Game.Components.Contains(BouclierPersonnage))
            {
                BouclierPersonnage = new Bouclier(Game, 1, Vector3.Zero, Position + Vector3.Up * 6, 6, new Vector2(2, 30), "BouclierNinja", Atelier.INTERVALLE_MAJ_STANDARD, DURÉE_BOUCLIER);
                //BouclierPersonnage = new Bouclier(Game, 1, Vector3.Zero, HitBox.Center, HitBox.Radius, new Vector2(2, 30), "BouclierNinja", Atelier.INTERVALLE_MAJ_STANDARD, DURÉE_BOUCLIER);
                Game.Components.Add(BouclierPersonnage);
            }
        }
        public void EncaisserDégâts(Personnage p)
        {
            if (!EstBouclierActif)
            {
                if (p.DIRECTION == ORIENTATION.DROITE)
                {
                    VecteurVitesse += p.ForceCoup * Vector3.Normalize(new Vector3(1, 0.1f, 0)) * TempsÉcouléDepuisMAJ * (1 + VieEnPourcentage / 100f) / Masse;
                }
                else
                {
                    VecteurVitesse += p.ForceCoup * Vector3.Normalize(new Vector3(-1, 0.1f, 0)) * TempsÉcouléDepuisMAJ * (1 + VieEnPourcentage / 100f) / Masse;
                }
                VieEnPourcentage += p.DommageAttaque;
            }
        }
        public void GérerRecul(Personnage p)
        {
            if (!EstBouclierActif)
                VecteurVitesse += (p.VecteurVitesseGaucheDroite + p.VecteurVitesse) * p.Masse / Masse;
        }

        public void EncaisserDégâts(Projectile p)
        {
            if (!EstBouclierActif)
            {
                VieEnPourcentage += p.Dégat;
                if (p.Direction == ORIENTATION.DROITE)
                {
                    VecteurVitesse += TempsÉcouléDepuisMAJ * p.Force * Vector3.Right * (1 + VieEnPourcentage / 100) / Masse ;
                }
                else
                {
                    VecteurVitesse += TempsÉcouléDepuisMAJ * p.Force * Vector3.Left * (1 + VieEnPourcentage / 100) / Masse ;
                }
            }
        }
        #endregion

        #endregion

        #region Booléens de la classe.
        protected bool EstMort()
        {
            return Position.X > Carte.LIMITE_MAP.X || Position.X < Carte.LIMITE_MAP.Y || Position.Y > Carte.LIMITE_MAP.Z || Position.Y < Carte.LIMITE_MAP.W;
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