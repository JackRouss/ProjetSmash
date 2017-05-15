using Microsoft.Xna.Framework;

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
        public int Index { get; set; }//Cette propriété n'est pas encapsulée mais c'est nécessaire.
        public float H { get; private set; }
        public float G { get; set; }//Cette propriété n'est pas encapsulée mais c'est nécessaire.
        public float F { get; set; }//Cette propriété n'est pas encapsulée mais c'est nécessaire.
        public bool EstExtrémitéeDroite { get; set; }//Cette propriété n'est pas encapsulée mais c'est nécessaire.
        public bool EstExtrémitéeGauche { get; set; }//Cette propriété n'est pas encapsulée mais c'est nécessaire.
        public int NomPlaquette { get; private set; }
        public Node CameFrom { get; set; }//Cette propriété n'est pas encapsulée mais c'est nécessaire pour le A*. De plus, cette classe n'est utilisée qu'exclkusivement pour le A*.

        /// <summary>
        /// Constructeur de copie.
        /// </summary>
        /// <param name="n"></param>
        public Node(Node n)
        {
            H = n.H;
            G = n.G;
            F = n.F;
            EstExtrémitéeDroite = n.EstExtrémitéeDroite;
            EstExtrémitéeGauche = n.EstExtrémitéeGauche;
            NomPlaquette = n.NomPlaquette;
            Index = n.Index;
            if(n.CameFrom != null)
                CameFrom = new Node(n.CameFrom);
            Position = n.GetPosition();
        }
        public Node(Vector3 position, int index)
        {
            Index = index;
            Position = position;
        }
        /// <summary>
        /// Méthode qui sert à identifier le node à une plaquette. Il est nécessaire de le faire à part de la construction car lors
        /// de la construction des premiers nodes, on ignore 
        /// </summary>
        /// <param name="nomPlaquette"></param>
        public void DonnerNomPlaquette(int nomPlaquette)
        {
            NomPlaquette = nomPlaquette;
        }
        public void CalculerH(Node arrivée)
        {
            H = Vector3.Distance(Position,arrivée.GetPosition());
        }
    }
}
