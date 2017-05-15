using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtelierXNA.AI;

namespace AtelierXNA
{

    public class Plaquette : PrimitiveDeBase
    {
        public const float LONGUEUR = 20f;
        public const float LARGEUR = 20f;
        const int NB_NODES = 6;
        const int NB_TRIANGLE_SURFACE = 2;
        const int NB_TRIANGLE_BASE = 8;
        const int NB_SOMMETS_LIST = 3;
        const int coeff_Surface = 2;
        public const int HAUTEUR = 1;

        float Largeur { get; set; }
        float Longueur { get; set; }
        public Vector3 IntervallesSurfaces{ get; private set; }
        public List<Node> Nodes { get; set; }



        Vector3 Origine { get; set; }
        public Vector3 Position { get; private set; }
        Vector3[] PtsSommets { get; set; }
        VertexPositionColor[] Sommets { get; set; }
        VertexPositionColor[] SommetsBase { get; set; }
        public BoundingBox Hitbox { get; private set; }
        BasicEffect EffetDeBase { get; set; }
        Color Couleur { get; set; }


        public Plaquette(Game game, float homothetie, Vector3 rotationInitiale, Vector3 position, Color couleur)
            : base(game, homothetie, rotationInitiale, position) { Position = position; Couleur = couleur; }

        public override void Initialize()
        {
            Hitbox = new BoundingBox(new Vector3(Position.X - Longueur / coeff_Surface, Position.Y - HAUTEUR, Position.Z - 10), new Vector3(Position.X + Longueur / coeff_Surface, Position.Y + HAUTEUR, Position.Z + 10));
            DrawOrder = 3;
            Longueur = LONGUEUR;
            Largeur = LARGEUR;
            Origine = Vector3.Zero;
            InitialiserPtsSommets();
            InitialiserSommets();
            CalculerPropriétésPourPersonnages();

            base.Initialize();
        }
        void InitialiserPtsSommets()
        {
            // Pas besoin de faire le coter arriere car la camera va juste voir lavant et peu etre les coter
            PtsSommets = new Vector3[(NB_TRIANGLE_SURFACE + NB_TRIANGLE_BASE) * NB_SOMMETS_LIST];

            //Plaque Du dessus
            PtsSommets[0] = new Vector3(Origine.X - Longueur / coeff_Surface, Origine.Y, Origine.Z - Largeur / coeff_Surface);
            PtsSommets[1] = new Vector3(Origine.X + Longueur / coeff_Surface, Origine.Y, Origine.Z + Largeur / coeff_Surface);
            PtsSommets[2] = new Vector3(Origine.X - Longueur / coeff_Surface, Origine.Y, Origine.Z + Largeur / coeff_Surface);
            PtsSommets[3] = PtsSommets[0];
            PtsSommets[4] = new Vector3(Origine.X + Longueur / coeff_Surface, Origine.Y, Origine.Z - Largeur / coeff_Surface);
            PtsSommets[5] = PtsSommets[1];

            //Plaque du dessous
            PtsSommets[6] = new Vector3(Origine.X - Longueur / coeff_Surface, Origine.Y - HAUTEUR, Origine.Z - Largeur / coeff_Surface);
            PtsSommets[7] = new Vector3(Origine.X - Longueur / coeff_Surface, Origine.Y - HAUTEUR, Origine.Z + Largeur / coeff_Surface);
            PtsSommets[8] = new Vector3(Origine.X + Longueur / coeff_Surface, Origine.Y - HAUTEUR, Origine.Z + Largeur / coeff_Surface);
            PtsSommets[9] = PtsSommets[6];
            PtsSommets[10] = PtsSommets[8];
            PtsSommets[11] = new Vector3(Origine.X + Longueur / coeff_Surface, Origine.Y - HAUTEUR, Origine.Z - Largeur / coeff_Surface);

            //Coter Face
            PtsSommets[12] = PtsSommets[7];
            PtsSommets[13] = PtsSommets[2];
            PtsSommets[14] = PtsSommets[8];
            PtsSommets[15] = PtsSommets[2];
            PtsSommets[16] = PtsSommets[1];
            PtsSommets[17] = PtsSommets[8];

            //Coter Droit
            PtsSommets[18] = PtsSommets[1];
            PtsSommets[19] = PtsSommets[11];
            PtsSommets[20] = PtsSommets[8];
            PtsSommets[21] = PtsSommets[4];
            PtsSommets[22] = PtsSommets[11];
            PtsSommets[23] = PtsSommets[1];

            //Coter Gauche
            PtsSommets[24] = PtsSommets[2];
            PtsSommets[25] = PtsSommets[7];
            PtsSommets[26] = PtsSommets[6];
            PtsSommets[27] = PtsSommets[2];
            PtsSommets[28] = PtsSommets[6];
            PtsSommets[29] = PtsSommets[0];
        }
        void CalculerPropriétésPourPersonnages()
        {
            IntervallesSurfaces = new Vector3(Position.X - Longueur/coeff_Surface,Position.X+Longueur/coeff_Surface,Position.Y);
            Nodes = new List<Node>();
            for (int i = 0; i < NB_NODES; ++i)
            {
                Node UnAutreBidonVilleDeMerde = new Node(new Vector3(IntervallesSurfaces.X + i * (IntervallesSurfaces.Y - IntervallesSurfaces.X) / (NB_NODES - 1), IntervallesSurfaces.Z, Position.Z), i);
                UnAutreBidonVilleDeMerde.EstExtrémitéeGauche = i == 0;
                UnAutreBidonVilleDeMerde.EstExtrémitéeDroite = NB_NODES -1 == i;
                Nodes.Add(UnAutreBidonVilleDeMerde);//Le i est bidon ici; il faut le redéfinir lorsque l'on créé le graphe.
            }
                
        }
        protected override void InitialiserSommets()
        {
            Sommets = new VertexPositionColor[NB_TRIANGLE_SURFACE * NB_SOMMETS_LIST];
            SommetsBase = new VertexPositionColor[NB_TRIANGLE_BASE * NB_SOMMETS_LIST];
            for (int i = 0; i < PtsSommets.Length; ++i)
            {
                if (i > Sommets.Length - 1) // est rendu a la base                
                    SommetsBase[i - Sommets.Length] = new VertexPositionColor(PtsSommets[i], Color.SaddleBrown);
                else
                    Sommets[i] = new VertexPositionColor(PtsSommets[i], Couleur);
            }
        }
        protected override void LoadContent()
        {
            base.LoadContent();
            EffetDeBase = new BasicEffect(GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;
            EffetDeBase.VertexColorEnabled = true;

            RasterizerState ancienÉtat = GraphicsDevice.RasterizerState;
            RasterizerState état = new RasterizerState();
            état.CullMode = CullMode.CullCounterClockwiseFace;
            état.FillMode = GraphicsDevice.RasterizerState.FillMode;
            GraphicsDevice.RasterizerState = état;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, Sommets, 0, NB_TRIANGLE_SURFACE);
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, SommetsBase, 0, NB_TRIANGLE_BASE);
            }
            GraphicsDevice.RasterizerState = ancienÉtat;
        }
    }
}
