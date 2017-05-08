using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AtelierXNA.AI
{
    public class Chemin
    {
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

            while (OpenList.Count != 0)//Tant qu'il y a des nodes à évaluer.
            {
                Node current = OpenList.OrderBy(n => n.F).First();


                if (current.Index == Arrivée.Index)
                {
                    CheminLePlusCourt = ReconstruireChemin(current);
                    break;
                }


                for (int i = 0; i < GrapheComplet.MatriceAdjacence.GetLength(0); ++i)//pour chaque voisin
                {
                    if (GrapheComplet.MatriceAdjacence[current.Index, i] == 1)
                    {
                        Node neighbor = GrapheComplet.GetGrapheComplet().Find(n => n.Index == i);
                        float tentative_gScore = current.G + CalculerG(current, neighbor);
                        if (ClosedList.Find(p => p.Index == i) != null)//première condition avec coût inférieur.
                        {
                            if(ClosedList.Find(p => p.Index == i).G <= tentative_gScore)
                                continue;//Ignorer le voisin, il est déjà évalué!
                        }
                        if(OpenList.Find(n => n.Index == i) != null)
                        {
                            if(tentative_gScore >= OpenList.First(n => n.Index == i).G)
                                continue;
                        }
                        else
                        {
                            OpenList.Add(neighbor);
                            OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].CameFrom = current;
                            OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].G = tentative_gScore;
                            OpenList[OpenList.IndexOf(OpenList.Find(n => n.Index == i))].F = OpenList.First(n => n.Index == i).G + OpenList.First(n => n.Index == i).H;
                        }
                    }
                    
                }
                ClosedList.Add(current);
                OpenList.Remove(current);
            }

            //On vide les listes pour un prochain appel de la fonction. L'extrant CheminLePlusCourt a été calculé.
            ClosedList.Clear();
            OpenList.Clear();
        }
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
            if(chemin.Find(t => t.Index == Départ.Index) == null)
                chemin.Add(Départ);
            
            chemin.Reverse();
            return chemin;
        }
        public List<Node> CopierChemin()
        {
            List<Node> c = new List<Node>();
            Node b;
            foreach(Node n in CheminLePlusCourt)
            {
                b = new Node(n);
                c.Add(b);
            }
            return c;
        }
        private float CalculerG(Node current, Node voisin)
        {
            return Vector3.Distance(current.GetPosition(), voisin.GetPosition());
        }
    }
}
