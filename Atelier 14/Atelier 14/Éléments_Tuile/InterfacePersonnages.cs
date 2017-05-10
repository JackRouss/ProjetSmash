using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AtelierXNA.Éléments_Tuile
{
    public class InterfacePersonnages : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int BORDURE_GAUCHE = 5;
        const int BORDURE_BAS = 20;
        const int ÉCART_ENTRE_INTERFACE = 1100;
        const int ÉCART_ENTRE_COPOSANT_INTERFACE = 90;
        const int DIMENSION_TETE_PERSO_LONGUEUR = 50;
        const int DIMENSION_TETE_PERSO_LARGEUR = 80;
        const int DIMENSION_TETE_PERSO_LONGUEUR_TOTALE = DIMENSION_TETE_PERSO_LONGUEUR + BORDURE_GAUCHE;
        const int DIMENSION_TETE_PERSO_LARGEUR_TOTALE = DIMENSION_TETE_PERSO_LARGEUR + BORDURE_BAS;


        string TypePersonnage { get; set; }
        PlayerIndex NumManette { get; set; }

        List<Texture2D> TetePersonnage { get; set; }
        Texture2D ImageVie { get; set; }
        RessourcesManager<SpriteFont> GestionnaireFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireTexture { get; set; }
        SpriteBatch GestionSprites { get; set; }
        SpriteFont ArialFont { get; set; }
        Color CouleurTexte { get; set; }
        List<PersonnageAnimé> ListesPerso { get; set; }
        CaméraDePoursuite Caméra { get; set; }

        TuileTexturée NomJoueur { get; set; }


        int DécalementImagePerso { get; set; }
        int DÉcalementPoucentageVie { get; set; }
        Rectangle[] PositionTete { get; set; }
        Vector2[] PositionTypePersonage { get; set; }


        public InterfacePersonnages(Game game, string typePersonnege, PlayerIndex numManette)
            : base(game)
        {
            TypePersonnage = typePersonnege;
            NumManette = numManette;
        }
        public override void Initialize()
        {
            DrawOrder = 6;
            GestionnaireFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            GestionnaireTexture = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            ArialFont = GestionnaireFonts.Find("Arial");
            ImageVie = GestionnaireTexture.Find("vie");
            CouleurTexte = Color.White;
            ListesPerso = new List<PersonnageAnimé>();
            TetePersonnage = new List<Texture2D>();
            PositionTete = new Rectangle[2];
            PositionTypePersonage = new Vector2[2];
            RemplirListePerso();
            RemplirListeCamera();
            RemplirListeTetePersonage();
            ModifierImage();
            Calculer();
            CréerPosition();
            base.Initialize();
        }

        void RemplirListePerso()
        {
            if (ListesPerso.Count == 0)
            {
                foreach (GameComponent perso in Game.Components)
                {
                    if (perso is PersonnageAnimé)
                    {
                        ListesPerso.Add(perso as PersonnageAnimé);
                    }
                }
            }
        }
        void RemplirListeCamera()
        {
            Caméra = Game.Components.First(t => t is CaméraDePoursuite) as CaméraDePoursuite;
        }

        void RemplirListeTetePersonage()
        {
            foreach(PersonnageAnimé perso in ListesPerso)
            {
                TetePersonnage.Add(GestionnaireTexture.Find(perso.TypePersonnage + "/" + perso.NomsSprites[4] + (perso.TypePersonnage == "Robot"? "(1)":"0")));
            }
        }
        private void ModifierImage()
        {
            int nbTexels;
            Color[] texels;
            nbTexels = ImageVie.Width * ImageVie.Height;
            texels = new Color[nbTexels];
            ImageVie.GetData<Color>(texels);

            for (int noTexel = 0; noTexel < nbTexels; ++noTexel)
            {
                if(texels[noTexel].R == 255 && texels[noTexel].G == 255 && texels[noTexel].B == 255)
                {
                    texels[noTexel].R = 0;
                    texels[noTexel].G = 0;
                    texels[noTexel].B = 0;
                    texels[noTexel].A = 0;
                }     
            }
            ImageVie.SetData<Color>(texels);
        }

        void Calculer()
        {
            DécalementImagePerso = (int)(Atelier.GetDimensionÉcran(0) - (DIMENSION_TETE_PERSO_LONGUEUR_TOTALE));
        }

        void CréerPosition()
        {
            for (int i = 0; i < PositionTete.Length; i++)
            {
                PositionTete[i] = new Rectangle(BORDURE_GAUCHE + i * DécalementImagePerso,
                                                            Game.Window.ClientBounds.Height - DIMENSION_TETE_PERSO_LARGEUR_TOTALE,
                                                            DIMENSION_TETE_PERSO_LONGUEUR,
                                                            DIMENSION_TETE_PERSO_LARGEUR);
            }
           


            PositionTypePersonage[0] = new Vector2(DIMENSION_TETE_PERSO_LONGUEUR_TOTALE, Game.Window.ClientBounds.Height - ÉCART_ENTRE_COPOSANT_INTERFACE);
            PositionTypePersonage[1] = new Vector2(DécalementImagePerso - DIMENSION_TETE_PERSO_LONGUEUR, Game.Window.ClientBounds.Height - ÉCART_ENTRE_COPOSANT_INTERFACE);

        }

        public override void Update(GameTime gameTime)
        {       
            base.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime)
        {
            int rocket = 0;
            GestionSprites.Begin();
            foreach(PersonnageAnimé perso in ListesPerso)
            {
                Vector3 positionPerso = perso.GetPositionPersonnage;
                GestionSprites.Draw(TetePersonnage[rocket],PositionTete[rocket], CouleurTexte);
                GestionSprites.DrawString(ArialFont, perso.TypePersonnage, PositionTypePersonage[rocket], CouleurTexte, 0, new Vector2(0, 0), 0.3f, SpriteEffects.None, 0);
                GestionSprites.DrawString(ArialFont, perso.VieEnPourcentage.ToString(), 
                                            new Vector2(DIMENSION_TETE_PERSO_LONGUEUR_TOTALE + rocket * (DécalementImagePerso - ArialFont.MeasureString(perso.VieEnPourcentage.ToString()).X - DIMENSION_TETE_PERSO_LONGUEUR_TOTALE)
                                          , Game.Window.ClientBounds.Height - ArialFont.MeasureString(TypePersonnage).Y), CouleurTexte);
                for (int i = 0; i < perso.NbVies; i++)
                {
                    AfficherNombreVie(i, rocket, perso);
                }
                Vector3 PositionÉcran = GestionSprites.GraphicsDevice.Viewport.Project(positionPerso, Caméra.Projection, Caméra.Vue, Matrix.Identity);
                GestionSprites.DrawString(ArialFont, perso.NumManette.ToString(), new Vector2(PositionÉcran.X - 15, PositionÉcran.Y - 100), CouleurTexte, 0, new Vector2(0, 0), 0.3f, SpriteEffects.None, 0);
                rocket++;
            }
            
            GestionSprites.End();
            base.Draw(gameTime);
        }

       void AfficherNombreVie(int cptVie, int cptPerso, PersonnageAnimé perso)
        {
            float Écart = DIMENSION_TETE_PERSO_LONGUEUR_TOTALE + ArialFont.MeasureString(perso.VieEnPourcentage.ToString()).X;
            GestionSprites.Draw(ImageVie, new Rectangle((int)(Écart + cptPerso * (DécalementImagePerso - Écart - ArialFont.MeasureString(perso.VieEnPourcentage.ToString()).X - 30) + cptVie * (cptPerso == 1 ? -1 : 1) * 30), Game.Window.ClientBounds.Height - 50, 30, 30), CouleurTexte);
        }
       
    }

}
