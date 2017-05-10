using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA.Éléments_Tuile
{
    public class TuileTexturéeAnimée : TuileTexturée
    {
        string état;
        private void CalculerÉtatNum()
        {
            int cpt = -1;
            string redneck = "";
            while (redneck != État)
            {
                ++cpt;
                redneck = NomsSprites[cpt];
            }
            ÉtatNum = cpt;
        }
        const int NB_ANIMATIONS = 10;
        public int CptFrame { get; private set; }
        string[] NomsSprites { get; set; }
        public string État
        {
            get
            {
                return état;
            }
            private set
            {
                état = value;
                CalculerÉtatNum();
            }
        }//L'état dans lequel se trouve le personnage. Donne le nom du sprite à utiliser.
        protected int ÉtatNum { get; set; }//Ce même état sous forme numérique (indice du tableau de NomsSprites).
        string TypePersonnage { get; set; }
        int[] NbFramesSprites { get; set; }
        Vector2 DimensionsZoneAffichage { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        List<List<Texture2D>> Textures { get; set; }
        Texture2D TextureCourante { get; set; }
        VertexPositionTexture[] Sommets { get; set; }
        Vector2[,] PtsTexture { get; set; }
        BlendState GestionAlpha { get; set; }
        Vector3 p;
        public Vector3 PositionÀModifier
        {
            get { return p; }

            set
            {
                p = value;
                CalculerMatriceMonde();
            }
        }
        protected override void CalculerMatriceMonde()
        {

            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(1);
            Monde *= Matrix.CreateFromYawPitchRoll(0, 0, 0);
            Monde *= Matrix.CreateTranslation(PositionÀModifier);
        }

        float IntervalleMAJAnimation { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }

        public TuileTexturéeAnimée(Game jeu, float homothétieInitiale, Vector3 rotationInitiale, Vector3 positionInitiale, Vector2 étendue, string nomTextureTuile, float intervalleMAJ, Vector2 dimensionsZoneAffichage, string[] nomsSprites, int[] nbFramesSprites, string typePersonnage, float intervalleMAJAnimation)
            : base(jeu, homothétieInitiale, rotationInitiale, positionInitiale, étendue, nomTextureTuile, intervalleMAJ)
        {
            PositionÀModifier = positionInitiale;
            DimensionsZoneAffichage = dimensionsZoneAffichage;
            NomsSprites = nomsSprites;
            TypePersonnage = typePersonnage;
            NbFramesSprites = nbFramesSprites;
            IntervalleMAJAnimation = intervalleMAJAnimation;
            État = NomsSprites[4];
        }


        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += tempsÉcoulé;

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJAnimation)
            {
                ++CptFrame;
                if (CptFrame == NbFramesSprites[ÉtatNum])
                {
                    CptFrame = 0;
                }
                TextureCourante = Textures[ÉtatNum][CptFrame];
                InitialiserParamètresEffetDeBase();
                TempsÉcouléDepuisMAJ = 0;
            }
        }

        public void ChangerÉtat(string état)
        {
            if (état != État)
            {
                État = état;
                CptFrame = 0;
                TextureCourante = Textures[ÉtatNum][CptFrame];
                InitialiserParamètresEffetDeBase();
            }
        }

        protected override void CréerTableauSommets()
        {
            PtsTexture = new Vector2[2, 2];
            CréerTableauPointsTexture();
            Sommets = new VertexPositionTexture[NbSommets];
        }

        private void CréerTableauPointsTexture()
        {
            PtsTexture[0, 0] = new Vector2(0, 1);
            PtsTexture[1, 0] = new Vector2(1, 1);
            PtsTexture[0, 1] = new Vector2(0, 0);
            PtsTexture[1, 1] = new Vector2(1, 0);
        }

        protected override void InitialiserSommets()
        {
            int NoSommet = -1;
            for (int j = 0; j < 1; ++j)
            {
                for (int i = 0; i < 2; ++i)
                {
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j], PtsTexture[i, j]);
                    Sommets[++NoSommet] = new VertexPositionTexture(PtsSommets[i, j + 1], PtsTexture[i, j + 1]);
                }
            }
        }

        protected override void CréerTableauPoints()
        {
            InitialiserPtsSommets();
        }

        public void DéplacerTuile(Vector3 nouvellePosition)
        {
            PositionÀModifier = nouvellePosition;
        }

        private void InitialiserPtsSommets()
        {
            PtsSommets[0, 0] = new Vector3(0 - DimensionsZoneAffichage.X / 2, 0, 0);
            PtsSommets[1, 0] = new Vector3(0 + DimensionsZoneAffichage.X / 2, 0, 0);
            PtsSommets[0, 1] = new Vector3(0 - DimensionsZoneAffichage.X / 2, 0 + DimensionsZoneAffichage.Y, 0);
            PtsSommets[1, 1] = new Vector3(0 + DimensionsZoneAffichage.X / 2, 0 + DimensionsZoneAffichage.Y, 0);
            InitialiserSommets();
        }

        protected override void LoadContent()
        {
            Textures = new List<List<Texture2D>>();
            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            if (TypePersonnage == "Ninja")
            {
                LoadContentNinja();
            }
            else if (TypePersonnage == "Robot")
            {
                LoadContentRobot();
            }
            //Loader les textures dans la liste de liste de textures.
            base.LoadContent();
        }

        private void LoadContentNinja()
        {
            Texture2D Frame;
            List<Texture2D> FramesAnimation;
            for (int j = 0; j <= NB_ANIMATIONS; ++j)
            {
                FramesAnimation = new List<Texture2D>();
                for (int i = 0; i < NbFramesSprites[j]; ++i)
                {
                    Frame = GestionnaireDeTextures.Find(NomsSprites[j] + i);
                    FramesAnimation.Add(Frame);
                }
                Textures.Add(FramesAnimation);
            }
            TextureCourante = Textures[4][0];
        }

        private void LoadContentRobot()
        {
            Texture2D Frame;
            List<Texture2D> FramesAnimation;
            for (int j = 0; j <= NB_ANIMATIONS; ++j)
            {
                FramesAnimation = new List<Texture2D>();
                for (int i = 1; i <= NbFramesSprites[j]; ++i)
                {
                    Frame = GestionnaireDeTextures.Find(NomsSprites[j] + "(" + i.ToString() + ")");
                    FramesAnimation.Add(Frame);
                }
                Textures.Add(FramesAnimation);
            }
            TextureCourante = Textures[4][0];
        }

        public override void Mirroir()
        {
            base.Mirroir();
        }

        protected override void InitialiserParamètresEffetDeBase()
        {
            EffetDeBase.TextureEnabled = true;
            EffetDeBase.Texture = TextureCourante;
            GestionAlpha = BlendState.AlphaBlend;
        }

        public override void Draw(GameTime gameTime)
        {
            BlendState oldBlendState = GraphicsDevice.BlendState;
            RasterizerState oldRasterizerState = GraphicsDevice.RasterizerState;
            GraphicsDevice.BlendState = GestionAlpha;
            RasterizerState ÉtatRasterizer = new RasterizerState();
            ÉtatRasterizer.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = ÉtatRasterizer;
            base.Draw(gameTime);
            GraphicsDevice.BlendState = oldBlendState;
            GraphicsDevice.RasterizerState = oldRasterizerState;
        }

        protected override void DessinerTriangleStrip()
        {
            GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, Sommets, 0, NB_TRIANGLES);
        }
    }
}
