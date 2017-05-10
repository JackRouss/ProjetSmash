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
        const float INTERVALLE_MAJ_CHEMIN = 0.25f;
        const float INTERVALLE_MAJ_BOT = 1 / 240f;

        const float P_FRAPPER_D = 1f;
        const float P_BLOQUER_D = 1f;
        const float P_LANCER_D = 1f;
        const float P_CHEMIN_D = 1f;

        const float P_FRAPPER_N = 1f;
        const float P_BLOQUER_N = 1f;
        const float P_LANCER_N = 1f;
        const float P_CHEMIN_N = 1f;

        const float P_FRAPPER_F = 1f;
        const float P_BLOQUER_F = 1f;
        const float P_LANCER_F = 1f;
        const float P_CHEMIN_F = 1f;
        #endregion

        #region Propriétés en lien avec le A*.
        Graphe GrapheDéplacements { get; set; }
        Chemin Path { get; set; }
        Node TargetNode { get; set; }
        Node NodeJoueur { get; set; }
        Node NodeBot { get; set; }
        List<Node> CheminLePlusCourt { get; set; }
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

            if (TempsÉcouléDepuisMAJBot >= INTERVALLE_MAJ_BOT)
            {
                GérerÉtat();
                if (ÉtatBot == ÉTATS.OFFENSIVE)
                {
                    Attaquer();
                    Lancer();
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
                if(EstMort())
                {
                    PathFind(PositionSpawn);
                }
                if (VecteurVitesse.Y != 0 && !EstEnAttaque)
                {
                    ÉTAT_PERSO = ÉTAT.SAUTER;
                }
                ////////////////////////////
                TempsÉcouléDepuisMAJBot = 0;
            }

            if ((TempsÉcouléDepuisMAJChemin >= INTERVALLE_MAJ_CHEMIN && !EstDansLesAirs()) || CheminÀCalculer())
            {
                PathFind();
                TempsÉcouléDepuisMAJChemin = 0;
            }

            base.Update(gameTime);
        }
        private void GérerÉtat()
        {
            if (Math.Abs(VecteurVitesse.X) > VITESSE_PANIQUE)
                ÉtatBot = ÉTATS.DÉFENSIVE;
            if (CheminLePlusCourt.Count == 0 && EstDansLesAirs())
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
            if (((Joueur.EstEnAttaque && Joueur.EstEnCollision(this)) || ProjectileInRange() && g.NextFloat() <= P_BLOQUER_D) && !EstDansLesAirs())
                base.Bloquer();
        }
        private bool ProjectileInRange()
        {
            foreach (GameComponent g in Game.Components)
            {
                if (g is Projectile)
                {
                    //if ((g as Projectile).EstEnCollision(SphèreDeRéaction) && (g as Projectile).NumPlayer != this.NumManette)
                    //    return true;
                }
            }
            return false;
        }
        private void Fuite()
        {

        }
        #endregion

        #region Méthodes offensive.
        private void Attaquer()
        {
            SeDéplacerSelonLeChemin();
            if (HitBox.Intersects(Joueur.HitBox) && g.NextFloat() <= P_FRAPPER_D)
            {
                GérerAttaque();
            }
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
        }

        private void PathFind(Vector3 départ)
        {
            NodeJoueur = CalculerNodeLePlusProche(Joueur.GetPositionPersonnage, GrapheDéplacements.GetGrapheComplet());
            NodeBot = CalculerNodeLePlusProche(départ, GrapheDéplacements.GetGrapheComplet());

            Path.A_Star(NodeBot, NodeJoueur);
            CheminLePlusCourt = Path.CopierChemin();
        }

        private void SeDéplacerSelonLeChemin()
        {
            TargetNode = CheminLePlusCourt[0];
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
            ÉTAT_PERSO = ÉTAT.COURRIR;
            if (VecteurVitesse.Y != 0)
                Position += 0.1f * Vector3.Right;
            else
                Position += 0.1f * Vector3.Right;
        }
        protected override void Gauche()
        {
            DIRECTION = ORIENTATION.GAUCHE;
            ÉTAT_PERSO = ÉTAT.COURRIR;
            if (VecteurVitesse.Y != 0)
                Position -= 0.1f * Vector3.Right;
            else
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

        #region Gérant la transition d'états.
        #endregion

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
            return CheminLePlusCourt.Count == 0 || (!EstDansLesAirs() && CalculerNodeLePlusProche(Joueur.Position,GrapheDéplacements.GetGrapheComplet()).Index != NodeJoueur.Index);
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
