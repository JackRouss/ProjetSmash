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
    public abstract class Personnage : Microsoft.Xna.Framework.DrawableGameComponent, IPause
    {

        #region Propri�t�s, constantes et initialisation.
        protected Keys[] CONTR�LES = { Keys.D, Keys.A, Keys.LeftShift };
        protected abstract string[] NomsSprites { get; set; }
        protected int NbVies { get; set; }
        int VieEnPourcentage { get; set; }



        protected float VitesseD�placementGaucheDroite { get; set; } 
        protected float VitesseD�placementSaut { get; set; } 
        protected float AncienneVitesse { get; set; }
        protected float VitesseMaximaleSaut { get; private set; }
        float Masse { get; set; }


        //Copies de certains �l�ments de l'environnement importants pour le personnage.
        Map Carte { get; set; }
        protected List<Vector3> IntervallesSurfaces { get; set; }
        protected List<Vector3> IntervallesPossibles { get; set; }
        protected Vector3 IntervalleCourante { get; set; }



        //Donn�es propres au personnages, qui seront variables.
        protected Vector3 Position { get; set; }
        protected Vector3 PositionSpawn { get; set; }
        protected Vector3 AnciennePosition { get; set; }
        Rectangle RectangleDeCollision { get; set; }
        
        
        Vector3 VecteurVitesse { get; set; }
        Vector3 VecteurGauche { get; set; }
        Vector3 VecteurQuantit�eDeMouvement { get; set; }
        protected int CptSaut { get; set; }
        
        

        protected float Intervalle_StunAnimation { get; set; }
        protected float IntervalleMAJ { get; set; }
        protected float Temps�coul�DepuisMAJ { get;  set; }
        


        //GestionManette
        protected InputManager GestionInput { get; set; }
        
        public Personnage(Game game, float vitesseD�placementGaucheDroite, float vitesseMaximaleSaut, float masse, Vector3 position, float intervalleMAJ)
            : base(game)
        {
            //Dans cet ordre: Grimper, Descendre, Droite, Gauche, Sauter, Lancer, Attaquer, Bloquer.
            Actions = new List<Delegate>();
            Actions.Add(new Droite(DroiteA));
            Actions.Add(new Gauche(GaucheA));
            Actions.Add(new Bloquer(BloquerA));


            IntervalleMAJ = intervalleMAJ;
            VitesseD�placementGaucheDroite = vitesseD�placementGaucheDroite;
            VitesseMaximaleSaut = vitesseMaximaleSaut;
            Masse = masse;
            Position = position;
            PositionSpawn = position;
            NbVies = 3;
        }
        public override void Initialize()
        {
            GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
            Carte = Game.Components.First(t => t is Map) as Map;
            IntervallesSurfaces = Carte.IntervallesSurfaces;
            VecteurGauche = Vector3.Normalize(Carte.VecteurGauche);
            VecteurVitesse = Vector3.Zero;
            VecteurQuantit�eDeMouvement = Vector3.Zero;
            base.Initialize();
        }
        #endregion



        #region Autres.
        protected bool EstMort()
        {
            return Position.X < -100 || Position.X > 100 || Position.Y < -50;
        }//Mettre des constantes en haut.
        protected virtual void G�rerContr�les()
        {
            AnciennePosition = new Vector3(Position.X, Position.Y, Position.Z);
            for (int i = 0; i < CONTR�LES.Length; ++i)
            {
                if (GestionInput.EstEnfonc�e(CONTR�LES[i]))
                {
                    Actions[i].DynamicInvoke();
                }
            }
        }
        
        protected bool EstDansIntervalleSurface(Vector3 intervalle, Vector3 position)
        {
            return (intervalle.X <= position.X) && (intervalle.Y >= position.X);
        }
        public Vector3 GetPositionPersonnage
        {
            get { return new Vector3(Position.X,Position.Y,Position.Z); }
        }
        #endregion
       


        #region M�thodes �v�nementielles.
        protected List<Delegate> Actions { get; set; }

        delegate void Droite();
        delegate void Gauche();
        delegate void Bloquer();
        
        protected virtual void DroiteA()
        {
            Position -= VecteurGauche * VitesseD�placementGaucheDroite * Temps�coul�DepuisMAJ;
        }
        protected virtual void GaucheA()
        {
            Position += VecteurGauche * VitesseD�placementGaucheDroite * Temps�coul�DepuisMAJ;
        }
        protected virtual void BloquerA(){ }
        #endregion
    }
}
