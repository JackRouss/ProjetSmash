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
        public List<Node> CheminLePlusCourt { get; private set; }

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
            OpenList.Add(Départ);


            GrapheComplet.CalculerH(Arrivée);

            Départ.F = Départ.H;

            while (OpenList.Count != 0)
            {
                Node current = OpenList.OrderBy(n => n.F).First();


                if (current.Index == Arrivée.Index)
                {
                    CheminLePlusCourt = ReconstruireChemin(current);
                    break;
                }

                OpenList.Remove(current);
                ClosedList.Add(current);

                for (int i = 0; i < GrapheComplet.MatriceAdjacence.GetLength(0); ++i)//pour chaque voisin
                {
                    if (GrapheComplet.MatriceAdjacence[current.Index, i] == 1)
                    {
                        Node neighbor = GrapheComplet.GetGrapheComplet().Find(n => n.Index == i);
                        if (ClosedList.Find(p => p.Index == i) != null)
                            continue;//Ignorer le voisin, il est déjà évalué!

                        float tentative_gScore = current.G + CalculerG(current, neighbor);
                        if (!OpenList.Contains(neighbor))//Trouver un nouveaux node.
                            OpenList.Add(neighbor);
                        else if (tentative_gScore >= OpenList.First(n => n.Index == i).G)
                            continue;

                        OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].CameFrom = current;
                        OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].G = tentative_gScore;
                        OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].F = OpenList.First(n => n.Index == i).G + OpenList.First(n => n.Index == i).H;
                    }
                }
            }


            ClosedList.Clear();
            OpenList.Clear();
        }
        private List<Node> ReconstruireChemin(Node current)
        {
            List<Node> chemin = new List<Node>();
            Node evaluated = current;

            while (evaluated != null)
            {
                chemin.Add(evaluated);
                evaluated = evaluated.GetCameFrom();
            }
            chemin.Add(Départ);//PEUT ÊTRE À ENLEVER
            chemin.Reverse();
            return chemin;
        }
        private float CalculerG(Node current, Node voisin)
        {
            return Vector3.Distance(current.GetPosition(), voisin.GetPosition());
        }
    }
}
