using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace AtelierXNA.Éléments_Tuile
{
    
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InterfacePersonnages : Microsoft.Xna.Framework.DrawableGameComponent
    {
        const int BORDURE_GAUCHE = 60;
        const int ÉCART_ENTRE_INTERFACE = 600;
        const int ÉCART_ENTRE_COPOSANT_INTERFACE = 90;

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


        public InterfacePersonnages(Game game, string typePersonnege, PlayerIndex numManette)
            : base(game)
        {
            TypePersonnage = typePersonnege;
            NumManette = numManette;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            GestionnaireFonts = Game.Services.GetService(typeof(RessourcesManager<SpriteFont>)) as RessourcesManager<SpriteFont>;
            GestionnaireTexture = Game.Services.GetService(typeof(RessourcesManager<Texture2D>)) as RessourcesManager<Texture2D>;
            GestionSprites = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
            ArialFont = GestionnaireFonts.Find("Arial");
            ImageVie = GestionnaireTexture.Find("vie");
            CouleurTexte = Color.White;
            ListesPerso = new List<PersonnageAnimé>();
            TetePersonnage = new List<Texture2D>();
            RemplirListePerso();
            RemplirListeCamera();
            RemplirListeTetePersonage();
            ModifierImage();

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
            //Image = new Texture2D(imagelocale.GraphicsDevice, imagelocale.Width, imagelocale.Height);
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

        public override void Update(GameTime gameTime)
        {       
            base.Update(gameTime);
        }
        
        public override void Draw(GameTime gameTime)
        {
            int redneck = 0;
            GestionSprites.Begin();
            foreach(PersonnageAnimé perso in ListesPerso)
            {
                Vector3 positionPerso = perso.GetPositionPersonnage;
                GestionSprites.Draw(TetePersonnage[redneck], new Rectangle(5 + redneck* ÉCART_ENTRE_INTERFACE + 100*redneck, Game.Window.ClientBounds.Height - 100, 50,80), CouleurTexte);
                GestionSprites.DrawString(ArialFont, perso.TypePersonnage, new Vector2(BORDURE_GAUCHE + redneck * ÉCART_ENTRE_INTERFACE, Game.Window.ClientBounds.Height - ÉCART_ENTRE_COPOSANT_INTERFACE), CouleurTexte, 0, new Vector2(0, 0), 0.3f, SpriteEffects.None, 0);
                GestionSprites.DrawString(ArialFont, perso.VieEnPourcentage.ToString(), new Vector2(BORDURE_GAUCHE + redneck * ÉCART_ENTRE_INTERFACE, Game.Window.ClientBounds.Height - ArialFont.MeasureString(TypePersonnage).Y), CouleurTexte);
                for (int i = 0; i < perso.NbVies; i++)
                {
                    AfficherNombreVie(i, redneck);
                }
                Vector3 PositionÉcran = GestionSprites.GraphicsDevice.Viewport.Project(positionPerso, Caméra.Projection, Caméra.Vue, Matrix.Identity);
                GestionSprites.DrawString(ArialFont, perso.NumManette.ToString(), new Vector2(PositionÉcran.X - 15, PositionÉcran.Y - 80), CouleurTexte, 0, new Vector2(0, 0), 0.3f, SpriteEffects.None, 0);
                redneck++;
            }
            
            GestionSprites.End();
            base.Draw(gameTime);
        }

       void AfficherNombreVie(int cptVie, int cptPerso)
        {
           GestionSprites.Draw(ImageVie, new Rectangle(100 + cptPerso * 460 + cptVie * 30, Game.Window.ClientBounds.Height - 50, 30, 30), CouleurTexte);
        }
       
    }

}
