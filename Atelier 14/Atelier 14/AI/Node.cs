using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA.AI
{
    public class Node
    {
        //Données de base.
        Vector3 Position { get; set; }
        public Vector3 GetPosition()
        {
            return new Vector3(Position.X, Position.Y, Position.Z);
        }
        public int Index { get; set; }
        public float H { get; set; }//Reste constant durant toute l'exécution.
        public float G { get; set; }
        public float F { get; set; }
        public Node CameFrom { private get; set; }
        public Node GetCameFrom()
        {
            if (CameFrom != null)
                return new Node(CameFrom.GetPosition(), CameFrom.Index);
            else
                return null;

        }

        public Node(Vector3 position, int index)
        {
            Index = index;
            Position = position;
        }
        public bool EstAdjacent(Node n,List<Vector3> intervalles)//À DÉFINIR, DIFFICILE À DÉFINIR//
        {
            //(intervalle.X <= position.X) && (intervalle.Y >= position.X);

            bool estAdjacent = false;
            int cpt = 0;
            float extrémiter = 0; // 0 : pas extremiter, 1 : extremiter gauche, 2 : extremiter droite 
            foreach(Vector3 v in intervalles)
            {
                if(Position.Y == intervalles[cpt].Z)
                {
                    if (Position.X == intervalles[cpt].X)
                    {
                        extrémiter = 1;
                    }
                    else if(Position.X == intervalles[cpt].Y)
                    {
                        extrémiter = 2;
                    }
                }
                cpt++;                       
            }
            if (Vector3.Distance(n.Position, Position) <= 25)
            {
                if (extrémiter == 0)
                    estAdjacent = Position.Y <= n.Position.Y;
                if (extrémiter == 1)
                    estAdjacent = Position.Y <= n.Position.Y || Position.X > n.Position.X;
                if(extrémiter == 2)
                    estAdjacent = Position.Y <= n.Position.Y || Position.X < n.Position.X;
            }
            return estAdjacent;
        }
        public void CalculerH(Node arrivée)
        {
            H = Math.Abs(arrivée.Position.X - Position.X) + Math.Abs(arrivée.Position.Y - Position.Y);
        }

    }
}
