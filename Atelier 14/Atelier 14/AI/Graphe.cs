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
        const int DISTANCE_MAX = 25;
        List<Node> GrapheComplet { get; set; }
        List<Vector3> Intervalles { get; set; }
        public int[,] MatriceAdjacence { get; private set; }
        public Graphe(Map carte)
        {
            Intervalles = new List<Vector3>(carte.IntervallesSurfaces);
            InitialiserGraphe(carte.Plateformes, carte.Nodes);
            
        }
        public List<Node> GetGrapheComplet()
        {
            List<Node> liste = new List<Node>();
            foreach (Node n in GrapheComplet)
            {
                Node q = new Node(n.GetPosition(), n.Index);
                liste.Add(q);
            }
            return liste;
        }
        void InitialiserGraphe(List<Plaquette> plaquettes, List<Node> nodesCarte)
        {
            GrapheComplet = new List<Node>();
            int cpt = 0;
            int cptPlaquette = -1;

            foreach (Plaquette p in plaquettes)
            {
                cptPlaquette++;
                foreach (Node node in p.Nodes)
                {
                    node.Index = cpt;
                    GrapheComplet.Add(node);
                    ++cpt;
                    node.DonnéNomPlaquette(cptPlaquette);
                }
            }

            cptPlaquette++;
            foreach (Node node in nodesCarte)
            {
                node.Index = cpt;
                GrapheComplet.Add(node);
                ++cpt;
                node.DonnéNomPlaquette(cptPlaquette);
            }

            MatriceAdjacence = new int[GrapheComplet.Count, GrapheComplet.Count];
            RelierNode(plaquettes);
        }
        void RelierNode(List<Plaquette> plaquettes)
        {
            for (int i = 0; i < MatriceAdjacence.GetLength(0); ++i)//Pour chaque rangée
                for (int j = 0; j < MatriceAdjacence.GetLength(1); ++j)//Pour chaque colonne.
                {
                    Node nodeActuelle = GrapheComplet.First(t => t.Index == i);
                    Node nodeVerifier = GrapheComplet.First(t => t.Index == j);
                    MatriceAdjacence[i, j] = nodeActuelle.NomPlaquette == nodeVerifier.NomPlaquette ? 1 : 0;
                    if(MatriceAdjacence[i,j] != 1 && Vector3.Distance(nodeActuelle.GetPosition(), nodeVerifier.GetPosition()) <= DISTANCE_MAX)
                    {
                        if(nodeActuelle.EstExtremiterGauche || nodeVerifier.EstExtremiterGauche)
                            MatriceAdjacence[i, j] = (nodeVerifier.GetPosition().Y <= nodeActuelle.GetPosition().Y && nodeVerifier.GetPosition().X <= nodeActuelle.GetPosition().X) ? 1 : 0;
                        else if(nodeActuelle.EstExtremiterDroite || nodeVerifier.EstExtremiterDroite)
                            MatriceAdjacence[i, j] = (nodeVerifier.GetPosition().Y <= nodeActuelle.GetPosition().Y && nodeVerifier.GetPosition().X >= nodeActuelle.GetPosition().X) ? 1 : 0;
                        else
                             MatriceAdjacence[i, j] = nodeActuelle.GetPosition().Y <= nodeVerifier.GetPosition().Y ? 1 : 0;
                    }
                    
                }
                    

        }
     
        public void CalculerH(Node arrivée)
        {
            foreach (Node n in GrapheComplet)
            {
                n.CalculerH(arrivée);
            }
        }
    }
}
