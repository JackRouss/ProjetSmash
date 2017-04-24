using AtelierXNA.AI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA.AI
{
    public class Graphe
    {
        List<Node> GrapheComplet { get; set; }
        public int[,] MatriceAdjacence { get;  private set; }

        //public int this[int r, int l]
        //{
        //    get
        //    {
        //        return MatriceAdjacence[r, l];
        //    }
        //}
        public Graphe(Map carte)
        {
            InitialiserGraphe(carte.Plateformes, carte.Nodes);
        }
        public List<Node> GetGrapheComplet()
        {
            List<Node> liste = new List<Node>();
            foreach(Node n in GrapheComplet)
            {
                Node q = new Node(n.GetPosition(),n.Index);
                liste.Add(q);
            }
            return liste;
        }
        void InitialiserGraphe(List<Plaquette> plaquettes, List<Node> nodesCarte)
        {
            GrapheComplet = new List<Node>();
            int cpt = 0;
            foreach(Plaquette p in plaquettes)
            {
                foreach(Node node in p.Nodes)
                {
                    node.Index = cpt;
                    GrapheComplet.Add(node);
                    ++cpt;
                }
            }
            foreach (Node node in nodesCarte)
            {
                node.Index = cpt;
                GrapheComplet.Add(node);
                ++cpt;
            }

            MatriceAdjacence = new int[GrapheComplet.Count,GrapheComplet.Count];
            for (int i = 0; i < MatriceAdjacence.GetLength(0); ++i)//Pour chaque rangée
                for (int j = 0; j < MatriceAdjacence.GetLength(1); ++j)//Pour chaque colonne.
                    MatriceAdjacence[i, j] = GrapheComplet.First(t => t.Index == i).EstAdjacent(GrapheComplet.First(t => t.Index == j)) ? 1 : 0;
        }
        public void CalculerH(Node arrivée)
        {
            foreach(Node n in GrapheComplet)
            {
                n.CalculerH(arrivée);
            }
        }
    }
}
