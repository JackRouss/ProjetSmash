using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;


namespace AtelierXNA
{

    public class InputControllerManager : Microsoft.Xna.Framework.GameComponent
    {
        int JoueurMax { get; set; }
        int cpt { get; set; }
        bool[] ManetteActive { get; set; }
        bool DÈconnection { get; set; }
        Color CouleurFond { get; set; }
        GamePadState[] Ancien…tatManette { get; set; }

        GamePadState[] …tatManette { get; set; }

        List<Buttons> LesAncinneTouche { get; set; }
        List<Buttons> LesNouvelleTouche { get; set; }

        Buttons[] lesBouttons { get; set; }


        string[,] convert { get; set; }


        public InputControllerManager(Game game)
            : base(game)
        {
            Initialize();
        }


        public override void Initialize()
        {
            …tatManette = new GamePadState[2];
            Ancien…tatManette = new GamePadState[2];
            DÈconnection = false;
            convert = new string[2, 2] { { "One", "Two" }, { "0", "1" } };
            cpt = 0;

            LesAncinneTouche = new List<Buttons> { {new Buttons()}, { new Buttons() } };
            LesNouvelleTouche = new List<Buttons> { { new Buttons() }, { new Buttons() } };

            lesBouttons = new Buttons[] { Buttons.A, Buttons.B, Buttons.Back, Buttons.BigButton, Buttons.DPadDown, Buttons.DPadLeft, Buttons.DPadRight, Buttons.DPadUp, Buttons.LeftShoulder, Buttons.LeftStick, Buttons.LeftThumbstickDown, Buttons.LeftThumbstickLeft, Buttons.LeftThumbstickRight, Buttons.LeftThumbstickUp, Buttons.LeftTrigger, Buttons.RightShoulder, Buttons.RightStick, Buttons.RightThumbstickDown, Buttons.RightThumbstickLeft, Buttons.RightThumbstickRight, Buttons.RightThumbstickUp, Buttons.RightTrigger, Buttons.Start, Buttons.X, Buttons.Y };
            
            base.Initialize();
        }



        public override void Update(GameTime gameTime)
        {
            LesAncinneTouche = new List<Buttons> { LesNouvelleTouche[0] ,  LesNouvelleTouche[1]  };
            Ancien…tatManette = …tatManette;
        }


        public bool EstManetteActivÈe(PlayerIndex numManette)
        {
            int Case = TrouverNumManette(numManette);
            …tatManette[Case] = GamePad.GetState(numManette);
            return …tatManette[Case].IsConnected;
        }

        public bool EstNouvelleTouche(PlayerIndex numManette, Buttons touche)
        {
            int Case = TrouverNumManette(numManette);
            LesNouvelleTouche[0] = GottaChekThemAll(0);
            …tatManette[Case] = GamePad.GetState(numManette);
            bool estNouvelleTouche = …tatManette[Case].IsButtonDown(touche);
            int i = 0;
            if (estNouvelleTouche)
            {
                estNouvelleTouche = (Buttons)LesAncinneTouche[Case] != touche;
            }
            return estNouvelleTouche;

        }

        public bool EstToucheEnfoncÈe(PlayerIndex numManette, Buttons touche)
        {
            int Case = TrouverNumManette(numManette);
            …tatManette[Case] = GamePad.GetState(numManette);
            return …tatManette[Case].IsButtonDown(touche);
        }

        int TrouverNumManette(PlayerIndex numManette)
        {
            string y = numManette.ToString();
            string x = "0";
            int i = 0;
            while (i < convert.GetLength(0))
            {
                if(convert[i,0] == y)
                {
                    x = convert[1, i];
                }
                i++;
            }
           return int.Parse(x);
        }

        Buttons GottaChekThemAll(int i)
        {
            Buttons boutton = new Buttons();
            bool trouver = false;
            int j = 0;
            while (j < lesBouttons.Length && !trouver)
            {
                if (…tatManette[i].IsButtonDown(lesBouttons[j]))
                {
                    trouver = true;
                    boutton = lesBouttons[j];
                    return boutton;
                 }
                    j++;
                if (j == lesBouttons.Length)
                {
                    trouver = true;
                }
            }
            return boutton;
        }



    }
}
