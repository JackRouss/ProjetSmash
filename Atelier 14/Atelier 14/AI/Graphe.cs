using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AtelierXNA.AI
{
    public class Graphe
    {
        #region Propriétés, constantes et initialisation.
        public const float DISTANCE_MAX = 25f;
        public const float DISTANCE_MIN = 10f;
        List<Node> GrapheComplet { get; set; }
        public List<Node> GetGrapheComplet()
        {
            List<Node> liste = new List<Node>();
            foreach (Node n in GrapheComplet)
            {
                Node q = new Node(n);
                liste.Add(q);
            }
            return liste;
        }
        List<Vector3> Intervalles { get; set; }
        public int[,] MatriceAdjacence { get; private set; }
        public int this[int i, int j] { get { return MatriceAdjacence[i, j]; } }
        public Graphe(Map carte)
        {
            Intervalles = new List<Vector3>(carte.IntervallesSurfaces);
            InitialiserGraphe(carte.Plateformes, carte.Nodes);
        }
        void InitialiserGraphe(List<Plaquette> plaquettes, List<Node> nodesCarte)
        {
            GrapheComplet = new List<Node>();
            int cpt = 0;
            int cptPlaquette = 0;


            AjouterNodesPlaquettes(ref cpt, ref cptPlaquette, plaquettes);
            ++cptPlaquette;
            AjouterNodesCartePrincipale(ref cpt, ref cptPlaquette, nodesCarte);
            MatriceAdjacence = new int[GrapheComplet.Count, GrapheComplet.Count];
            RelierNodes();
        }
        /// <summary>
        /// Cette méthode ajoute les nodes des petites plaquettes au GrapheComplet.
        /// </summary>
        /// <param name="cpt"></param>
        /// <param name="cptPlaquette"></param>
        /// <param name="plaquettes"></param>
        private void AjouterNodesPlaquettes(ref int cpt, ref int cptPlaquette, List<Plaquette> plaquettes)
        {
            foreach (Plaquette p in plaquettes)
            {
                ++cptPlaquette;
                foreach (Node node in p.Nodes)
                {
                    node.Index = cpt;
                    node.DonnerNomPlaquette(cptPlaquette);
                    GrapheComplet.Add(node);
                    ++cpt;
                }
            }
        }
        /// <summary>
        /// Cette méthode ajoute les nodes de la carte principale au GrapheComplet.
        /// </summary>
        /// <param name="cpt"></param>
        /// <param name="cptPlaquette"></param>
        /// <param name="nodesCarte"></param>
        private void AjouterNodesCartePrincipale(ref int cpt, ref int cptPlaquette, List<Node> nodesCarte)
        {
            //Pour la carte principale.
            foreach (Node node in nodesCarte)
            {
                node.Index = cpt;
                node.DonnerNomPlaquette(cptPlaquette);
                GrapheComplet.Add(node);
                ++cpt;
            }
        }
        /// <summary>
        /// Cette fonction créer la matrice d'adjacence pour tout les nodes.
        /// </summary>
        private void RelierNodes()
        {
            for (int i = 0; i < MatriceAdjacence.GetLength(0); ++i)
                for (int j = 0; j < MatriceAdjacence.GetLength(1); ++j)
                {
                    Node nodeActuelle = GrapheComplet.First(t => t.Index == i);
                    Node nodeVerifier = GrapheComplet.First(t => t.Index == j);

                    MatriceAdjacence[i, j] = nodeActuelle.NomPlaquette == nodeVerifier.NomPlaquette ? 1 : 0;
                    if (MatriceAdjacence[i, j] != 1 && Vector3.Distance(nodeActuelle.GetPosition(), nodeVerifier.GetPosition()) <= DISTANCE_MAX)
                    {
                        if ((nodeActuelle.EstExtremiterGauche))
                        {
                            MatriceAdjacence[i, j] = (nodeVerifier.GetPosition().Y <= nodeActuelle.GetPosition().Y && nodeVerifier.GetPosition().X < nodeActuelle.GetPosition().X) ? 1 : 0;
                        }
                        else if (nodeActuelle.EstExtremiterDroite)
                        {
                            MatriceAdjacence[i, j] = (nodeVerifier.GetPosition().Y <= nodeActuelle.GetPosition().Y && nodeVerifier.GetPosition().X > nodeActuelle.GetPosition().X) ? 1 : 0;
                        }
                        else
                            MatriceAdjacence[i, j] = nodeActuelle.GetPosition().Y <= nodeVerifier.GetPosition().Y ? 1 : 0;
                    }
                }
        }
        /// <summary>
        /// Cette fonction calcule le H du Node, qui sera une valeur constante pour un trajet du A*.
        /// Est appeler à chaque appel du A*.
        /// </summary>
        /// <param name="arrivée"></param>
        public void CalculerH(Node arrivée)
        {
            foreach (Node n in GrapheComplet)
            {
                n.CalculerH(arrivée);
            }
        }
        #endregion
    }
}
