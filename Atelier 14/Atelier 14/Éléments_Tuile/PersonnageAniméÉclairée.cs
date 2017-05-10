//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;


//namespace AtelierXNA.Éléments_Tuile
//{
//    /// <summary>
//    /// This is a game component that implements IUpdateable.
//    /// </summary>
//    public class PersonnageAniméÉclairée : PersonnageAnimé
//    {
//        string NomEffetAffichage { get; set; }

//        Lumière LumiereJeu { get; set; }
//        Vector3 CouleurLumièreAmbiante { get; set; }
//        Vector4 CouleurLumièreDiffuse { get; set; }
//        Vector3 CouleurLumièreSpéculaire { get; set; }
//        Vector3 CouleurLumièreEmissive { get; set; }
//        Effect EffetAffichage { get; set; }
//        MatériauÉclairé MatériauAffichage { get; set; }
//        Caméra CaméraJeu { get; set; }



//        RessourcesManager<Effect> GestionnaireDeShaders { get; set; }
//        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
//        BoundingSphere SphèreDeCollision { get; set; }

//        public PersonnageAniméÉclairée(Game game, float vitesseDéplacementGaucheDroite, float vitesseMaximaleSaut, float masse, Vector3 position, float intervalleMAJ,
//            Keys[] contrôles, float intervalleMAJAnimation, string[] nomSprites, string type, int[] nbFramesSprites, PlayerIndex numManette, Lumière lumiereJeu, string nomEffetAffichage)
//            : base(game, vitesseDéplacementGaucheDroite, vitesseMaximaleSaut, masse, position, intervalleMAJ, contrôles, intervalleMAJAnimation, nomSprites, type, nbFramesSprites, numManette)
//        {
//            LumiereJeu = lumiereJeu;
//            NomEffetAffichage = nomEffetAffichage;
//        }

//        /// <summary>
//        /// Allows the game component to perform any initialization it needs to before starting
//        /// to run.  This is where it can query for any required services and load content.
//        /// </summary>
//        public override void Initialize()
//        {
//            CouleurLumièreAmbiante = new Vector3(0.5f, 0.5f, 0.5f);
//            CouleurLumièreDiffuse = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
//            CouleurLumièreEmissive = new Vector3(0.1f, 0.1f, 0.1f);
//            CouleurLumièreSpéculaire = new Vector3(0.6f, 0.6f, 0.6f);

//            base.Initialize();
//        }

//        protected override void LoadContent()
//        {
//            base.LoadContent();
//            GestionnaireDeTextures = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
//            GestionnaireDeShaders = Game.Services.GetService(typeof(RessourcesManager<Effect>)) as RessourcesManager<Effect>;
//            EffetAffichage = (GestionnaireDeShaders.Find(NomEffetAffichage)).Clone();
//            MatériauAffichage = new MatériauÉclairé(CaméraJeu, LumiereJeu, CouleurLumièreAmbiante, CouleurLumièreDiffuse,
//                                                    CouleurLumièreEmissive, CouleurLumièreSpéculaire, LumiereJeu.Intensité);
//            AnalyserModèle();
//            CréerSphèreDeCollision();
//            RemplirListeCamera();
//        }

//        void RemplirListeCamera()
//        {
//            CaméraJeu = Game.Components.First(t => t is CaméraDePoursuite) as CaméraDePoursuite;
//        }

//        protected void CréerSphèreDeCollision()
//        {
//            BoundingSphere sphèreTotaleTemporaire = new BoundingSphere();
//            for (int i = 0; i < Frame.Sommets.Length; i++)
//            {
//                BoundingSphere sphèreCollisionDuMaillage = Frame.Sommets[i].Position.;
//                sphèreTotaleTemporaire = BoundingSphere.CreateMerged(sphèreTotaleTemporaire, sphèreCollisionDuMaillage);
//                BoundingSphere sphèreCollisionDuMaillage = new BoundingSphere(Frame.Sommets[i].Position, Frame.Sommets[i + 1].Position.);
//            }
//            for (int i = 0; i < Modèle.Meshes.Count; ++i)
//            {
//                BoundingSphere sphèreCollisionDuMaillage = Modèle.Meshes[i].BoundingSphere;
//                sphèreTotaleTemporaire = BoundingSphere.CreateMerged(sphèreTotaleTemporaire, sphèreCollisionDuMaillage); // Ou
//                                                                                                                         //BoundingSphere.CreateMerged(ref sphèreTotaleTemporaire, ref sphèreCollisionDuMaillage, out sphèreTotaleTemporaire);
//            }
//            SphèreDeCollision = sphèreTotaleTemporaire.Transform(Monde);
//        }

//        private void AnalyserModèle()
//        {
//            Texture2D textureLocale = TextureModèle;

//            foreach (ModelMesh maillage in Modèle.Meshes)
//            {
//                foreach (ModelMeshPart facetteMaillage in maillage.MeshParts)
//                {
//                    InfoModèle infoModèle;
//                    Effect effetLocal = EffetAffichage.Clone();
//                    if (facetteMaillage.Effect is BasicEffect)
//                    {
//                        BasicEffect effetDeBase = (BasicEffect)facetteMaillage.Effect;
//                        if (effetDeBase.Texture != null)
//                        {
//                            textureLocale = effetDeBase.Texture;
//                        }
//                        infoModèle = new InfoModèle(effetLocal, effetDeBase.Texture, effetDeBase.TextureEnabled, CouleurLumièreAmbiante, new Vector4(effetDeBase.DiffuseColor, 1f),
//                                                    CouleurLumièreEmissive, effetDeBase.SpecularColor, effetDeBase.SpecularPower);
//                        //infoModèle = new InfoModèle(effetLocal, effetDeBase.Texture, effetDeBase.TextureEnabled, CouleurLumièreAmbiante, CouleurLumièreDiffuse,
//                        //                            CouleurLumièreEmissive, CouleurLumièreSpéculaire, PUISSANCE_SPÉCULAIRE);
//                    }
//                    else
//                    {
//                        infoModèle = new InfoModèle(effetLocal, textureLocale, false, CouleurLumièreAmbiante, CouleurLumièreDiffuse, CouleurLumièreEmissive, CouleurLumièreSpéculaire, PUISSANCE_SPÉCULAIRE);
//                    }
//                    facetteMaillage.Tag = infoModèle;
//                }
//            }
//        }

//        /// <summary>
//        /// Allows the game component to update itself.
//        /// </summary>
//        /// <param name="gameTime">Provides a snapshot of timing values.</param>
//        public override void Update(GameTime gameTime)
//        {
//            // TODO: Add your update code here

//            base.Update(gameTime);
//        }
//    }
//}
