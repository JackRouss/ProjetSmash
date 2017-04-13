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
        public List<Node> GrapheComplet { get; private set; }
        public int[,] MatriceAdjacence { get; private set; }

        public Graphe(List<Plaquette> plaquettes)
        {
            InitialiserGraphe(plaquettes);
        }
        void InitialiserGraphe(List<Plaquette> plaquettes)
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

            MatriceAdjacence = new int[GrapheComplet.Count,GrapheComplet.Count];
            for (int i = 0; i < MatriceAdjacence.GetLength(0); ++i)//Pour chaque rangée
                for (int j = 0; j < MatriceAdjacence.GetLength(1); ++j)//Pour chaque colonne.
                    MatriceAdjacence[i, j] = GrapheComplet.First(t => t.Index == i).EstAdjacent(GrapheComplet.First(t => t.Index == j)) ? 1 : 0;
        }
    }
}
