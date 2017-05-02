using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtelierXNA
{
   public class ParamètresShaders
   {
      public static void InitialiserParamètresShader(string nomEffet, Effect effetLocal, InfoModèle infoModèle, MatériauÉclairé MatériauAffichage)
      {
         switch (nomEffet)
         {
            case "BASE":
               InitialiserParamètresShaderBase(effetLocal, infoModèle, MatériauAffichage);
               break;
            case "BLINN-PHONG":
               InitialiserParamètresShaderBlinnPhong(effetLocal, infoModèle, MatériauAffichage);
               break;
            case "BLINN-PHONG-BUMPMAP":
               InitialiserParamètresShaderBlinnPhongBumpMap(effetLocal, infoModèle, MatériauAffichage);
               break;
            case "PHONG":
               InitialiserParamètresShaderPhong(effetLocal, infoModèle, MatériauAffichage);
               break;
            case "PHONG-JEAN":
               InitialiserParamètresShaderPhongJean(effetLocal, infoModèle, MatériauAffichage);
               break;
            case "SPOTLIGHT":
               InitialiserParamètresShaderSpotLight(effetLocal, infoModèle, MatériauAffichage);
               break;
            default:
               throw new ArgumentException("Paramètres de l'effet d'affichage inconnus!");
         }
      }

      static void InitialiserParamètresShaderBase(Effect effetLocal, InfoModèle infoModèle, MatériauÉclairé MatériauAffichage)
      {
         effetLocal.Parameters["MondeVueProjection"].SetValue(MatériauAffichage.MondeVueProjection);
         effetLocal.Parameters["CouleurLumiereDiffuse"].SetValue(infoModèle.CouleurDiffuse);
         effetLocal.Parameters["Texture"].SetValue(infoModèle.Texture);
         effetLocal.Parameters["TextureActive"].SetValue(infoModèle.TextureActive);
      }

      static void InitialiserParamètresShaderBlinnPhong(Effect effetLocal, InfoModèle infoModèle, MatériauÉclairé MatériauAffichage)
      {
         effetLocal.Parameters["Monde"].SetValue(MatériauAffichage.Monde);
         effetLocal.Parameters["MondeVueProjection"].SetValue(MatériauAffichage.MondeVueProjection);
         effetLocal.Parameters["MondeTransposeeInverse"].SetValue(MatériauAffichage.MondeTransposéeInverse);
         effetLocal.Parameters["PositionCamera"].SetValue(MatériauAffichage.CaméraJeu.Position);
         effetLocal.Parameters["DirectionLumiere"].SetValue(MatériauAffichage.Position - MatériauAffichage.LumièreJeu.Position);
         effetLocal.Parameters["CouleurLumiere"].SetValue(MatériauAffichage.LumièreJeu.Couleur);
         effetLocal.Parameters["CouleurLumiereAmbiante"].SetValue(infoModèle.CouleurAmbiante);
         effetLocal.Parameters["CouleurLumiereDiffuse"].SetValue(infoModèle.CouleurDiffuse);
         effetLocal.Parameters["CouleurLumiereEmissive"].SetValue(infoModèle.CouleurEmissive);
         effetLocal.Parameters["CouleurLumiereSpeculaire"].SetValue(infoModèle.CouleurSpéculaire);
         effetLocal.Parameters["PuissanceSpeculaire"].SetValue(infoModèle.PuissanceSpéculaire);
         effetLocal.Parameters["Texture"].SetValue(infoModèle.Texture);
         effetLocal.Parameters["TextureActive"].SetValue(infoModèle.TextureActive);
      }

      static void InitialiserParamètresShaderBlinnPhongBumpMap(Effect effetLocal, InfoModèle infoModèle, MatériauÉclairé MatériauAffichage)
      {
         effetLocal.Parameters["Monde"].SetValue(MatériauAffichage.Monde);
         effetLocal.Parameters["MondeVueProjection"].SetValue(MatériauAffichage.MondeVueProjection);
         effetLocal.Parameters["MondeTransposeeInverse"].SetValue(MatériauAffichage.MondeTransposéeInverse);
         effetLocal.Parameters["PositionCamera"].SetValue(MatériauAffichage.CaméraJeu.Position);
         effetLocal.Parameters["DirectionLumiere"].SetValue(MatériauAffichage.Position - MatériauAffichage.LumièreJeu.Position);
         effetLocal.Parameters["CouleurLumiere"].SetValue(MatériauAffichage.LumièreJeu.Couleur);
         effetLocal.Parameters["CouleurLumiereAmbiante"].SetValue(infoModèle.CouleurAmbiante);
         effetLocal.Parameters["CouleurLumiereDiffuse"].SetValue(infoModèle.CouleurDiffuse);
         effetLocal.Parameters["CouleurLumiereEmissive"].SetValue(infoModèle.CouleurEmissive);
         effetLocal.Parameters["CouleurLumiereSpeculaire"].SetValue(infoModèle.CouleurSpéculaire);
         effetLocal.Parameters["PuissanceSpeculaire"].SetValue(infoModèle.PuissanceSpéculaire);
         effetLocal.Parameters["Texture"].SetValue(infoModèle.Texture);
         effetLocal.Parameters["TextureActive"].SetValue(infoModèle.TextureActive);
         effetLocal.Parameters["BumpMap"].SetValue(MatériauAffichage.BumpMap);
      }

      static void InitialiserParamètresShaderPhong(Effect effetLocal, InfoModèle infoModèle, MatériauÉclairé MatériauAffichage)
      {
         effetLocal.Parameters["Monde"].SetValue(MatériauAffichage.Monde);
         effetLocal.Parameters["MondeVueProjection"].SetValue(MatériauAffichage.MondeVueProjection);
         effetLocal.Parameters["PositionCamera"].SetValue(MatériauAffichage.CaméraJeu.Position);
         effetLocal.Parameters["PositionLumiere"].SetValue(MatériauAffichage.LumièreJeu.Position);
         effetLocal.Parameters["DirectionLumiere"].SetValue(MatériauAffichage.Position - MatériauAffichage.LumièreJeu.Position);
         effetLocal.Parameters["CarreDistanceLumiere"].SetValue(MatériauAffichage.CarréDistanceLumière);
         effetLocal.Parameters["CouleurLumiereAmbiante"].SetValue(infoModèle.CouleurAmbiante);
         effetLocal.Parameters["CouleurLumiereDiffuse"].SetValue(infoModèle.CouleurDiffuse);
         effetLocal.Parameters["CouleurLumiereSpeculaire"].SetValue(infoModèle.CouleurSpéculaire);
         effetLocal.Parameters["PuissanceSpeculaire"].SetValue(infoModèle.PuissanceSpéculaire);
         effetLocal.Parameters["IntensiteLumiereDiffuse"].SetValue(infoModèle.IntensitéLumièreDiffuse);
         effetLocal.Parameters["IntensiteLumiereSpeculaire"].SetValue(infoModèle.IntensitéLumièreSpéculaire);
         effetLocal.Parameters["Texture"].SetValue(infoModèle.Texture);
         effetLocal.Parameters["TextureActive"].SetValue(infoModèle.TextureActive);
      }

      static void InitialiserParamètresShaderPhongJean(Effect effetLocal, InfoModèle infoModèle, MatériauÉclairé MatériauAffichage)
      {
         effetLocal.Parameters["Monde"].SetValue(MatériauAffichage.Monde);
         effetLocal.Parameters["MondeVueProjection"].SetValue(MatériauAffichage.MondeVueProjection);
         effetLocal.Parameters["PositionCamera"].SetValue(MatériauAffichage.CaméraJeu.Position);
         effetLocal.Parameters["PositionLumiere"].SetValue(MatériauAffichage.LumièreJeu.Position);
         effetLocal.Parameters["CouleurLumiereAmbiante"].SetValue(infoModèle.CouleurAmbiante);
         effetLocal.Parameters["CouleurLumiereDiffuse"].SetValue(infoModèle.CouleurDiffuse);
         effetLocal.Parameters["PuissanceSpeculaire"].SetValue(infoModèle.PuissanceSpéculaire);
         effetLocal.Parameters["Texture"].SetValue(infoModèle.Texture);
         effetLocal.Parameters["TextureActive"].SetValue(infoModèle.TextureActive);
      }

      static void InitialiserParamètresShaderSpotLight(Effect effetLocal, InfoModèle infoModèle, MatériauÉclairé MatériauAffichage)
      {
         effetLocal.Parameters["Monde"].SetValue(MatériauAffichage.Monde);
         effetLocal.Parameters["MondeVueProjection"].SetValue(MatériauAffichage.MondeVueProjection);
         effetLocal.Parameters["PositionLumiere"].SetValue(MatériauAffichage.LumièreJeu.Position);
         effetLocal.Parameters["RayonLumiere"].SetValue(MatériauAffichage.LumièreJeu.Rayon);
         effetLocal.Parameters["CouleurLumiereDiffuse"].SetValue(infoModèle.CouleurDiffuse);
         effetLocal.Parameters["Texture"].SetValue(infoModèle.Texture);
         effetLocal.Parameters["TextureActive"].SetValue(infoModèle.TextureActive);
      }

      static void InitialiserParamètresShaderTeinteDeGris(Effect effetLocal, InfoModèle infoModèle, MatériauÉclairé MatériauAffichage)
      {
         effetLocal.Parameters["Monde"].SetValue(MatériauAffichage.Monde);
         effetLocal.Parameters["MondeVueProjection"].SetValue(MatériauAffichage.MondeVueProjection);
         effetLocal.Parameters["PositionCamera"].SetValue(MatériauAffichage.CaméraJeu.Position);
         effetLocal.Parameters["PositionLumiere"].SetValue(MatériauAffichage.LumièreJeu.Position);
         effetLocal.Parameters["RayonLumiere"].SetValue(MatériauAffichage.LumièreJeu.Rayon);
         effetLocal.Parameters["CouleurLumiereDiffuse"].SetValue(infoModèle.CouleurDiffuse);
         effetLocal.Parameters["Texture"].SetValue(infoModèle.Texture);
         effetLocal.Parameters["TextureActive"].SetValue(infoModèle.TextureActive);
      }

      static void InitialiserParamètresShaderModèle(Effect effetLocal, InfoModèle infoModèle, MatériauÉclairé MatériauAffichage)
      {
         effetLocal.Parameters["Monde"].SetValue(MatériauAffichage.Monde);
         effetLocal.Parameters["VueProjection"].SetValue(MatériauAffichage.VueProjection);
         effetLocal.Parameters["MondeVueProjection"].SetValue(MatériauAffichage.MondeVueProjection);
         effetLocal.Parameters["MondeTransposeeInverse"].SetValue(MatériauAffichage.MondeTransposéeInverse);
         effetLocal.Parameters["PositionCamera"].SetValue(MatériauAffichage.CaméraJeu.Position);
         effetLocal.Parameters["PositionLumiere"].SetValue(MatériauAffichage.LumièreJeu.Position);
         effetLocal.Parameters["DirectionLumiere"].SetValue(MatériauAffichage.Position - MatériauAffichage.LumièreJeu.Position);
         effetLocal.Parameters["CouleurLumiere"].SetValue(MatériauAffichage.LumièreJeu.Couleur);
         effetLocal.Parameters["CarreDistanceLumiere"].SetValue(MatériauAffichage.CarréDistanceLumière);
         effetLocal.Parameters["RayonLumiere"].SetValue(MatériauAffichage.LumièreJeu.Rayon);
         effetLocal.Parameters["CouleurLumiereAmbiante"].SetValue(infoModèle.CouleurAmbiante);
         effetLocal.Parameters["CouleurLumiereDiffuse"].SetValue(infoModèle.CouleurDiffuse);
         effetLocal.Parameters["CouleurLumiereEmissive"].SetValue(infoModèle.CouleurEmissive);
         effetLocal.Parameters["CouleurLumiereSpeculaire"].SetValue(infoModèle.CouleurSpéculaire);
         effetLocal.Parameters["PuissanceSpeculaire"].SetValue(infoModèle.PuissanceSpéculaire);
         effetLocal.Parameters["IntensiteLumiereDiffuse"].SetValue(infoModèle.IntensitéLumièreDiffuse);
         effetLocal.Parameters["IntensiteLumiereSpeculaire"].SetValue(infoModèle.IntensitéLumièreSpéculaire);
         effetLocal.Parameters["Texture"].SetValue(infoModèle.Texture);
         effetLocal.Parameters["TextureActive"].SetValue(infoModèle.TextureActive);
      }
   }
}
