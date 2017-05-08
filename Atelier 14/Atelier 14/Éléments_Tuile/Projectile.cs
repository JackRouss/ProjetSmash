using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA.Éléments_Tuile
{
    public class Projectile : TuileTexturée, IPause
    {
        float FORCE_COUP_PROJECTILE = 50f;
        public Vector3 Position { get; private set; }
        Vector2 Étendue { get; set; }
        public float Vitesse { get; private set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float TempsÉcouléTotal { get; set; }
        float IntervalleMAJ { get; set; }
        public Personnage.ORIENTATION Direction { get; private set; }
        bool Atombé { get; set; }
        public bool ADetruire { get; set; }
        public int Dégat { get; private set; }
        public float Force { get; private set; }
        public BoundingSphere SphèreDeCollision { get; private set; }
        Map Carte { get; set; }
        public PlayerIndex NumPlayer { get; private set; }

        public Projectile(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, string nomTextureTuile, float intervalleMAJ,Personnage.ORIENTATION direction, float vitesse, bool atombé,int dégat, PlayerIndex numPlayer) 
            : base(jeu,homothétieInitiale,rotationInitiale,positionInitiale,étendue,nomTextureTuile,intervalleMAJ)
        {
            NumPlayer = numPlayer;
            Force = FORCE_COUP_PROJECTILE;
            PositionInitiale = positionInitiale;
            Position = new Vector3(positionInitiale.X, positionInitiale.Y + 6, positionInitiale.Z);
            IntervalleMAJ = intervalleMAJ;
            Direction = direction;
            Atombé = atombé;
            Dégat = dégat;
            if (Direction == Personnage.ORIENTATION.DROITE)
            {
                Vitesse = vitesse;
            }
            else
            {
                Vitesse = -vitesse;
            }
        }
        public override void Initialize()
        {
            DrawOrder = 4;
            ADetruire = false;
            CalculerMatriceMonde();
            base.Initialize();
            Carte = Game.Components.First(t => t is Map) as Map;
            if (Direction == Personnage.ORIENTATION.GAUCHE)
            {
                Mirroir();
            }
        }
        public override void Mirroir()
        {
            Vector3 buffer = new Vector3(PtsSommets[0, 0].X, PtsSommets[0, 0].Y, PtsSommets[0, 0].Z);
            PtsSommets[0, 0] = new Vector3(PtsSommets[0, 1].X, PtsSommets[0, 1].Y, PtsSommets[0, 1].Z);
            PtsSommets[0, 1] = buffer;
            buffer = new Vector3(PtsSommets[1, 1].X, PtsSommets[1, 1].Y, PtsSommets[1, 1].Z);
            PtsSommets[1, 1] = new Vector3(PtsSommets[1, 0].X, PtsSommets[1, 0].Y, PtsSommets[1, 0].Z);
            PtsSommets[1, 0] = buffer;
            InitialiserSommets();
        }
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            TempsÉcouléTotal += tempsÉcoulé;
            GérerDelete();

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                GérerDéplacement();
                CalculerMatriceMonde();
                GérerHitbox();
                TempsÉcouléDepuisMAJ = 0;
            }          
        }
        void GérerHitbox()
        {
            SphèreDeCollision = new BoundingSphere(Position, Math.Max(Étendue.X / 2, Étendue.Y / 2));
        }

        void GérerDelete()
        {
            if(ADelete() || ADetruire)
            {
                Game.Components.Remove(this);
            }
        }
        bool ADelete()
        {
            return Position.X >  Carte.LIMITE_MAP.X || Position.X < Carte.LIMITE_MAP.Y;
        }
        void GérerDéplacement()
        {
            if(Atombé)           
                Position = new Vector3(Position.X + Vitesse, Position.Y - (float)((Atelier.ACCÉLÉRATION_GRAVITATIONNELLE_PROJECTILE * Math.Pow(TempsÉcouléTotal, 2)) / 2), Position.Z);         
            else            
                Position = new Vector3(Position.X + Vitesse, Position.Y , Position.Z);
            
        }
        public bool EstEnCollision(Personnage personnage)
        {
            return SphèreDeCollision.Intersects(personnage.HitBox);
        }
        public bool EstEnCollision(BoundingSphere h)
        {
            return SphèreDeCollision.Intersects(h);
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
