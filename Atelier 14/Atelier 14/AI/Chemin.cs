using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AtelierXNA.AI
{
    public class Chemin
    {
        #region Propriétés et initialisation.
        //Données initiales.
        Node Départ { get; set; }
        Node Arrivée { get; set; }
        Graphe GrapheComplet { get; set; }

        //Données de manipulation.
        List<Node> ClosedList { get; set; }
        List<Node> OpenList { get; set; }

        //Données sortantes.
        List<Node> CheminLePlusCourt { get; set; }

        public Chemin(Graphe grapheComplet)
        {
            GrapheComplet = grapheComplet;
            ClosedList = new List<Node>();
            OpenList = new List<Node>();
        }
        #endregion

        #region Méthodes servant au calcul du chemin le plus court.
        /// <summary>
        /// Cette méthode prend en intrant deux nodes. Un node de départ et un node d'arrivée, tous les deux membres de GrapheComplet.
        /// À partir de ces deux intrants, elle calcule le chemin le plus court entre ces deux nodes et le stocke dans la liste
        /// CheminLePlusCourt.
        /// </summary>
        /// <param name="départ"></param>
        /// <param name="arrivée"></param>
        public void A_Star(Node départ, Node arrivée)
        {
            InitialiserAÉtoile(départ, arrivée);

            while (OpenList.Count != 0)//Tant qu'il y a des nodes à évaluer.
            {
                Node current = OpenList.OrderBy(n => n.F).First();

                if (current.Index == Arrivée.Index)
                {
                    CheminLePlusCourt = ReconstruireChemin(current);
                    break;
                }

                ÉvaluerTousLesVoisins(ref current);
            }
        }
        /// <summary>
        /// Grosse fonction dont le rôle est de déterminer si les voisins du node actuel sont "intéressants".Le cas échéant, la méthode sauvegarde le voisin dans la OpenList.
        /// Après avoir fait ces opérations, elle place le node actuel dans la ClosedList.
        /// </summary>
        /// <param name="current"></param>
        private void ÉvaluerTousLesVoisins(ref Node current)
        {
            for (int i = 0; i < GrapheComplet.MatriceAdjacence.GetLength(0); ++i)//Pour chacun des voisins.
            {
                if (SontConnectés(current.Index,i))
                {
                    Node neighbor = GrapheComplet.GetGrapheComplet().Find(n => n.Index == i);
                    float tentative_gScore = current.G + CalculerG(current, neighbor);

                    if (EstDansClosed(i) && EstGInférieurDansClose(tentative_gScore, i))
                        continue;

                    if (EstDansOpen(i) && EstGInférieurDansOpen(tentative_gScore, i))
                        continue;
                    else
                        SauvegarderDansOpenList(i, tentative_gScore, ref current,neighbor);
                }
            }
            ClosedList.Add(current);
            OpenList.Remove(current);
        }
        /// <summary>
        /// Sauvegarde un des voisins jugé "intéressant" dans la OpenList.
        /// </summary>
        /// <param name="neighbor"></param>
        /// <param name="tentative_gScore"></param>
        /// <param name="current"></param>
        private void SauvegarderDansOpenList(int i, float tentative_gScore, ref Node current, Node neighbor)
        {
            OpenList.Add(neighbor);
            OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].CameFrom = current;
            OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].G = tentative_gScore;
            OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].F = OpenList.First(n => n.Index == i).G + OpenList.First(n => n.Index == i).H;
        }
        private void InitialiserAÉtoile(Node départ, Node arrivée)
        {
            ClosedList.Clear();
            OpenList.Clear();
            Départ = départ;
            Arrivée = arrivée;
            OpenList.Add(Départ);
            GrapheComplet.CalculerH(Arrivée);
            Départ.F = Départ.H;
        }

        #region Autres Méthodes.
        /// <summary>
        /// Cette fonction permet de prendre le node d'une liste et de retourner à partir de son CameFrom
        /// la liste complète de ses parents, afin de pouvoir reconstruire le chemin au complet.
        /// </summary>
        /// <param name="current"></param>
        /// <returns></returns>
        private List<Node> ReconstruireChemin(Node current)
        {
            List<Node> chemin = new List<Node>();
            Node evaluated = new Node(current);

            while (evaluated != null)
            {
                chemin.Add(evaluated);
                if (evaluated.CameFrom != null)
                    evaluated = new Node(evaluated.CameFrom);
                else
                    evaluated = null;
            }
            if (chemin.Find(t => t.Index == Départ.Index) == null)
                chemin.Add(Départ);

            chemin.Reverse();
            return chemin;
        }
        /// <summary>
        /// Cette fonction permet de copier en profondeur les nodes présents dans la propriété CheminLePlusCourt.
        /// </summary>
        /// <returns></returns>
        public List<Node> CopierChemin()
        {
            List<Node> c = new List<Node>();
            Node b;
            foreach (Node n in CheminLePlusCourt)
            {
                b = new Node(n);
                c.Add(b);
            }
            return c;
        }
        /// <summary>
        /// Cette fonction permet de calculer la distance entre deux nodes adjacents.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="voisin"></param>
        /// <returns></returns>
        private float CalculerG(Node current, Node voisin)
        {
            return Vector3.Distance(current.GetPosition(), voisin.GetPosition());
        }
        #endregion

        #region Booléens.
        private bool EstDansClosed(int indexNode)
        {
            return ClosedList.Find(p => p.Index == indexNode) != null;
        }
        private bool EstDansOpen(int indexNode)
        {
            return OpenList.Find(n => n.Index == indexNode) != null;
        }
        /// <summary>
        /// Cette fonction retourne vrai si le G qui se trouve dans la ClosedList est inférieur au nouveau G.
        /// </summary>
        /// <param name="tentative_gScore"></param>
        /// <param name="indexNode"></param>
        /// <returns></returns>
        private bool EstGInférieurDansClose(float tentative_gScore, int indexNode)
        {
            return ClosedList.Find(p => p.Index == indexNode).G <= tentative_gScore;
        }
        /// <summary>
        /// Cette fonction retourne vrai si le G qui se trouve dans la OpenList est inférieur au nouveau G.
        /// </summary>
        /// <param name="tentative_gScore"></param>
        /// <param name="indexNode"></param>
        /// <returns></returns>
        private bool EstGInférieurDansOpen(float tentative_gScore, int indexNode)
        {
            return OpenList.First(n => n.Index == indexNode).G <= tentative_gScore;
        }
        private bool SontConnectés(int index1, int index2)
        {
            return GrapheComplet.MatriceAdjacence[index1, index2] == 1;
        }
        #endregion
        #endregion
    }
}
