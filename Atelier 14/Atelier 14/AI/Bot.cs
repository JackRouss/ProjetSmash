using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using AtelierXNA.Éléments_Tuile;
using System;

namespace AtelierXNA.AI
{
    public class Bot : PersonnageAnimé
    {
        #region Propriétés, constantes et initialisation.
        #region Constantes.
        const float DISTANCE_ATTAQUE = 5f;
        const float RAYON_RÉACTION = 6;
        const float VITESSE_PANIQUE = 25f;

        const float DISTANCE_THRESH = 0.07f;
        const float INTERVALLE_MAJ_CHEMIN = 0.5f;
        const float INTERVALLE_MAJ_BOT = 1 / 240f;

        const float P_FRAPPER_D = 1f;
        const float P_BLOQUER_D = 1f;
        const float P_LANCER_D = 1f;
        const float P_CHEMIN_D = 1f;

        const float P_FRAPPER_N = 0.5f;
        const float P_BLOQUER_N = 0.05f;
        const float P_LANCER_N = 0.005f;
        const float P_CHEMIN_N = 0.05f;

        const float P_FRAPPER_F = 0.005f;
        const float P_BLOQUER_F = 0.05f;
        const float P_LANCER_F = 0.005f;
        const float P_CHEMIN_F = 0.1f;
        #endregion

        #region Propriétés en lien avec le A*.
        Graphe GrapheDéplacements { get; set; }
        Chemin Path { get; set; }
        Node TargetNode { get; set; }
        Node AncienTargetNode { get; set; }
        Node NodeJoueur { get; set; }
        Node NodeBot { get; set; }
        List<Node> CheminLePlusCourt { get; set; }
        BoundingSphere SphèreDeRéaction { get; set; }
        #endregion

        #region Éléments extérieurs.
        Personnage Joueur { get; set; }
        Map Carte { get; set; }
        #endregion

        #region Autres Propriétés.
        string Difficulté { get; set; }
        float PFrapper { get; set; }
        float PBloquer { get; set; }
        float PLancer { get; set; }
        float PChemin { get; set; }
        float TempsÉcouléDepuisMAJChemin { get; set; }
        float TempsÉcouléDepuisMAJBot { get; set; }
        enum ÉTATS { OFFENSIVE, DÉFENSIVE, PASSIF };
        ÉTATS ÉtatBot { get; set; }
        #endregion

        #region Initialisation.
        public Bot(Game game, float vitesseDéplacementGaucheDroite, float vitesseMaximaleSaut, float masse, Vector3 position, float intervalleMAJ, Keys[] contrôles, float intervalleMAJAnimation, string[] nomSprites, string type, int[] nbFramesSprites, string difficulté, PlayerIndex numManette)
            : base(game, vitesseDéplacementGaucheDroite, vitesseMaximaleSaut, masse, position, intervalleMAJ, contrôles, intervalleMAJAnimation, nomSprites, type, nbFramesSprites, numManette) { Difficulté = difficulté; }
        public override void Initialize()
        {
            base.Initialize();
            ÉtatBot = ÉTATS.OFFENSIVE;
            InitialiserProbabilités();
            Joueur = Game.Components.First(t => t is Personnage && t != this) as Personnage;
            Carte = Game.Components.First(t => t is Map) as Map;
            GrapheDéplacements = new Graphe(Carte);
            Path = new Chemin(GrapheDéplacements);
            CheminLePlusCourt = new List<Node>();
            SphèreDeRéaction = new BoundingSphere(Position + Vector3.Up * 5, 6);
        }
        private void InitialiserProbabilités()
        {
            if (Difficulté == "Facile")
            {
                PFrapper = P_FRAPPER_F;
                PBloquer = P_BLOQUER_F;
                PLancer = P_LANCER_F;
                PChemin = P_CHEMIN_F;
            }
            else if (Difficulté == "Normal")
            {
                PFrapper = P_FRAPPER_N;
                PBloquer = P_BLOQUER_N;
                PLancer = P_LANCER_N;
                PChemin = P_CHEMIN_N;
            }
            else
            {
                PFrapper = P_FRAPPER_D;
                PBloquer = P_BLOQUER_D;
                PLancer = P_LANCER_D;
                PChemin = P_CHEMIN_D;
            }
        }
        #endregion
        #endregion

        #region Boucle de jeu.
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJBot += tempsÉcoulé;
            TempsÉcouléDepuisMAJChemin += tempsÉcoulé;
            base.Update(gameTime);
            if (TempsÉcouléDepuisMAJBot >= INTERVALLE_MAJ_BOT)
            {
                GérerÉtat();
                if (ÉtatBot == ÉTATS.OFFENSIVE)
                {
                    Attaquer();
                    if(!HitBox.Intersects(Joueur.HitBox))
                    Lancer();
                    if (EstMort())
                    {
                        PathFind(PositionSpawn);
                    }
                }
                else if (ÉtatBot == ÉTATS.DÉFENSIVE)
                {
                    Survivre();
                    Bloquer();
                }
                else if(ÉtatBot == ÉTATS.PASSIF)
                {
                    Patrouiller();
                }

                    
                //À CHANGER D'ENDROIT.///////
                if (CheminLePlusCourt.Count == 0)
                    ÉTAT_PERSO = ÉTAT.IMMOBILE;
                ////////////////////////////
                TempsÉcouléDepuisMAJBot = 0;
            }

