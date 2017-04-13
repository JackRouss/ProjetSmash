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
       public Vector3 Position { get; private set; }
       public int Index { get; set; }
       public float H { get; set; }//Reste constant durant toute l'exécution.
       public float G { get; set; }
       public float F { get; set; }
       public Node CameFrom { get; set; }

        public Node(Vector3 position, int index)
        {
            Index = index;
            Position = position;
        }
        public bool EstAdjacent(Node n)//À DÉFINIR, DIFFICILE À DÉFINIR//
        {
            return (Vector3.Distance(n.Position, Position) <= 10 );
        }
        public void CalculerH(Node arrivée)
        {
            H = Math.Abs(arrivée.Position.X - Position.X)+Math.Abs(arrivée.Position.Y - Position.Y);
        }

    }
}
