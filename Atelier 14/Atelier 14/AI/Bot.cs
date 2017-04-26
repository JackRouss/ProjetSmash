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
        const float TIME_STEP = 1f;
        const float DISTANCE_ATTAQUE = 5f;
        const float DISTANCE_THRESH = 0.2f;
        enum ÉTATS { OFFENSIVE, DÉFENSIVE, NEUTRE };
        ÉTATS ÉtatBot { get; set; }

        //Éléments utilisées dans le A_Star:
        #region A*
        Graphe GrapheDéplacements { get; set; }
        Chemin Path { get; set; }
        Node TargetNode { get; set; }
        List<Node> CheminLePlusCourt { get; set; }
        bool EstEnModeDéplacement { get; set; }
        float TempsPath { get; set; }
        #endregion
        BoundingSphere SphèreDeRéaction { get; set; }

        #region Éléments du monde.
        Personnage Joueur { get; set; }
        Map Carte { get; set; }
        #endregion

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
            CheminLePlusCourt = new List<Node>();
        }
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsPath += tempsÉcoulé;
            base.Update(gameTime);
            GérerÉtat();
            if (ÉtatBot == ÉTATS.OFFENSIVE)
            {
                if(TempsPath >= TIME_STEP)
                {
                    PathFind();
                    TempsPath = 0;
                }
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
            //if (VieEnPourcentage <= 50)
            //{
            //    ÉtatBot = ÉTATS.OFFENSIVE;
            //}
            //if (VieEnPourcentage > 50 && VieEnPourcentage < 150)
            //{
            //    ÉtatBot = ÉTATS.NEUTRE;
            //}
            //if (VieEnPourcentage > 150)
            //{
            //    ÉtatBot = ÉTATS.DÉFENSIVE;
            //}
        }


        #region Défensive.
        private void Survivre()
        {
            if (!EstDansIntervalleSurface(IntervalleCourante, Position))
            {
                RevenirSurSurface();
            }
        }
        private void Éviter(Projectile p)
        {
            //List<Vector3> positionsIntermédiaires = new List<Vector3>();
            //Vector3 positionProjectile = p.Position;
            //float vitesse = p.Direction == Personnage.ORIENTATION.DROITE ? p.Vitesse : p.Vitesse * -1;

            //int cpt = 0;
            //float buffer = positionProjectile.X;
            //Vector3 posActuelle = positionProjectile;
            //while(!EstDansIntervalleSurface(new Vector3(buffer,posActuelle.X,0),new Vector3(Position.X, 0, 0)))//de la marde
            //{
            //    buffer = posActuelle.X;
            //}
        }
        private void Bloquer()
        {

        }
        private void Fuite()
        {

        }
        private void RevenirSurSurface()
        {
            Node n = CalculerNodeLePlusProche(Position, GrapheDéplacements.GetGrapheComplet());
            if (CptSaut == 0)
            {
                GérerSauts();
            }
            else if (VecteurVitesse.Y == 0)
            {
                GérerSauts();
            }

            if (n.GetPosition().X < Position.X)
            {
                Gauche();
            }
            else if (n.GetPosition().X > Position.X)
            {
                Droite();
            }
        }
        #endregion

        #region Offensive
        private void Attaquer()
        {
            
            SeDéplacerSelonLeChemin();
        }
        private void Lancer()
        {
            //S'occupe de viser et de lancer un projectile au bon moment.
        }


        #endregion

        #region Méthodes pour le A*
        private void PathFind()
        {
            Node nodeJoueur = CalculerNodeLePlusProche(Joueur.GetPositionPersonnage, GrapheDéplacements.GetGrapheComplet());
            Node nodeBot = CalculerNodeLePlusProche(Position, GrapheDéplacements.GetGrapheComplet());

            Path.A_Star(nodeBot, nodeJoueur);
            if (Path.CheminLePlusCourt != null)
                EstEnModeDéplacement = true;
        }

        private void SeDéplacerSelonLeChemin()
        {
            if (Path.CheminLePlusCourt != null)
            {
                if(Path.CheminLePlusCourt.Count >= 2)
                {
                    TargetNode = Path.CheminLePlusCourt[1];
                    //Node nodeActuel = CalculerNodeLePlusProche(Position, CheminLePlusCourt);//Ne fonctionnera pas toujours je crois bien: il peut exister un node plus proche, mais il ne sera pas nécessairment celui qui mènera au chemin le plus court.
                    if (Math.Abs(TargetNode.GetPosition().X - Position.X) > DISTANCE_THRESH)
                    {
                        if (TargetNode.GetPosition().X > Position.X)
                        {
                            Droite();
                        }
                        if (TargetNode.GetPosition().X < Position.X)
                        {
                            Gauche();
                        }
                    }
                    if (Math.Abs(TargetNode.GetPosition().Y - Position.Y) > DISTANCE_THRESH)
                    {
                        if (TargetNode.NomPlaquette != Path.CheminLePlusCourt[0].NomPlaquette)
                        {
                            GérerSauts();
                        }
                    }
                    if ((Math.Abs(TargetNode.GetPosition().Y - Position.Y) <= DISTANCE_THRESH || Math.Abs(TargetNode.GetPosition().X - Position.X) <= DISTANCE_THRESH) && (AncienVecteurVitesse.Y < 0 && VecteurVitesse.Y ==0))
                    {
                        Path.CheminLePlusCourt.Remove(TargetNode);
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
