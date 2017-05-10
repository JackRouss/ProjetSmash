using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AtelierXNA.Éléments_Tuile;
using AtelierXNA.Autres;

namespace AtelierXNA.AI
{
    public class Bot : PersonnageAnimé
    {
        const float TEMPS_UPDATE_PATH = 0.1f;
        const float DISTANCE_ATTAQUE = 5f;
        const float RAYON_RÉACTION = 6;

        public const float DISTANCE_THRESH = 1f;

        //Constantes normales
        const float INTERVALLE_MAJ_BOT = 1 / 240f;
        const float P_FRAPPER = 0.07f;
        const float P_SHIELD = 0.05f;
        const float P_LANCER = 0.01f;
        enum ÉTATS { OFFENSIVE, DÉFENSIVE, NEUTRE };
        ÉTATS ÉtatBot { get; set; }

        //Éléments utilisées dans le A_Star:
        #region A*
        Graphe GrapheDéplacements { get; set; }
        Chemin Path { get; set; }
        Node TargetNode { get; set; }
        Node NodeJoueur { get; set; }
        Node NodeBot { get; set; }
        List<Node> CheminLePlusCourt { get; set; }
        bool EstEnModeDéplacement { get; set; }
        bool EstEnSaut { get; set; }
        float TempsÉcouléDepuisMAJBot { get; set; }
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
            SphèreDeRéaction = new BoundingSphere(Position, RAYON_RÉACTION);
            PathFind();
        }
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJBot += tempsÉcoulé;

            if (TempsÉcouléDepuisMAJBot >= INTERVALLE_MAJ_BOT)
            {
                GérerÉtat();
                if (ÉtatBot == ÉTATS.OFFENSIVE)
                {
                    if (!EstEnModeDéplacement)
                    {
                        PathFind();
                    }
                   
                    Attaquer();
                    Bloquer();
                    Lancer();
                    Survivre();
                }
                else if (ÉtatBot == ÉTATS.NEUTRE)
                {

                }
                else if (ÉtatBot == ÉTATS.DÉFENSIVE)
                {
                    
                }
                SphèreDeRéaction = new BoundingSphere(Position, SphèreDeRéaction.Radius);
                TempsÉcouléDepuisMAJBot = 0;
            }

            base.Update(gameTime);
        }

        private void GérerÉtat()
        {
        }


        #region Défensive.
        private void Survivre()
        {
            if(Math.Abs(VecteurVitesse.X) > 0)
            {
                RevenirSurSurface();
            }
        }

        private void RevenirSurSurface()
        {
            if (!EstDansIntervalleSurface(IntervalleCourante, Position))
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
        }

        private void Éviter(Projectile p)
        {
        }
        protected override void Bloquer()
        {
            if(((Joueur.EstEnAttaque && Joueur.EstEnCollision(this)) || ProjectileInRange() && g.NextFloat() <= P_SHIELD)&&!EstEnSaut)
                base.Bloquer();
        }
        private bool ProjectileInRange()
        {
            foreach (GameComponent g in Game.Components)
            {
                if (g is Projectile)
                {
                    if ((g as Projectile).EstEnCollision(SphèreDeRéaction) && (g as Projectile).NumPlayer != this.NumManette)
                        return true;
                }
            }
            return false;
        }
        private void Fuite()
        {

        }
        #endregion

        #region Offensive
        private void Attaquer()
        {
            if (!Joueur.EstEnCollision(this))
            {
                SeDéplacerSelonLeChemin();
            }
            else
            {
                if (g.NextFloat() <= P_FRAPPER)
                    ÉTAT_PERSO = ÉTAT.ATTAQUER;
            }
        }
        private void Lancer()
        {
            if (new BoundingSphere(new Vector3(0, Joueur.GetPositionPersonnage.Y + 5, 0), HAUTEUR_HITBOX).Intersects(new BoundingSphere(new Vector3(0, HitBox.Center.Y, 0), HAUTEUR_HITBOX)) && g.NextFloat() <= P_LANCER)
            {
                if (Joueur.GetPositionPersonnage.X <= Position.X)
                    Gauche();
                else
                    Droite();
                GérerLancer();
            }
        }


        #endregion

        #region Méthodes pour le A*
        private void PathFind()
        {
            NodeJoueur = CalculerNodeLePlusProche(Joueur.GetPositionPersonnage, GrapheDéplacements.GetGrapheComplet());
            NodeBot = CalculerNodeLePlusProche(Position, GrapheDéplacements.GetGrapheComplet());

            Path.A_Star(NodeBot, NodeJoueur);
            if (Path.CheminLePlusCourt != null)
                EstEnModeDéplacement = true;
            CheminLePlusCourt = Path.CopierChemin();
        }

        private void SeDéplacerSelonLeChemin()
        {
            EstEnSaut = VecteurVitesse.Y > 0;
            TargetNode = CheminLePlusCourt[0];

            //Déplacements en Y (sauts). Synchronisé pour maximiser le saut.
            if (Math.Abs(TargetNode.GetPosition().Y - Position.Y) > DISTANCE_THRESH && !EstEnSaut)
            {
                 if (TargetNode.NomPlaquette != Path.CheminLePlusCourt[0].NomPlaquette && TargetNode.GetPosition().Y >= Position.Y && (float)Math.Abs(TargetNode.GetPosition().Y - Position.Y) >= 10)
                {
                    GérerSauts();
                }
            }

            //Déplacements en X.
            if (TargetNode.GetPosition().X > Position.X && Position.X + 0.1f <= TargetNode.GetPosition().X)
            {
                Droite();
            }
            else if (TargetNode.GetPosition().X < Position.X && Position.X - 0.1f >= TargetNode.GetPosition().X)
            {
                Gauche();
            }
            else
            {
                Position = new Vector3(TargetNode.GetPosition().X, Position.Y, Position.Z);
                if (TargetNode.EstExtremiterDroite && CheminLePlusCourt.Count >= 2)
                {
                    Droite();
                }
                if (TargetNode.EstExtremiterGauche && CheminLePlusCourt.Count >= 2)
                {
                    Gauche();
                }
                CheminLePlusCourt.Remove(TargetNode);
                ÉTAT_PERSO = ÉTAT.IMMOBILE;
            }
            if (CheminLePlusCourt.Count == 0)
            {
                EstEnModeDéplacement = false;
            }

        }

        protected override void Droite()
        {
            DIRECTION = ORIENTATION.DROITE;
            if (VecteurVitesse.Y == 0)
                ÉTAT_PERSO = ÉTAT.COURRIR;
            Position += 0.1f * Vector3.Right;
        }
        protected override void Gauche()
        {
            DIRECTION = ORIENTATION.GAUCHE;
            if (VecteurVitesse.Y == 0)
                ÉTAT_PERSO = ÉTAT.COURRIR;
            Position -= 0.1f * Vector3.Right;
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
