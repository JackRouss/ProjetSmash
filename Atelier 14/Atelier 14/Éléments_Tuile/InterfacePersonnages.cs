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

        Texture2D TetePersonnage { get; set; }
        RessourcesManager<SpriteFont> GestionnaireFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireTexture { get; set; }
        SpriteBatch GestionSprites { get; set; }
        SpriteFont ArialFont { get; set; }
        Color CouleurTexte { get; set; }
        List<PersonnageAnimé> ListesPerso { get; set; }
        CaméraDePoursuite Caméra { get; set; }

        TuileTexturée NomJoueur { get; set; }
        Game Game { get; set; }


        public InterfacePersonnages(Game game, string typePersonnege, PlayerIndex numManette)
            : base(game)
        {
            TypePersonnage = typePersonnege;
            NumManette = numManette;
            Game = game;
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
            CouleurTexte = Color.White;
            TetePersonnage = GestionnaireTexture.Find("Idle (1)");
            ListesPerso = new List<PersonnageAnimé>();
            RemplirListePerso();
            RemplirListeCamera();

            base.Initialize();
        }

        void RemplirListePerso()
        {
            if (ListesPerso.Count == 0)
            {
                foreach (GameComponent perso in Game.Components)
                {
                    if (perso is Personnage)
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

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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
                GestionSprites.Draw(TetePersonnage, new Rectangle(5, Game.Window.ClientBounds.Height - 100, 50,80), CouleurTexte);
                GestionSprites.DrawString(ArialFont, perso.TypePersonnage, new Vector2(BORDURE_GAUCHE + redneck * ÉCART_ENTRE_INTERFACE, Game.Window.ClientBounds.Height - ÉCART_ENTRE_COPOSANT_INTERFACE), CouleurTexte, 0, new Vector2(0, 0), 0.3f, SpriteEffects.None, 0);
                GestionSprites.DrawString(ArialFont, perso.VieEnPourcentage.ToString(), new Vector2(BORDURE_GAUCHE + redneck * ÉCART_ENTRE_INTERFACE, Game.Window.ClientBounds.Height - ArialFont.MeasureString(TypePersonnage).Y), CouleurTexte);
                //GestionSprites.DrawString(ArialFont, perso.numManette.ToString(), new Vector2(positionPerso.X+390 + (float)2.1* positionPerso.X, positionPerso.Y+160 + (float)-4 * positionPerso.Y), CouleurTexte, 0, new Vector2(0, 0), 0.3f, SpriteEffects.None, 0);
                GestionSprites.DrawString(ArialFont, perso.numManette.ToString(), new Vector2(CounterCamera(positionPerso.X, Caméra.Position.X), positionPerso.Y + 160 + (float)-4 * positionPerso.Y), CouleurTexte, 0, new Vector2(0, 0), 0.3f, SpriteEffects.None, 0);

                redneck++;
            }
            
            GestionSprites.End();
            base.Draw(gameTime);
        }

        float CounterCamera(float position, float positionCamera)
        {
            float différence = Math.Abs(positionCamera - position);
            return 390 + différence;

        }
    }
}
