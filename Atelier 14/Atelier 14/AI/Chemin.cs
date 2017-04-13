using AtelierXNA.AI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA.AI
{
    public class Chemin
    {
        //https://www.youtube.com/watch?v=KNXfSOx4eEE, https://en.wikipedia.org/wiki/A*_search_algorithm
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
        public void A_Star(Node départ, Node arrivée)
        {
            Départ = départ;
            Arrivée = arrivée;
            InitialiserOpenList();


            foreach (Node n in GrapheComplet.GrapheComplet)//Heuristic calculation.
                n.CalculerH(Arrivée);

            Départ.G = 0;
            Départ.F = Départ.G + Départ.H;

            while (OpenList.Count != 0)
            {
                Node current = OpenList.OrderBy(n => n.F).First();

                if(current == Arrivée)
                    CheminLePlusCourt = ReconstruireChemin(current); 

                OpenList.Remove(current);
                ClosedList.Add(current);

                for (int i = 0; i < GrapheComplet.MatriceAdjacence.GetLength(0); ++i)//foreach neighbors of current
                {
                    if (GrapheComplet.MatriceAdjacence[current.Index, i] == 1)
                    {
                        if (ClosedList.Find(p => p.Index == i) != null)
                            continue;//Ignore the neighbor which is already evaluated.

                        float tentative_gScore = current.G + CalculerG(current, OpenList.Find(p => p.Index == i));

                        if (OpenList.First(n => n.Index == i) == null)//Trouver un nouveaux node.
                            OpenList.Add(ClosedList.First(p => p.Index == i));
                        else if (tentative_gScore >= OpenList.First(n => n.Index == i).G)
                            continue;

                        OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].CameFrom = current;
                        OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].G = tentative_gScore;
                        OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].F = OpenList.First(n => n.Index == i).G + OpenList.First(n => n.Index == i).H;
                    }
                }
            }
        }
        private void InitialiserOpenList()
        {
            for (int i = 0; i < GrapheComplet.MatriceAdjacence.GetLength(0); ++i)
                if (GrapheComplet.MatriceAdjacence[Départ.Index, i] == 1)
                    OpenList.Add(GrapheComplet.GrapheComplet.First(n => n.Index == i));
        }
        private List<Node> ReconstruireChemin(Node current)
        {
            List<Node> chemin = new List<Node>();
            Node evaluated = current;

            while(evaluated != null)
            {
                chemin.Add(evaluated);
                evaluated = evaluated.CameFrom;
            }

            return chemin;
        }
        private float CalculerG(Node current,Node voisin)
        {
           return Vector3.Distance(current.Position, voisin.Position);
        }
    }
}
