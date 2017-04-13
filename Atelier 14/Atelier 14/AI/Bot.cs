using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AtelierXNA.AI;
using AtelierXNA.Éléments_Tuile;

namespace AtelierXNA.AI
{
    public class Bot : PersonnageAnimé
    {
        const float DISTANCE_ATTAQUE = 5f;
        enum ÉTATS {OFFENSIVE, DÉFENSIVE, NEUTRE};
        ÉTATS ÉtatBot { get; set; }

        //Éléments utilisées dans le A_Star:
        Graphe GrapheDéplacements { get; set; }
        Chemin Path { get; set; }

        BoundingSphere SphèreDeRéaction { get; set; }
        Personnage Joueur { get; set; }
        Map Carte { get; set; }
        string Difficulté { get; set; }
        public Bot(Game game, float vitesseDéplacementGaucheDroite, float vitesseMaximaleSaut, float masse, Vector3 position, float intervalleMAJ, Keys[] contrôles, float intervalleMAJAnimation, string[] nomSprites, string type, int[] nbFramesSprites, string difficulté, PlayerIndex numManette)
            : base(game, vitesseDéplacementGaucheDroite, vitesseMaximaleSaut, masse, position, intervalleMAJ, contrôles, intervalleMAJAnimation, nomSprites, type, nbFramesSprites, numManette) { Difficulté = difficulté; TempsÉcouléDepuisMAJ = 0; }


        public override void Initialize()
        {
            ÉtatBot = ÉTATS.OFFENSIVE;
            base.Initialize();
            Joueur = Game.Components.First(t => t is Personnage && t != this) as Personnage;
            Carte = Game.Components.First(t => t is Map) as Map;
            GrapheDéplacements = new Graphe(Carte.Plateformes);
            Path = new Chemin(GrapheDéplacements);
        }
        public override void Update(GameTime gameTime)
        {
               base.Update(gameTime);
                SphèreDeRéaction = new BoundingSphere(Position,DISTANCE_ATTAQUE);
                PathFind();
                GérerÉtat();
                if (ÉtatBot == ÉTATS.OFFENSIVE)
                {
                    Attaquer();
                }
                else if (ÉtatBot == ÉTATS.NEUTRE)
                {

                }
                else if (ÉtatBot == ÉTATS.DÉFENSIVE)
                {
                    Survivre();
                }

        }

        private void GérerÉtat()
        {
            //Les lignes de code pour changer d'état et de sous-états.
        }


        #region Défensive.
        private void Survivre()
        {
            //Ici, subdiviser l'objectif "survivre" sous plusieurs sous-états qui exécuteront
        }
        private void Éviter(Projectile p)
        {


            //Vector3 force = Vector3.Normalize(new Vector3(-1, 0.1f, 0))*p.Force;




        }

        //Retourne le temps t auquel le projectile p sera en collision avec le personnage perso.
        //private float CalculerTrajectoireProjectile(Projectile p, Personnage perso)
        //{
        //    //if(SphèreDeRéaction.Intersects(p.SphèreDeCollision))
        //    //{
        //        Vector3 vitesseProjectile = p.Direction == ORIENTATION.DROITE ? Vector3.Right * p.Vitesse : Vector3.Left * p.Vitesse;

        //        float t1 = (perso.GetPositionPersonnage.X - perso.HitBox.Radius - p.PositionInitiale.X) / vitesseProjectile.X;//min
        //        float t2 = (perso.GetPositionPersonnage.X + perso.HitBox.Radius - p.PositionInitiale.X) / vitesseProjectile.X;//max

        //        float t3 = (float)Math.Sqrt(2 * (perso.GetPositionPersonnage.Y - perso.HitBox.Radius - p.PositionInitiale.Y) / Atelier.ACCÉLÉRATION_GRAVITATIONNELLE_PROJECTILE);//min
        //        float t4 = (float)Math.Sqrt(2 * (perso.GetPositionPersonnage.Y + perso.HitBox.Radius - p.PositionInitiale.Y) / Atelier.ACCÉLÉRATION_GRAVITATIONNELLE_PROJECTILE);//max

        //        if (t1 < 0 || t2 < 0)
        //            return -1;

        //}
        private void Bloquer()
        {

        }
        private void Fuite()
        {

        }
        #endregion


        #region Offensive
        private void Attaquer()
        {
            //S'occupe de donner des coups au bon moment.
        }
        private void Lancer()
        {
            //S'occupe de viser et de lancer un projectile au bon moment.
        }
        private void Kamikaze()
        {
            //Le tout pour le tout...
        }
        private void PathFind()
        {
            //Path.A_Star(GrapheDéplacements.GrapheComplet.ElementAt(0),GrapheDéplacements.GrapheComplet.ElementAt(GrapheDéplacements.GrapheComplet.Count - 3));
        }
        #endregion

    }
}
