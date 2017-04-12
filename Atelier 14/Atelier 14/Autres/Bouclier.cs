using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtelierXNA.Autres;
using AtelierXNA.Éléments_Tuile;

namespace AtelierXNA.Autres
{
    public class Bouclier : PrimitiveDeBaseAnimée
    {
        
        #region NOUVEAU CODE HORS DE SPHÈRETEXTURÉE.
        //Propriétées existantes expressement pour le bouclier.
        Color COULEUR = Color.Red;

        Color Couleur { get; set; }
        float DommageAbsorbé { get; set; }
        #endregion
      
        #region Propriétés et initialisation.
        //Caractéristiques de la sphère.

        public BoundingSphere SphèreDeCollision { get; private set; }         
        protected Vector3 Origine { get; set; } 
        public float Rayon { get; private set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        float IntervalleMAJ { get; set; }
        Texture2D TextureSphère { get; set; }

        //Propriétés relatives au découpage et à la préparation à l'affichage
        //de la sphère.
        Vector2 Deltas { get; set; }
        Vector2 Charpente { get; set; }
        VertexPositionTexture[] Sommets { get; set; }
        Vector2[,] PtsTexture { get; set; }
        Vector3[,] PtsEspace { get; set; }

        //Autres propriétés.
        string NomTexture { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        BasicEffect EffetDeBase { get; set; }

        public Bouclier(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, float rayon, Vector2 charpente, string nomTexture, float intervalleMAJ)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, intervalleMAJ)
        {
            IntervalleMAJ = intervalleMAJ;
            Rayon = rayon;
            Charpente = charpente;
            NomTexture = nomTexture;
            Deltas = Vector2.Zero;
            NbTriangles = (int)(Charpente.X * Charpente.Y) * 2;
            NbSommets = NbTriangles * 3;
        }
        public override void Initialize()
        {
            SphèreDeCollision = new BoundingSphere(PositionInitiale, Rayon);
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            TextureSphère = GestionnaireDeTextures.Find(NomTexture);
            Sommets = new VertexPositionTexture[NbSommets];
            PtsTexture = new Vector2[(int)Charpente.Y + 1, (int)Charpente.X + 1];
            PtsEspace = new Vector3[(int)Charpente.Y + 1, (int)Charpente.X + 1];
            Deltas = new Vector2(TextureSphère.Width / Charpente.X, TextureSphère.Height / Charpente.Y);     
            InitialiserPtsTexture();
            InitialiserPtsEspace();
            base.Initialize();
            EffetDeBase = new BasicEffect(GraphicsDevice);
            InitialiserParamètresEffetDeBase();
        }

        private void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureSphère;
        }
        #endregion

        #region Méthodes en lien avec le calcul des sommets du polygone.
        protected override void InitialiserSommets()
        {
            int i = -1;
            for (int l = 0; l < Charpente.Y; ++l)
                for (int c = 0; c < Charpente.X; ++c)
                {
                    Sommets[++i] = new VertexPositionTexture(PtsEspace[l, c], PtsTexture[l, c]);
                    Sommets[++i] = new VertexPositionTexture(PtsEspace[l, c + 1], PtsTexture[l, c + 1]);
                    Sommets[++i] = new VertexPositionTexture(PtsEspace[l + 1, c], PtsTexture[l + 1, c]);

                    Sommets[++i] = new VertexPositionTexture(PtsEspace[l + 1, c], PtsTexture[l + 1, c]);
                    Sommets[++i] = new VertexPositionTexture(PtsEspace[l, c + 1], PtsTexture[l, c + 1]);
                    Sommets[++i] = new VertexPositionTexture(PtsEspace[l + 1, c + 1], PtsTexture[l + 1, c + 1]);
                }
        }
        protected void InitialiserPtsTexture()
        {
            float deltaX = 1 / (float)Charpente.X;
            float deltaY = 1 / (float)Charpente.Y;

            for (int l = 0; l <= Charpente.Y; ++l)
                for (int c = 0; c <= Charpente.X; ++c)
                {
                    PtsTexture[l, c] = new Vector2(c * deltaX, l * deltaY);
                }
        }
        protected void InitialiserPtsEspace()
        {
            float x;
            float z;
            float y;
            for (int l = 0; l <= Charpente.Y; ++l)
                for (int c = 0; c <= Charpente.X; ++c)
                {
                    x = Origine.X + Rayon * (float)Math.Sin(l * MathHelper.Pi / Charpente.Y) * (float)Math.Cos(c * MathHelper.TwoPi / Charpente.X);
                    z = Origine.Z + Rayon * (float)Math.Sin(l * MathHelper.Pi / Charpente.Y) * (float)Math.Sin(c * MathHelper.TwoPi / Charpente.X);
                    y = Origine.Y + Rayon * (float)Math.Cos(l * MathHelper.Pi / Charpente.Y);

                    PtsEspace[l, c] = new Vector3(x, y, z);
                }
        }
        #endregion
     public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {
                Rayon = MathHelper.Max(Rayon - 0.01f,0);
                Initialize();
                if(DommageAbsorbé != 0)
                {
                    Rayon = Rayon - DommageAbsorbé / 10;
                    Initialize(); 
                    DommageAbsorbé = 0;
                }
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
        #region Affichage
        public override void Draw(GameTime gameTime)
        {
            EffetDeBase.World = GetMonde();
            EffetDeBase.View = CaméraJeu.Vue;
            EffetDeBase.Projection = CaméraJeu.Projection;
            EffetDeBase.Alpha = 0.3f;

            foreach (EffectPass passeEffet in EffetDeBase.CurrentTechnique.Passes)
            {
                passeEffet.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, Sommets, 0, NbTriangles);
            }

        }
        #endregion

        #region Booléens de la classe.
        public void EncaisserDégâts(Personnage p)
        {
            DommageAbsorbé = p.DommageAttaque;
        }
        public void EncaisserDégâts(Projectile p)
        {
            DommageAbsorbé = p.Dégat;
        }
        public bool EstEnCollision(Personnage p)
        {
            return SphèreDeCollision.Intersects(p.HitBox);
        }
        public bool EstEnCollision(Projectile p)
        {
            return SphèreDeCollision.Intersects(p.SphèreDeCollision);
        }
        #endregion

    }
}