            if ((TempsÉcouléDepuisMAJChemin >= INTERVALLE_MAJ_CHEMIN && !EstDansLesAirs()) || CheminÀCalculer())
            {
                PathFind();
                TempsÉcouléDepuisMAJChemin = 0;
        }

            
            SphèreDeRéaction = new BoundingSphere(Position,SphèreDeRéaction.Radius);
        }
        private void GérerÉtat()
        {
            if (Math.Abs(VecteurVitesse.X) > VITESSE_PANIQUE || Joueur.EstEnAttaque)
                ÉtatBot = ÉTATS.DÉFENSIVE;
            else if (CheminLePlusCourt.Count == 0 && EstDansLesAirs())
                ÉtatBot = ÉTATS.PASSIF;
            else
                ÉtatBot = ÉTATS.OFFENSIVE;
        }

        #region Méthodes défensives et passives.
        private void Survivre()
        {
                RevenirSurSurface();
            }
        private void Patrouiller()
        {
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
        protected override void Bloquer()
        {
            if ((((Joueur.EstEnCollision(this)&&Joueur.EstEnAttaque) || ProjectileInRange()) && g.NextFloat() <= PBloquer) && !EstDansLesAirs())
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
        #endregion

        #region Méthodes offensive.
        private void Attaquer()
        {
            if (HitBox.Intersects(Joueur.HitBox) && g.NextFloat() <= PFrapper)
            {
                GérerAttaque();
            }
            else if(!EstEnAttaque && CheminLePlusCourt.Count != 0)
                SeDéplacerSelonLeChemin();
            }
        private void Lancer()
        {
            if (new BoundingSphere(new Vector3(0, Joueur.GetPositionPersonnage.Y + 5, 0), HAUTEUR_HITBOX).Intersects(new BoundingSphere(new Vector3(0, HitBox.Center.Y, 0), HAUTEUR_HITBOX)) && g.NextFloat() <= PLancer && !EstEnAttaque)
            {
                if (Joueur.GetPositionPersonnage.X <= Position.X)
                    Gauche();
                else
                    Droite();
                GérerLancer();
            }
        }

        #region Méthodes pour le déplacement du bot et pour le A*.
        private void PathFind()
        {
            NodeJoueur = CalculerNodeLePlusProche(Joueur.GetPositionPersonnage, GrapheDéplacements.GetGrapheComplet());
            NodeBot = CalculerNodeLePlusProche(Position, GrapheDéplacements.GetGrapheComplet());

            Path.A_Star(NodeBot, NodeJoueur);
            CheminLePlusCourt = Path.CopierChemin();
            TargetNode = CheminLePlusCourt[0];
        }

        private void PathFind(Vector3 départ)
        {
            NodeJoueur = CalculerNodeLePlusProche(Joueur.GetPositionPersonnage, GrapheDéplacements.GetGrapheComplet());
            NodeBot = CalculerNodeLePlusProche(départ, GrapheDéplacements.GetGrapheComplet());

            Path.A_Star(NodeBot, NodeJoueur);
            CheminLePlusCourt = Path.CopierChemin();
            TargetNode = CheminLePlusCourt[0];
        }

        private void SeDéplacerSelonLeChemin()
            {
            if (EstSurNode())
                {
                ChangerDeNode();
                }
            else
            {
                SeDéplacerEnHauteur();
                SeDéplacerEnLargeur();
            }
        }
        private void ChangerDeNode()
            {
            Position = new Vector3(TargetNode.GetPosition().X, Position.Y, Position.Z);
            if (TargetNode.GetPosition().X > Position.X && CheminLePlusCourt.Count != 0)
                Droite();
            else if (TargetNode.GetPosition().X < Position.X && CheminLePlusCourt.Count != 0)
                Gauche();
            CheminLePlusCourt.Remove(TargetNode);
            if(CheminLePlusCourt.Count != 0)
                TargetNode = CheminLePlusCourt[0];
            AncienTargetNode = new Node(TargetNode);
            }
        private void SeDéplacerEnHauteur()
        {
            if (!(EstEnSaut()))
            {
                if (TargetNode.GetPosition().Y > Position.Y)
            {
                    GérerSauts();
                }
            }
            }
        private void SeDéplacerEnLargeur()
        {
            if (TargetNode.GetPosition().X > Position.X  && !EstEnAttaque)
            {
                if (Position.X + DISTANCE_THRESH >= TargetNode.GetPosition().X)
                Position = new Vector3(TargetNode.GetPosition().X, Position.Y, Position.Z);
                else
                    Droite();
                }
            else if (TargetNode.GetPosition().X < Position.X  && !EstEnAttaque)
                {
                if(Position.X - DISTANCE_THRESH <= TargetNode.GetPosition().X)
                    Position = new Vector3(TargetNode.GetPosition().X, Position.Y, Position.Z);
                else
                    Gauche();
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
            if (VecteurVitesse.Y != 0)
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

        #endregion

        #region Booléens de la classe.


        #region Autres.
        private bool EstEnSaut()
        {
            return VecteurVitesse.Y > 0;
        }
        private bool EstEnChute()
        {
            return VecteurVitesse.Y < 0;
        }
        private bool EstDansLesAirs()
        {
            return EstEnChute() || EstEnSaut();
        }
        private bool CheminÀCalculer()
        {
            return CheminLePlusCourt.Count == 0 || AncienTargetNode.NomPlaquette != TargetNode.NomPlaquette;
        }
        private bool EstEnAttenteNodeHauteur()
        {
            return (EstDansLesAirs());
        }
        private bool EstSurNode()
        {
            return TargetNode.Index == CalculerNodeLePlusProche(Position, GrapheDéplacements.GetGrapheComplet()).Index && !EstEnAttenteNodeHauteur();
        }
        #endregion

        #endregion

        #endregion
    }
}
