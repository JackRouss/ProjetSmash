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
        const float TEMPS_PATHFIND_UPDATE = 2f;
        enum ÉTATS {OFFENSIVE, DÉFENSIVE, NEUTRE};
        ÉTATS ÉtatBot { get; set; }

        //Éléments utilisées dans le A_Star:
        Graphe GrapheDéplacements { get; set; }
        Chemin Path { get; set; }
        bool EstEnModeDéplacement { get; set; }
        float TempsPourPath { get; set; }

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
            GrapheDéplacements = new Graphe(Carte);
            Path = new Chemin(GrapheDéplacements);
        }
        public override void Update(GameTime gameTime)
        {
               base.Update(gameTime);
                SphèreDeRéaction = new BoundingSphere(Position,DISTANCE_ATTAQUE);
                float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
                TempsPourPath += tempsÉcoulé;









                if (TempsPourPath >= TEMPS_PATHFIND_UPDATE)
                {
                    PathFind();
                    TempsPourPath = 0;
                }
                SeDéplacerSelonLeChemin();












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
            Node nodeJoueur = CalculerNodeLePlusProche(Joueur.GetPositionPersonnage, GrapheDéplacements.GetGrapheComplet());
            Node nodeBot = CalculerNodeLePlusProche(Position, GrapheDéplacements.GetGrapheComplet());

            Path.A_Star(nodeBot, nodeJoueur);
            if(Path.CheminLePlusCourt != null)
                EstEnModeDéplacement = true;
        }

        private void SeDéplacerSelonLeChemin()
        {
            if(Path.CheminLePlusCourt != null)
            {
                List<Node> CheminLePlusCourt = Path.CheminLePlusCourt;
                Node nodeActuel = CalculerNodeLePlusProche(Position, CheminLePlusCourt);//Ne fonctionnera pas toujours je crois bien: il peut exister un node plus proche, mais il ne sera pas nécessairment celui qui mènera au chemin le plus court.

                if (CheminLePlusCourt.IndexOf(nodeActuel) != CheminLePlusCourt.Count - 1)//Si on n'est pas arrivé à destination.
                {
                    if (nodeActuel.GetPosition().X > CheminLePlusCourt[CheminLePlusCourt.IndexOf(nodeActuel) + 1].GetPosition().X)
                    {
                        Gauche();
                    }
                    else if (nodeActuel.GetPosition().X < CheminLePlusCourt[CheminLePlusCourt.IndexOf(nodeActuel) + 1].GetPosition().X)
                    {
                        Droite();
                    }
                    //else if(nodeActuel.Position.Y < CheminLePlusCourt[CheminLePlusCourt.IndexOf(nodeActuel) + 1].Position.Y)
                    //{
                    //  Bas();
                    //}
                    if (nodeActuel.GetPosition().Y < CheminLePlusCourt[CheminLePlusCourt.IndexOf(nodeActuel) + 1].GetPosition().Y)
                    {
                        GérerSauts();
                        EstEnModeDéplacement = false;
                    }
                }
                
            }
        }

        private Node CalculerNodeLePlusProche(Vector3 position, List<Node> listeÀParcourir)
        {
            Node node = listeÀParcourir[0];
            float distance = Vector3.Distance(node.GetPosition(), position);

            foreach (Node n in listeÀParcourir)
            {
                if (Vector3.Distance(n.GetPosition(), position) < distance)
                {
                    node = n;
                    distance = Vector3.Distance(n.GetPosition(), position);
                }
            }

            return node;
        }
        #endregion

    }
}
