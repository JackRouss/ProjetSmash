using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace AtelierXNA
{
    public class CaméraDePoursuite : Caméra, IPause
    {
        const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const float ACCÉLÉRATION = 0.001f;
        const float VITESSE_INITIALE_TRANSLATION = 0.5f;
        const float RAYON_COLLISION = 1f;

        Vector3 Direction { get; set; }
        Vector3 Latéral { get; set; }
        Vector3 Angle { get; set; }
        float VitesseTranslation { get; set; }
        public Vector3 PositionInitial { get; set; }

        float IntervalleMAJ { get; set; }
        float TempsÉcouléDepuisMAJ { get; set; }
        List<PersonnageAnimé> ListesPerso { get; set; }
        Map Carte { get; set; }
        bool DoIt { get; set; }

        public CaméraDePoursuite(Game jeu, Vector3 positionCaméra, Vector3 cible, Vector3 orientation, float intervalleMAJ)
           : base(jeu)
        {
            IntervalleMAJ = intervalleMAJ;
            PositionInitial = positionCaméra;
            CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            CréerPointDeVue(positionCaméra, cible, orientation);
        }

        public override void Initialize()
        {
            VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
            Angle = Vector3.Zero;
            ListesPerso = new List<PersonnageAnimé>();
            DoIt = true;
        }

        protected override void CréerPointDeVue()
        {           
            Latéral = Vector3.Transform(Latéral, Matrix.CreateFromAxisAngle(OrientationVerticale, Angle.Y));
            Latéral = Vector3.Transform(Latéral, Matrix.CreateFromAxisAngle(Direction, Angle.Z));

            Vue = Matrix.CreateLookAt(Position, Position + Direction , OrientationVerticale);
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
            RemplirListePerso();
            InitialiserPropCarte();
            
            float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsÉcouléDepuisMAJ += TempsÉcoulé;
            GérerDéplacement();

            if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
            {              
                CréerPointDeVue();             
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }
        void InitialiserPropCarte()
        {   
            if(DoIt)
            {
                Carte = Game.Components.First(t => t is Map) as Map;
                DoIt = false;
            }                           
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
        private void GérerDéplacement()
        {
            float moyennePosX = ListesPerso.Average(x => x.GetPositionPersonnage.X);
            float moyennePosY = ListesPerso.Average(x => x.GetPositionPersonnage.Y) + 20;
            Position = new Vector3(MathHelper.Min(moyennePosX,Carte.LIMITE_PLAQUETTE.X), MathHelper.Min(moyennePosY,Carte.LIMITE_PLAQUETTE.Z), Position.Z);
            if(moyennePosX < 0)
            {
                Position = new Vector3(MathHelper.Max(moyennePosX, Carte.LIMITE_PLAQUETTE.Y), Position.Y, Position.Z);
            }
            if(moyennePosY < 0)
            {          
                Position = new Vector3(Position.X, MathHelper.Max(moyennePosY, Carte.LIMITE_PLAQUETTE.W), Position.Z);
            }
        }
       
    }
}
