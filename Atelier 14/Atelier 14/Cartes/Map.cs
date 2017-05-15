﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AtelierXNA.AI;
using AtelierXNA.Autres;


namespace AtelierXNA
{

    public class Map : PrimitiveDeBase, IPause
    {
        const int NB_NODES = 20;
        int[] Seeds = new int[] { 99, 67, 666, 5924725};


        public Vector4 LIMITE_MAP { get; private set; }// (x a droite, x a gauche, y en haut, y en bas)
        public Vector4 LIMITE_PLAQUETTE { get; private set; }
        const int NB_TRIANGLE_SURFACE = 2;
        const int NB_TRIANGLE_BASE = 8;
        const int NB_SOMMETS_LIST = 3;
        const int COEFF_SURFACE = 2;
        const int COEFF_BASE = 4;
        const int HAUTEUR = 15;
        const int HAUTEURBOX = 10;
        const int TRANSLATION_MODELE = 10;
        const float GROSSEUR_MODELE = 0.05f;

        //Propriétés utilisées par le personnage.
        public Vector3 VecteurGauche { get; private set; }
        BoundingBox Hitbox { get; set; }
        public List<Vector3> IntervallesSurfaces { get; private set; }//( MinX, MaxX,Hauteur en Y de la surface )
        public List<Plaquette> Plateformes { get; private set; }
        public List<Node> Nodes { get; private set; }
        int NbPlateformes { get; set; }
        Générateur g { get; set; }

        float Largeur { get; set; }
        float Longueur { get; set; }
        Vector3 Origine { get; set; }
        Vector3[] PtsSommets { get; set; }
        VertexPositionColor[] Sommets { get; set; }
        VertexPositionColor[] SommetsBase { get; set; }
        BasicEffect EffetDeBase { get; set; }
        Color Couleur { get; set; }
        RessourcesManager<Model> GestionnaireDeModele { get; set; }
        Model Arbre { get; set; }


        public Map(Game game, float homothetie, Vector3 rotationInitiale, Vector3 position, Color couleur, int nbPlateformes)
            : base(game, homothetie, rotationInitiale, position)
        {
            g = Game.Services.GetService(typeof(Générateur)) as Générateur;
            Origine = position;
            Couleur = couleur;
            NbPlateformes = nbPlateformes;
                 
            for(int i =0; i < Seeds.Length -1; ++i)
            {
                if(Couleur == Atelier.CouleurCartes[i])
                {
                    g.ModifierSeed(Seeds[i]);
                }            
           }       
        }

