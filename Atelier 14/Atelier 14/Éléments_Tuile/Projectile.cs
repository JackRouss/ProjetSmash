using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA.Éléments_Tuile
{
    public class Projectile : TuileTexturée
    {
        const float INCREMENT_VITESSE = 0.005f; 
        Vector3 Position { get; set; }
        float Vitesse { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float IntervalleMAJ { get; set; }
        Personnage.ORIENTATION Direction { get; set; }

        public BoundingSphere SphèreDeCollision { get; private set; }
        public Projectile(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, string nomTextureTuile, float intervalleMAJ,Personnage.ORIENTATION direction) 
            : base(jeu,homothétieInitiale,rotationInitiale,positionInitiale,étendue,nomTextureTuile,intervalleMAJ)
        {
            Position = new Vector3(positionInitiale.X, positionInitiale.Y + 5, positionInitiale.Z);
            IntervalleMAJ = intervalleMAJ;
            Direction = direction;
        }
        public override void Initialize()
        {
            CalculerMatriceMonde();
            base.Initialize();
            if(Direction == Personnage.ORIENTATION.DROITE)
            {
                Vitesse = INCREMENT_VITESSE;
            }
            else
            {
                Vitesse = -INCREMENT_VITESSE;
                Mirroir();
            }
            
        }
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if(TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerDéplacement();
                CalculerMatriceMonde();
            }
           
            base.Update(gameTime);
        }
        void GérerDéplacement()
        {
            Position = new Vector3(Position.X + Vitesse, Position.Y, Position.Z);
        }
        public bool EstEnCollision(Personnage personnage)
        {
            return SphèreDeCollision.Intersects(personnage.HitBox);
        }
        protected override void CalculerMatriceMonde()
        {
            Monde =  Matrix.Identity;
            Monde *= Matrix.CreateScale(HomothétieInitiale);
            Monde *= Matrix.CreateFromYawPitchRoll(RotationInitiale.X,RotationInitiale.Y,RotationInitiale.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }
    }
}
