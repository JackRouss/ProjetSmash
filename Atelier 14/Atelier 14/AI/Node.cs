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
        public int Index { get; set; }
        public float H { get; set; }
        public float G { get; set; }
        public float F { get; set; }
        public bool EstExtremiterDroite { get; set; }
        public bool EstExtremiterGauche { get; set; }
        public int NomPlaquette { get; private set; }
        public Node CameFrom { get; set; }
        public Node(Node n)
        {
            H = n.H;
            G = n.G;
            F = n.F;
            EstExtremiterDroite = n.EstExtremiterDroite;
            EstExtremiterGauche = n.EstExtremiterGauche;
            NomPlaquette = n.NomPlaquette;
            Index = n.Index;
            if(n.CameFrom != null)
                CameFrom = new Node(n.CameFrom);
            Position = n.GetPosition();
        }
        BoundingSphere Hitbox { get; set; }
        public Node(Vector3 position, int index)
        {
            Index = index;
            Position = position;
            Hitbox = new BoundingSphere(Position, Bot.DISTANCE_THRESH);
        }
        public void DonnéNomPlaquette(int nomPlaquette)
        {
            NomPlaquette = nomPlaquette;
        }
        public void CalculerH(Node arrivée)
        {
            H = Vector3.Distance(Position,arrivée.GetPosition());
        }
        public bool EstEnCollision(Personnage p)
        {
            return Hitbox.Intersects(p.HitBox);
        }

    }
}