        public override void Initialize()
        {
            DrawOrder = 1;
            Longueur = 100f;
            Largeur = 50;
            Hitbox = new BoundingBox(new Vector3(-Longueur / COEFF_SURFACE, -Largeur / COEFF_SURFACE, -HAUTEURBOX), new Vector3(Longueur / COEFF_SURFACE, Largeur / COEFF_SURFACE, HAUTEURBOX));

            AjouterModele3D();        

            LIMITE_MAP = new Vector4(150, -150, 100, -100);
            LIMITE_PLAQUETTE = new Vector4(Origine.X + Longueur / 2, Origine.X - Longueur / 2,(Origine.Y + LIMITE_MAP.Z)/3, 0);
            Plateformes = new List<Plaquette>();
            InitialiserPtsSommets();
            InitialiserSommets();
            IntervallesSurfaces = new List<Vector3>();
            IntervallesSurfaces.Add(new Vector3(PtsSommets[0].X, PtsSommets[4].X, Origine.Y));
            InitialisationPlaquettes();

            CalculerPropriétésPourPersonnages();

            base.Initialize();
        }
        void InitialisationPlaquettes()
        {
            Plateformes.Add(new Plaquette(this.Game, 1, Vector3.Zero, new Vector3(Origine.X - Longueur / 4, Origine.Y + HAUTEUR, Origine.Z), Couleur));
            Plateformes.Add(new Plaquette(this.Game, 1, Vector3.Zero, new Vector3(Origine.X + Longueur / 4, Origine.Y + HAUTEUR, Origine.Z), Couleur));

            foreach (Plaquette p in Plateformes)
            {
                p.Initialize();
            }
            GénérerPlateformes();
            foreach (Plaquette p in Plateformes)
            {
                Game.Components.Add(p);
            }
        }
        void AjouterModele3D()
        {
            Game.Components.Add(new ObjetDeBase(Game, "LP_tree", GROSSEUR_MODELE, Vector3.Zero, new Vector3(-Longueur / 2 + TRANSLATION_MODELE, -2, -Largeur / 2 + TRANSLATION_MODELE)));
            Game.Components.Add(new ObjetDeBase(Game, "LP_tree", GROSSEUR_MODELE, Vector3.Zero, new Vector3(-Longueur / 4 + TRANSLATION_MODELE, -2, -Largeur / 2 + TRANSLATION_MODELE)));
            Game.Components.Add(new ObjetDeBase(Game, "LP_tree", GROSSEUR_MODELE, Vector3.Zero, new Vector3(Longueur / 2 - TRANSLATION_MODELE, -2, -Largeur / 2 + TRANSLATION_MODELE)));
            Game.Components.Add(new ObjetDeBase(Game, "LP_tree", GROSSEUR_MODELE, Vector3.Zero, new Vector3(Longueur / 4 - TRANSLATION_MODELE, -2, -Largeur / 2 + TRANSLATION_MODELE)));
        }
        void GénérerPlateformes()
        {
            float x;
            float y;
            for(int i = 0; i< NbPlateformes; ++i)
            {
                do
                {
                    x = g.GénérerFloatAléatoire(LIMITE_MAP.X + 25, LIMITE_MAP.W-25);
                    y = g.GénérerFloatAléatoire(LIMITE_MAP.Z+25, LIMITE_MAP.Y-25);
                } while (!EmplacementValide(x, y));

                int positifOuNégatif = g.GénérerEntierAléatoire(1, 3);
                if(positifOuNégatif == 2)
                    Plateformes.Add(new Plaquette(this.Game, 1, Vector3.Zero, new Vector3(Origine.X + x, Origine.Y + y, Origine.Z), Couleur));
                else
                    Plateformes.Add(new Plaquette(this.Game, 1, Vector3.Zero, new Vector3(Origine.X - x, Origine.Y + y, Origine.Z), Couleur));
                
            }
        }
        bool EmplacementValide(float x, float y)
        {
            if(Plateformes.Count != 0)
            {
                foreach(Plaquette p in Plateformes)
                {
                    if ((EstDistanceAcceptable(new Vector3(x, y, 0), new Vector3(p.IntervallesSurfaces.X, p.IntervallesSurfaces.Z, 0))))
                    {
                        break;
                    }
                }
               
                foreach(Plaquette p  in Plateformes)
                {
                    if (p.Hitbox.Intersects(new BoundingBox(new Vector3(x - Plaquette.LONGUEUR / 2, y - Plaquette.HAUTEUR, Plaquette.LARGEUR / 2), new Vector3(x + Plaquette.LONGUEUR / 2, y, -Plaquette.LARGEUR / 2))) || y > 50 || y<15)
                     {
                        return false;
                     }
                }
                
                return true;
            }
            if ((EstDistanceAcceptable(new Vector3(x, y, 0), Vector3.Zero) && !Hitbox.Intersects(new BoundingBox(new Vector3(x - Plaquette.LONGUEUR / 2, y - Plaquette.HAUTEUR, Plaquette.LARGEUR / 2), new Vector3(x + Plaquette.LONGUEUR / 2, y, -Plaquette.LARGEUR / 2))) && y <= 50 && y >= 15))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        bool EstDistanceAcceptable(Vector3 v, Vector3 u)
        {
            return Vector3.Distance(v,u) <= 2*Plaquette.LONGUEUR + Graphe.DISTANCE_MAX && Vector3.Distance(v,u) >= 2*Plaquette.LONGUEUR+ Graphe.DISTANCE_MIN;
        }
        void InitialiserPtsSommets()
        {
            // Pas besoin de faire le coter arriere car la camera va juste voir lavant et peu etre les coter
            PtsSommets = new Vector3[(NB_TRIANGLE_SURFACE + NB_TRIANGLE_BASE) * NB_SOMMETS_LIST];

            //Plaque Du dessus
            PtsSommets[0] = new Vector3(Origine.X - Longueur / COEFF_SURFACE, Origine.Y, Origine.Z - Largeur / COEFF_SURFACE);
            PtsSommets[1] = new Vector3(Origine.X + Longueur / COEFF_SURFACE, Origine.Y, Origine.Z + Largeur / COEFF_SURFACE);
            PtsSommets[2] = new Vector3(Origine.X - Longueur / COEFF_SURFACE, Origine.Y, Origine.Z + Largeur / COEFF_SURFACE);
            PtsSommets[3] = PtsSommets[0];
            PtsSommets[4] = new Vector3(Origine.X + Longueur / COEFF_SURFACE, Origine.Y, Origine.Z - Largeur / COEFF_SURFACE);
            PtsSommets[5] = PtsSommets[1];

            //Plaque du dessous
            PtsSommets[6] = new Vector3(Origine.X - Longueur / COEFF_BASE, Origine.Y - HAUTEUR, Origine.Z - Largeur / COEFF_BASE);
            PtsSommets[7] = new Vector3(Origine.X - Longueur / COEFF_BASE, Origine.Y - HAUTEUR, Origine.Z + Largeur / COEFF_BASE);
            PtsSommets[8] = new Vector3(Origine.X + Longueur / COEFF_BASE, Origine.Y - HAUTEUR, Origine.Z + Largeur / COEFF_BASE);
            PtsSommets[9] = PtsSommets[6];
            PtsSommets[10] = PtsSommets[8];
            PtsSommets[11] = new Vector3(Origine.X + Longueur / 3, Origine.Y - HAUTEUR, Origine.Z - Largeur / 3);

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
            
            VecteurGauche = PtsSommets[0] - PtsSommets[4];
            foreach(Plaquette p in Plateformes)
            {
                IntervallesSurfaces.Add(p.IntervallesSurfaces);
            }
            Nodes = new List<Node>();
            for (int i = 0; i < NB_NODES; ++i)
            {
                Node nodeATesterExtremiter = new Node(new Vector3(PtsSommets[0].X + i * (PtsSommets[4].X - PtsSommets[0].X) / (NB_NODES - 1), Origine.Y, Origine.Z), i);
                nodeATesterExtremiter.EstExtremiterGauche = i == 0;
                nodeATesterExtremiter.EstExtremiterDroite = i == NB_NODES - 1;                          
                Nodes.Add(nodeATesterExtremiter);
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
