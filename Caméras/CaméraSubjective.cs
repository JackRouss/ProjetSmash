using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
   public class CaméraSubjective : Caméra
   {
      const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
      const float ACCÉLÉRATION = 0.001f;
      const float VITESSE_INITIALE_ROTATION = 5f;
      const float VITESSE_INITIALE_TRANSLATION = 0.5f;
      const float DELTA_LACET = MathHelper.Pi / 180; // 1 degré à la fois
      const float DELTA_TANGAGE = MathHelper.Pi / 180; // 1 degré à la fois
      const float DELTA_ROULIS = MathHelper.Pi / 180; // 1 degré à la fois
      const float RAYON_COLLISION = 1f;

      Vector3 Direction { get; set; }
      Vector3 Latéral { get; set; }
      Vector3 Angle { get; set; }
      float VitesseTranslation { get; set; }
      float VitesseRotation { get; set; }

      float IntervalleMAJ { get; set; }
      float TempsÉcouléDepuisMAJ { get; set; }
      InputManager GestionInput { get; set; }

      bool estEnZoom;
      bool EstEnZoom
      {
         get { return estEnZoom; }
         set
         {
            float ratioAffichage = Game.GraphicsDevice.Viewport.AspectRatio;
            estEnZoom = value;
            if (estEnZoom)
            {
               CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF / 2, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            }
            else
            {
               CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            }
         }
      }

      public CaméraSubjective(Game jeu, Vector3 positionCaméra, Vector3 cible, Vector3 orientation, float intervalleMAJ)
         : base(jeu)
      {
         IntervalleMAJ = intervalleMAJ;
         CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
         CréerPointDeVue(positionCaméra, cible, orientation);
         EstEnZoom = false;
      }

      public override void Initialize()
      {
         VitesseRotation = VITESSE_INITIALE_ROTATION;
         VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
         TempsÉcouléDepuisMAJ = 0;
         base.Initialize();
         GestionInput = Game.Services.GetService(typeof(InputManager)) as InputManager;
         Angle = Vector3.Zero;
      }

      protected override void CréerPointDeVue()
      {
         Angle *= VitesseRotation;
         OrientationVerticale = Vector3.Transform(OrientationVerticale, Matrix.CreateFromAxisAngle(Latéral, Angle.X));
         OrientationVerticale = Vector3.Transform(OrientationVerticale, Matrix.CreateFromAxisAngle(Direction, Angle.Z));

         Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(OrientationVerticale, Angle.Y));
         Direction = Vector3.Transform(Direction, Matrix.CreateFromAxisAngle(Latéral, Angle.X));

         Latéral = Vector3.Transform(Latéral, Matrix.CreateFromAxisAngle(OrientationVerticale, Angle.Y));
         Latéral = Vector3.Transform(Latéral, Matrix.CreateFromAxisAngle(Direction, Angle.Z));


         Vue = Matrix.CreateLookAt(Position, Position + Direction, OrientationVerticale);
         GénérerFrustum();
         Angle = Vector3.Zero;
      }

      protected override void CréerPointDeVue(Vector3 position, Vector3 cible, Vector3 orientation)
      {
         Position = position;
            Direction = Vector3.Normalize(cible - position);
            OrientationVerticale = Vector3.Normalize(orientation);
            Latéral = Vector3.Normalize(Vector3.Cross(OrientationVerticale, Direction));
            CréerPointDeVue();
      }

      public override void Update(GameTime gameTime)
      {
         float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
         TempsÉcouléDepuisMAJ += TempsÉcoulé;
         GestionClavier();
         if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
         {
            if (GestionInput.EstEnfoncée(Keys.LeftShift) || GestionInput.EstEnfoncée(Keys.RightShift))
            {
               GérerAccélération();
               GérerDéplacement();
               GérerRotation();
               CréerPointDeVue();
            }
            TempsÉcouléDepuisMAJ = 0;
         }
         base.Update(gameTime);
      }

      private int GérerTouche(Keys touche)
      {
         return GestionInput.EstEnfoncée(touche) ? 1 : 0;
      }

      private void GérerAccélération()
      {
         int valAccélération = (GérerTouche(Keys.Subtract) + GérerTouche(Keys.OemMinus)) - (GérerTouche(Keys.Add)+GérerTouche(Keys.OemPlus));
         if (valAccélération != 0)
         {
            IntervalleMAJ += ACCÉLÉRATION * valAccélération;
            IntervalleMAJ = MathHelper.Max(INTERVALLE_MAJ_STANDARD, IntervalleMAJ);
         }
      }

      private void GérerDéplacement()
      {
         Vector3 nouvellePosition = Position;
         float déplacementDirection = (GérerTouche(Keys.W) - GérerTouche(Keys.S)) * VitesseTranslation;
         float déplacementLatéral = (GérerTouche(Keys.A) - GérerTouche(Keys.D)) * VitesseTranslation;

         Position += déplacementDirection * Direction;
         Position += déplacementLatéral * Latéral;
      }

      private void GérerRotation()
      {
         GérerLacet();
         GérerTangage();
         GérerRoulis();
      }

      private void GérerLacet()
      {
         float angle = 0;
         if (GestionInput.EstEnfoncée(Keys.LeftShift))
         {
            angle += GestionInput.EstEnfoncée(Keys.Left) ? DELTA_LACET : 0;
            angle -= GestionInput.EstEnfoncée(Keys.Right) ? DELTA_LACET : 0;
            Angle += new Vector3(0, angle, 0);
         }
      }

      private void GérerTangage()
      {
         float angle = 0;
         if (GestionInput.EstEnfoncée(Keys.LeftShift))
         {
            angle += GestionInput.EstEnfoncée(Keys.Up) ? DELTA_TANGAGE : 0;
            angle -= GestionInput.EstEnfoncée(Keys.Down) ? DELTA_TANGAGE : 0;
            Angle += new Vector3(angle, 0, 0);
         }
      }

      private void GérerRoulis()
      {
         float angle = 0;
         if (GestionInput.EstEnfoncée(Keys.LeftShift))
         {
            angle += GestionInput.EstEnfoncée(Keys.PageUp) ? DELTA_ROULIS : 0;
            angle -= GestionInput.EstEnfoncée(Keys.PageDown) ? DELTA_ROULIS : 0;
            Angle += new Vector3(0, 0, angle);
         }
      }

      private void GestionClavier()
      {
         if (GestionInput.EstNouvelleTouche(Keys.Z))
         {
            EstEnZoom = !EstEnZoom;
         }
      }
   }
}
