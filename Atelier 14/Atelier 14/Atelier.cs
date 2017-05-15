using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using AtelierXNA.Menus;
using AtelierXNA.Éléments_Tuile;
using AtelierXNA.AI;
using AtelierXNA.Autres;

namespace AtelierXNA
{
    public class Atelier : Microsoft.Xna.Framework.Game
    {
        #region Constantes.
   
        public const float INTERVALLE_CALCUL_FPS = 1f;
        public const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const float INTERVALLE_MAJ_ANIMATION = 1f / 25f;
        readonly string[] NomCartes = { "BackGround1", "BackGround2", "BackGround3", "BackGround4", "BackGround5", "BackGround6"};
        readonly Vector2[] DimensionCarte = { new Vector2(843, 316),new Vector2(844, 358), new Vector2(844, 364), new Vector2(844, 362), new Vector2(844, 362) , new Vector2(844, 362) };
        public static readonly Color[] CouleurCartes = { Color.ForestGreen, Color.DeepSkyBlue, Color.Beige, Color.YellowGreen, Color.AliceBlue, Color.Aquamarine };
        PlayerIndex[] Joueurs = { PlayerIndex.One, PlayerIndex.Two};
        bool[] ConnectionJoueur { get; set; }
        bool PvP { get; set; }
        MenuPersonnage.ÉTAT[] ChoixJoueurs { get; set; }
        int cptJoueur { get; set; }


        public Vector3 VECTEUR_ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE = ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE * (Vector3.Down);
        public Vector3 CIBLE_INITIALE_CAMÉRA = new Vector3(1, 0, -1);
        public Vector3 POSITION_INITIALE_CAMÉRA = Vector3.Zero;
        public const float ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE = 32f;
        public const float ACCÉLÉRATION_GRAVITATIONNELLE_PROJECTILE = 0.5f; 

        public string[] NOMS_SPRITES_NINJA = { "Attack__00", "Climb_00", "Dead__00", "Glide_00", "Idle__00", "Jump__00", "Jump_Attack__00", "Jump_Throw__00", "Run__00", "Slide__00", "Throw__00" };
        public string[] NOMS_SPRITES_ROBOT = { "Melee ", "RunShoot ", "Dead ", "Jump ", "Idle ", "Jump ", "JumpMelee ", "JumpShoot ", "Run ", "Slide ", "Shoot " };
        public int[] NB_FRAMES_SPRITES_ROBOT = { 8, 9, 10, 10, 10, 10, 8, 5, 8, 10, 4 };
        public int[] NB_FRAMES_SPRITES_NINJA = { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };

        static int LongueurÉcran = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        static int LargeurÉcran = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        const int NB_PLATEFORMES = 10;
        enum GameState { MENU_PRINCIPAL, MENU_PERSONNAGE, MENU_DIFFICULTÉ, MENU_CARTE, MENU_PAUSE, JEU , FIN_JEU, MENU_PvP}


        #endregion

        #region Services et propriétés de base.
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        RessourcesManager<SoundEffect> GestionnaireDeSons { get; set; }
        RessourcesManager<Song> GestionnaireDeChansons { get; set; }
        Générateur g { get; set; }
        RessourcesManager<Effect> GestionnaireDeShaders { get; set; }



        InputControllerManager GestionManettes { get; set; }
        InputManager GestionInput { get; set; }
        Caméra CaméraJeu { get; set; }
        GameState ÉtatJeu { get; set; }
        GameState AncienÉtatJeu { get; set; }
        bool AChangéÉtat { get; set; }
        MenuPrincipal Menu { get; set; }
        MenuPersonnage MenuPerso { get; set; }
        MenuDifficulté MenuDiff { get; set; }
        MenuPause MenuPau { get; set; }
        MenuCartes MenuCa { get; set; }
        MenuFinJeu Fin { get; set; }
        MenuPvP MenuJoueurs { get; set; }
        PersonnageAnimé PersoEnJeux { get; set; }



        #endregion

        #region Composants de jeu.
        PersonnageAnimé Joueur { get; set; }
        PersonnageAnimé Joueur2 { get; set; }
        PersonnageAnimé Bot { get; set; }
        Map Carte { get; set; }
        TuileTexturéeAniméeJulia julia { get; set; }
        TuileTexturée BackGround { get; set; }
        ArrièrePlanDéroulant ArrièrePlan { get; set; }
        bool VieilÉtatCollisionPerso { get; set; }
        bool VieilÉtatAttaqueJoueur { get; set; }
        bool VieilÉtatAttaqueBot { get; set; }
        bool VieilÉtatAttaqueJoueur2 { get; set; }
        InterfacePersonnages Interface { get; set; }
        #endregion

        #region Initialisation.
        public Atelier()
        {
            
            PériphériqueGraphique = new GraphicsDeviceManager(this);
           
            this.PériphériqueGraphique.PreferredBackBufferWidth = this.Window.ClientBounds.Width + (LongueurÉcran - this.Window.ClientBounds.Width);
            this.PériphériqueGraphique.PreferredBackBufferHeight = this.Window.ClientBounds.Height + (LargeurÉcran - this.Window.ClientBounds.Height);
            //this.PériphériqueGraphique.IsFullScreen = true;
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
            g = new Générateur();
            Services.AddService(typeof(Générateur), g);
        }

        public static float GetDimensionÉcran(int coté)
        {
            if(coté == 0)
            {
                return LongueurÉcran;
            }
            else
            {
                return LargeurÉcran;
            }

        }

        protected override void Initialize()
        {
            InitialiserServices();
            Commencer();
            base.Initialize();
            //MediaPlayer.Play(GestionnaireDeChansons.Find("Pixelland"));
        }

        void Commencer()
        {
            InitialiserMenuPrincipal();
            ConnectionManette();
            InitialiserJoueur();
        }

        void InitialiserJoueur()
        {
            cptJoueur = 0;
            ChoixJoueurs = new MenuPersonnage.ÉTAT[2];
        }

        #region Chargement des ressources.
        protected override void LoadContent()
        {
            ChargerTextures();
            ChargerSons();
            ChargerModèles();
            ChargerFonts();
            base.LoadContent();
        }

        private void ChargerFonts()
        {
            GestionnaireDeFonts.Add("Arial", this.Content.Load<SpriteFont>("Fonts/Arial"));
        }
        private void ChargerSons()
        {
            GestionnaireDeChansons.Add("Cyborg Ninja", this.Content.Load<Song>("Sounds/Songs/Cyborg Ninja"));
            GestionnaireDeChansons.Add("Decisions", this.Content.Load<Song>("Sounds/Songs/Decisions"));
            GestionnaireDeChansons.Add("Pinball Spring 160", this.Content.Load<Song>("Sounds/Songs/Pinball Spring 160"));
            GestionnaireDeChansons.Add("Pixelland", this.Content.Load<Song>("Sounds/Songs/Pixelland"));
            GestionnaireDeChansons.Add("MEME", this.Content.Load<Song>("Sounds/Songs/MEME"));

            GestionnaireDeSons.Add("gameover", this.Content.Load<SoundEffect>("Sounds/SoundEffects/gameover"));
            GestionnaireDeSons.Add("punch", this.Content.Load<SoundEffect>("Sounds/SoundEffects/punch"));
            GestionnaireDeSons.Add("screaminggoat", this.Content.Load<SoundEffect>("Sounds/SoundEffects/screaminggoat"));
            GestionnaireDeSons.Add("steelsword", this.Content.Load<SoundEffect>("Sounds/SoundEffects/steelsword"));
            GestionnaireDeSons.Add("Arrow", this.Content.Load<SoundEffect>("Sounds/SoundEffects/Arrow"));
            GestionnaireDeSons.Add("LaserBlasts", this.Content.Load<SoundEffect>("Sounds/SoundEffects/LaserBlasts"));
            GestionnaireDeSons.Add("ROBOTATTAQUE", this.Content.Load<SoundEffect>("Sounds/SoundEffects/ROBOTATTAQUE"));
            GestionnaireDeSons.Add("Run", this.Content.Load<SoundEffect>("Sounds/SoundEffects/Run"));
            

        }
        private void ChargerModèles()
        {
            GestionnaireDeModèles.Add("LP_tree",this.Content.Load<Model>("Models/LP_tree"));
            GestionnaireDeModèles.Add("tree", this.Content.Load<Model>("Models/tree"));
        }
        private void ChargerTextures()
        {
            ChargerTexturesPersonnages();
        }
        private void ChargerTexturesPersonnages()
        {
            ChargerNinja();
            ChargerRobot();
        }
        private void ChargerNinja()
        {
            for (int j = 0; j < PersonnageAnimé.NB_ANIMATIONS; ++j)
            {
                for (int i = 0; i < NB_FRAMES_SPRITES_NINJA[j]; ++i)
                {
                    GestionnaireDeTextures.Add(NOMS_SPRITES_NINJA[j] + i.ToString(), this.Content.Load<Texture2D>("Textures/" + "Ninja/" + NOMS_SPRITES_NINJA[j] + i));
                }
            }
            GestionnaireDeTextures.Add("BouclierNinja",this.Content.Load<Texture2D>("Textures/"+"Ninja/"+"BouclierNinja"));
        }
        private void ChargerRobot()
        {
            for (int j = 0; j < PersonnageAnimé.NB_ANIMATIONS; ++j)
            {
                for (int i = 1; i <= NB_FRAMES_SPRITES_ROBOT[j]; ++i)
                {
                    GestionnaireDeTextures.Add(NOMS_SPRITES_ROBOT[j] + "(" + i.ToString() + ")", this.Content.Load<Texture2D>("Textures/" + "Robot/" + NOMS_SPRITES_ROBOT[j] + "(" + i + ")"));
                }
            }
        }

        #endregion
        private void InitialiserServices()
        {
            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
            GestionInput = new InputManager(this);
            GestionSprites = new SpriteBatch(GraphicsDevice);
            GestionManettes = new InputControllerManager(this);
            GestionnaireDeChansons = new RessourcesManager<Song>(this, "Songs");
            GestionnaireDeSons = new RessourcesManager<SoundEffect>(this, "Sounds");
            GestionnaireDeShaders = new RessourcesManager<Effect>(this, "Effets");


            Services.AddService(typeof(RessourcesManager<SoundEffect>), GestionnaireDeSons);
            Services.AddService(typeof(RessourcesManager<Song>), GestionnaireDeChansons);
            Services.AddService(typeof(RessourcesManager<Effect>), GestionnaireDeShaders);
            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
            Services.AddService(typeof(InputControllerManager), GestionManettes);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(SpriteBatch), GestionSprites);



            Components.Add(GestionInput);
            Components.Add(GestionManettes);
        }

        void ConnectionManette()
        {
            ConnectionJoueur = new bool[Joueurs.Length];
            for (int i = 0; i < ConnectionJoueur.Length; i++)
            {
                ConnectionJoueur[i] = GestionManettes.EstManetteActivée(Joueurs[i]);
            }
        }

        void InitialiserMenuPrincipal()
        {
            Menu = new MenuPrincipal(this);
            Components.Add(Menu);
        }

        void InitialiserJeu()
        {


            AjouterCaméra();
            if(MenuCa.ChoixCarte == 4)
            {
                julia = new TuileTexturéeAniméeJulia(this, 1, new Vector3(0, 0, 0), new Vector3(0, -60, -200), DimensionCarte[MenuCa.ChoixCarte], "Fond_blanc.svg", 0);
                Components.Add(julia);

            }
            else
            {
                BackGround = new TuileTexturée(this, 1, new Vector3(0, 0, 0), new Vector3(0, -60, -200), DimensionCarte[MenuCa.ChoixCarte], NomCartes[MenuCa.ChoixCarte], 0);
                Components.Add(BackGround);

            }



            AjouterCarte();
            if (PvP)
            {
                AjouterJoueursPvP();
            }
            else
            {
                AjouterJoueursPvBot();

            }
            base.Initialize();
        }
        void AjouterCaméra()
        {
            CaméraJeu = new CaméraDePoursuite(this, new Vector3(1, -10, 100), new Vector3(0, -30, 0), Vector3.Up, INTERVALLE_MAJ_STANDARD);
            Services.AddService(typeof(Caméra), CaméraJeu);
            Components.Add(CaméraJeu);
        }
        void AjouterCarte()
        {
            Carte = new Map(this, 1, Vector3.Zero, Vector3.Zero, CouleurCartes[MenuCa.ChoixCarte],NB_PLATEFORMES);
            Components.Add(Carte);

        }
        void AjouterJoueursPvBot()
        {
            Keys[] CONTRÔLES_JOUEUR = { Keys.D, Keys.A, Keys.LeftShift, Keys.Space, Keys.P, Keys.J };
            Keys[] CONTRÔLES_BOT = { Keys.H, Keys.F, Keys.RightShift, Keys.Enter, Keys.L, Keys.N };
            if (MenuPerso.État == MenuPersonnage.ÉTAT.NINJA)
            {
                Joueur = new PersonnageAnimé(this, 20f, 35f, 100, new Vector3(15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_JOUEUR, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_NINJA, "Ninja", NB_FRAMES_SPRITES_NINJA, PlayerIndex.One);
            }
            if (MenuPerso.État == MenuPersonnage.ÉTAT.ROBOT)
            {
                Joueur = new PersonnageAnimé(this, 15f, 35f, 100, new Vector3(15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_JOUEUR, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_ROBOT, "Robot", NB_FRAMES_SPRITES_ROBOT, PlayerIndex.One);
            }
            if (MenuDiff.CHOIX == MenuDifficulté.ÉTAT.FACILE)
            {
                Bot = new Bot(this, 15f, 35f, 100, new Vector3(-15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_BOT, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_ROBOT, "Robot", NB_FRAMES_SPRITES_ROBOT,"Facile", PlayerIndex.Two);
            }
            if (MenuDiff.CHOIX == MenuDifficulté.ÉTAT.NORMAL)
            {
                Bot = new Bot(this, 15f, 35f, 100, new Vector3(-15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_BOT, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_ROBOT, "Robot", NB_FRAMES_SPRITES_ROBOT,"Normal", PlayerIndex.Two);
            }
            if (MenuDiff.CHOIX == MenuDifficulté.ÉTAT.DIFFICILE)
            {
                Bot = new Bot(this, 15f, 35f, 100, new Vector3(-15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_BOT, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_ROBOT, "Robot", NB_FRAMES_SPRITES_ROBOT, "Difficile",PlayerIndex.Two);
            }
            
            Components.Add(Joueur);
            Components.Add(Bot);
            PersoEnJeux = Bot;
            Interface = new InterfacePersonnages(this, "Robot", PlayerIndex.One);
            Components.Add(Interface);
        }

        void AjouterJoueursPvP()
        {
            Keys[] CONTRÔLES_JOUEUR = { Keys.D, Keys.A, Keys.LeftShift, Keys.Space, Keys.P, Keys.J };
            Keys[] CONTRÔLES_JOUEUR2 = { Keys.H, Keys.F, Keys.RightShift, Keys.Enter, Keys.L, Keys.N };
            if (ChoixJoueurs[0] == MenuPersonnage.ÉTAT.NINJA)
            {
                Joueur = new PersonnageAnimé(this, 20f, 35f, 100, new Vector3(15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_JOUEUR, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_NINJA, "Ninja", NB_FRAMES_SPRITES_NINJA, PlayerIndex.One);
            }
            if (ChoixJoueurs[0] == MenuPersonnage.ÉTAT.ROBOT)
            {
                Joueur = new PersonnageAnimé(this, 15f, 35f, 100, new Vector3(15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_JOUEUR, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_ROBOT, "Robot", NB_FRAMES_SPRITES_ROBOT, PlayerIndex.One);
            }
            if (ChoixJoueurs[1] == MenuPersonnage.ÉTAT.NINJA)
            {
                Joueur2 = new PersonnageAnimé(this, 15f, 35f, 100, new Vector3(-15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_JOUEUR2, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_NINJA, "Ninja", NB_FRAMES_SPRITES_ROBOT, PlayerIndex.Two);
            }
            if (ChoixJoueurs[1] == MenuPersonnage.ÉTAT.ROBOT)
            {
                Joueur2 = new PersonnageAnimé(this, 15f, 35f, 100, new Vector3(-15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_JOUEUR2, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_ROBOT, "Robot", NB_FRAMES_SPRITES_ROBOT, PlayerIndex.Two);
            }
           

            Components.Add(Joueur);
            Components.Add(Joueur2);
            PersoEnJeux = Joueur2;
            Interface = new InterfacePersonnages(this, "Robot", PlayerIndex.One);
            Components.Add(Interface);
        }


        void InitialiserMenuPersonnages(PlayerIndex player)
        {
            Components.Remove(MenuJoueurs);
            MenuPerso = new MenuPersonnage(this, INTERVALLE_MAJ_ANIMATION, player);
            MenuPerso.Initialize();
            Components.Add(MenuPerso);
            base.Initialize();
        }

        void InitialiserMenuCartes()
        {
            Components.Remove(MenuPerso);
            MenuCa = new MenuCartes(this, NomCartes);
            Components.Add(MenuCa);
        }
        void InitialiserMenuDifficulté()
        {
            Components.Remove(MenuCa);
            MenuDiff = new MenuDifficulté(this, INTERVALLE_MAJ_ANIMATION);
            Components.Add(MenuDiff);
        }
       
        void InitialiserMenuPause()
        {
            ToggleComponentsUpdate();
            MenuPau = new MenuPause(this, INTERVALLE_MAJ_ANIMATION);
            Components.Add(MenuPau);
        }
        void ToggleComponentsUpdate()
        {
            for (int i = 0; i < Components.Count; ++i)
            {
                if (Components[i] is IPause)
                {
                    (Components[i] as GameComponent).Enabled = !(Components[i] as GameComponent).Enabled;
                }
            }
        }
        void AnimationFinJeux()
        {
            ToggleComponentsUpdate();

            }

        void InitialiserMenuPvP()
        {
            Components.Remove(Menu);
            MenuJoueurs = new MenuPvP(this, INTERVALLE_MAJ_ANIMATION);
            Components.Add(MenuJoueurs);
        }

        void InitialiserFinJeux(PersonnageAnimé mort)
        {
            GestionnaireDeSons.Find("gameover").Play();
            Fin = new MenuFinJeu(this, INTERVALLE_MAJ_STANDARD, 0.4f, mort.NumManette.ToString());

            ÉtatJeu = GameState.FIN_JEU;
            ToggleComponentsUpdate();
            Components.Add(Fin);
        }

        #endregion

        #region Boucle de jeu.
        protected override void Update(GameTime gameTime)
        {
            GérerTransition();
            GérerMusique();
            base.Update(gameTime);
            if (ÉtatJeu == GameState.JEU)
            {
                if (PvP)
                {
                    GérerCollisionsPvP();
                }
                else
                {
                    GérerCollisionsPvBot();
                }
            }
        }

        void GérerTransition()
        {
            AncienÉtatJeu = ÉtatJeu;
            switch (ÉtatJeu)
            {
                case GameState.MENU_PRINCIPAL:

                    if (Menu.PasserMenuSuivant)
                    {
                        ÉtatJeu = GameState.MENU_PvP;
                        Menu.PasserMenuSuivant = false;
                        InitialiserMenuPvP();
                        if (ConnectionJoueur[1])
                        {
                            MenuJoueurs.multiplayer = true;
                        }
                    }
                    break;

                case GameState.MENU_PvP:
                    ConnectionManette();
                    if (ConnectionJoueur[1])
                    {
                        MenuJoueurs.multiplayer = true;
                    }
                    else
                    {
                        MenuJoueurs.multiplayer = false;

                    }
                    if (MenuJoueurs.PasserMenuSuivant)
                    {
                        PvP = MenuJoueurs.PvPActiver;
                        ÉtatJeu = GameState.MENU_PERSONNAGE;
                        MenuJoueurs.PasserMenuSuivant = false;
                        InitialiserMenuPersonnages(Joueurs[cptJoueur]);
                        Components.Remove(MenuJoueurs);

                    }

                    break;
                case GameState.MENU_PERSONNAGE:
                    if (MenuPerso.PasserMenuSuivant)
                    {
                        Components.Remove(MenuPerso);
                        
                        ChoixJoueurs[cptJoueur] = MenuPerso.État;
                        cptJoueur++;
                        if (MenuJoueurs.PvPActiver)
                        {
                            InitialiserMenuPersonnages(Joueurs[cptJoueur]);
                            MenuPerso.PasserMenuSuivant = false;
                            MenuJoueurs.PvPActiver = false;

                        }
                        else
                        {
                        ÉtatJeu = GameState.MENU_CARTE;
                        MenuPerso.PasserMenuSuivant = false;
                        InitialiserMenuCartes();

                    }
                       
                    }
                    break;
                case GameState.MENU_CARTE:
                    if (MenuCa.PasserMenuSuivant)
                    {
                        if (MenuJoueurs.PvBotActiver)
                        {
                            ÉtatJeu = GameState.MENU_DIFFICULTÉ;
                            MenuCa.PasserMenuSuivant = false;
                            InitialiserMenuDifficulté();
                        }
                        else
                        {
                            ÉtatJeu = GameState.JEU;
                            MenuCa.PasserMenuSuivant = false;
                            InitialiserJeu();
                        }
                    }
                    break;
                case GameState.MENU_DIFFICULTÉ:
                    if (MenuDiff.PasserMenuSuivant)
                    {
                        ÉtatJeu = GameState.JEU;
                        MenuDiff.PasserMenuSuivant = false;
                        Components.Remove(MenuDiff);
                        InitialiserJeu();
                    }
                    break;
                
                case GameState.JEU:
                    if (GestionInput.EstNouvelleTouche(Keys.Escape))
                    {
                        ÉtatJeu = GameState.MENU_PAUSE;
                        InitialiserMenuPause();
                        MediaPlayer.Pause();
                    }
                    if (Joueur.décédé)
                    {
                        InitialiserFinJeux(PersoEnJeux);
                        Joueur.décédé = false;
                    }
                    if (PersoEnJeux.décédé)
                    {
                        InitialiserFinJeux(Joueur);
                        PersoEnJeux.décédé = false;
                    }
                   
                    break;
                
               
                case GameState.MENU_PAUSE:
                    if (MenuPau.RésumerLaPartie)
                    {
                        ÉtatJeu = GameState.JEU;
                        MenuPau.RésumerLaPartie = false;
                        ToggleComponentsUpdate();
                        Components.Remove(MenuPau);
                        MediaPlayer.Resume();
                    }
                    if (MenuPau.RetournerMenuPrincipale)
                    {
                        ÉtatJeu = GameState.MENU_PRINCIPAL;
                        Components.Clear();
                        EnleverServices();
                        Initialize();
                        MenuPau.RetournerMenuPrincipale = false;
                    }
                    if (MenuPau.PasserMenuPause)
                    {

                    }
                    break;
                case GameState.FIN_JEU:
                    MediaPlayer.Stop();
                    if (Fin.Recommencer)
                    {
                        Components.Remove(Fin);
                        ÉtatJeu = GameState.MENU_DIFFICULTÉ;
                        MenuDiff.PasserMenuSuivant = true;
                        Components.Clear();
                        EnleverServices();
                        Initialize();
                        Components.Remove(Menu);
                        Fin.Recommencer = false;
                    }
                    if (Fin.RetournerMenuPrincipale)
                    {
                        ÉtatJeu = GameState.MENU_PRINCIPAL;
                        Components.Clear();
                        EnleverServices();
                        Initialize();
                        Fin.RetournerMenuPrincipale = false;
                    }
                    break;
            }
            if (AncienÉtatJeu != ÉtatJeu)
            {
                AChangéÉtat = true;
            }
            else
            {
                AChangéÉtat = false;
            }
        }

        void EnleverServices()
        {
            Services.RemoveService(typeof(RessourcesManager<SoundEffect>));
            Services.RemoveService(typeof(RessourcesManager<Effect>));
            Services.RemoveService(typeof(RessourcesManager<Song>));
            Services.RemoveService(typeof(RessourcesManager<SpriteFont>));
            Services.RemoveService(typeof(RessourcesManager<Texture2D>));
            Services.RemoveService(typeof(RessourcesManager<Model>));
            Services.RemoveService(typeof(RessourcesManager<Effect>));
            Services.RemoveService(typeof(InputControllerManager));
            Services.RemoveService(typeof(InputManager));
            Services.RemoveService(typeof(SpriteBatch));
            Services.RemoveService(typeof(Caméra));
        }


        void GérerMusique()
        {
            if (ÉtatJeu == GameState.JEU && AChangéÉtat)
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(GestionnaireDeChansons.Find("MEME"));
            }
        }

        void GérerCollisionsPvBot()
        {
            bool bouclierJoueurAAbsorber = false;
            bool bouclierBotAAbsorber = false;

            if (Joueur.EstBouclierActif )
            {
                foreach (GameComponent g in Components)
                {
                    if(g is Projectile)
                    {
                        if(Joueur.BouclierPersonnage.EstEnCollision(g as Projectile) && (g as Projectile).NumPlayer != Joueur.NumManette)
                        {
                            Joueur.BouclierPersonnage.EncaisserDégâts(g as Projectile);
                            (g as Projectile).ADetruire = true;
                            bouclierJoueurAAbsorber = true;
                        }
                    }
                    if(g is Personnage)
                    {
                        if(Joueur.BouclierPersonnage.EstEnCollision(g as Personnage) && (g as Personnage).EstEnAttaque && VieilÉtatAttaqueBot != Bot.EstEnAttaque)
                        {
                            Joueur.BouclierPersonnage.EncaisserDégâts(g as Personnage);
                            bouclierJoueurAAbsorber = true;
                        }
                    }
                }
            }
            if (Bot.EstBouclierActif)
            {
                foreach (GameComponent g in Components)
                {
                    if (g is Projectile)
                    {
                        if (Bot.BouclierPersonnage.EstEnCollision(g as Projectile) && (g as Projectile).NumPlayer != Bot.NumManette)
                        {
                            Bot.BouclierPersonnage.EncaisserDégâts(g as Projectile);
                            (g as Projectile).ADetruire = true;
                            bouclierBotAAbsorber = true;
                        }
                    }
                    if (g is Personnage)
                    {
                        if (Bot.BouclierPersonnage.EstEnCollision(g as Personnage) && (g as Personnage).EstEnAttaque && VieilÉtatAttaqueJoueur != Joueur.EstEnAttaque)
                        {
                            Bot.BouclierPersonnage.EncaisserDégâts(g as Personnage);
                            bouclierBotAAbsorber = true;
                        }
                    }
                }
            }
            
            if (Joueur.EstEnCollision(Bot) && VieilÉtatCollisionPerso != Joueur.EstEnCollision(Bot))
            {
                if(!Joueur.EstBouclierActif)
                    Joueur.GérerRecul(Bot);
                if(!Bot.EstBouclierActif)
                    Bot.GérerRecul(Joueur);         
            }

            if (Joueur.EstEnAttaque && Joueur.EstEnCollision(Bot) && (VieilÉtatAttaqueJoueur != Joueur.EstEnAttaque) && !bouclierBotAAbsorber)
            {
                Bot.EncaisserDégâts(Joueur);
            }

            if (Bot.EstEnAttaque && Bot.EstEnCollision(Joueur) && (VieilÉtatAttaqueBot != Bot.EstEnAttaque) && !bouclierJoueurAAbsorber)
            {
                Joueur.EncaisserDégâts(Bot);

            }

            foreach (GameComponent g in Components)
            {
                if (g is Projectile)
                {
                    
                    if ((g as Projectile).EstEnCollision(Joueur) && (g as Projectile).NumPlayer != Joueur.NumManette)
                    {
                        Joueur.EncaisserDégâts(g as Projectile);
                        (g as Projectile).ADetruire = true;
                    }
                    else if ((g as Projectile).EstEnCollision(Bot) && (g as Projectile).NumPlayer != Bot.NumManette)
                    {
                        Bot.EncaisserDégâts(g as Projectile);
                        (g as Projectile).ADetruire = true;
                    }
                }
            }
            VieilÉtatCollisionPerso = Joueur.EstEnCollision(Bot);
            VieilÉtatAttaqueJoueur = Joueur.EstEnAttaque;
            VieilÉtatAttaqueBot = Bot.EstEnAttaque;
        }

        void GérerCollisionsPvP()
        {
            bool bouclierJoueurAAbsorber = false;
            bool bouclierJoueur2AAbsorber = false;

            if (Joueur.EstBouclierActif)
            {
                foreach (GameComponent g in Components)
                {
                    if (g is Projectile)
                    {
                        if (Joueur.BouclierPersonnage.EstEnCollision(g as Projectile) && (g as Projectile).NumPlayer != Joueur.NumManette)
                        {
                            Joueur.BouclierPersonnage.EncaisserDégâts(g as Projectile);
                            (g as Projectile).ADetruire = true;
                            bouclierJoueurAAbsorber = true;
                        }
                    }
                    if (g is Personnage)
                    {
                        if (Joueur.BouclierPersonnage.EstEnCollision(g as Personnage) && (g as Personnage).EstEnAttaque && VieilÉtatAttaqueJoueur2 != Joueur2.EstEnAttaque)
                        {
                            Joueur.BouclierPersonnage.EncaisserDégâts(g as Personnage);
                            bouclierJoueurAAbsorber = true;
                        }
                    }
                }
            }
            if (Joueur2.EstBouclierActif)
            {
                foreach (GameComponent g in Components)
                {
                    if (g is Projectile)
                    {
                        if (Joueur2.BouclierPersonnage.EstEnCollision(g as Projectile) && (g as Projectile).NumPlayer != Joueur2.NumManette)
                        {
                            Joueur2.BouclierPersonnage.EncaisserDégâts(g as Projectile);
                            (g as Projectile).ADetruire = true;
                            bouclierJoueur2AAbsorber = true;
                        }
                    }
                    if (g is Personnage)
                    {
                        if (Joueur2.BouclierPersonnage.EstEnCollision(g as Personnage) && (g as Personnage).EstEnAttaque && VieilÉtatAttaqueJoueur != Joueur.EstEnAttaque)
                        {
                            Joueur2.BouclierPersonnage.EncaisserDégâts(g as Personnage);
                            bouclierJoueur2AAbsorber = true;
                        }
                    }
                }
            }

            if (Joueur.EstEnCollision(Joueur2) && VieilÉtatCollisionPerso != Joueur.EstEnCollision(Joueur2))
            {
                if (!Joueur.EstBouclierActif)
                    Joueur.GérerRecul(Joueur2);
                if (!Joueur2.EstBouclierActif)
                    Joueur2.GérerRecul(Joueur);
            }

            if (Joueur.EstEnAttaque && Joueur.EstEnCollision(Joueur2) && (VieilÉtatAttaqueJoueur != Joueur.EstEnAttaque) && !bouclierJoueur2AAbsorber)
            {
                Joueur2.EncaisserDégâts(Joueur);
            }

            if (Joueur2.EstEnAttaque && Joueur2.EstEnCollision(Joueur) && (VieilÉtatAttaqueJoueur2 != Joueur2.EstEnAttaque) && !bouclierJoueurAAbsorber)
            {
                Joueur.EncaisserDégâts(Joueur2);

            }

            foreach (GameComponent g in Components)
            {
                if (g is Projectile)
                {

                    if ((g as Projectile).EstEnCollision(Joueur) && (g as Projectile).NumPlayer != Joueur.NumManette)
                    {
                        Joueur.EncaisserDégâts(g as Projectile);
                        (g as Projectile).ADetruire = true;
                    }
                    else if ((g as Projectile).EstEnCollision(Joueur2) && (g as Projectile).NumPlayer != Joueur2.NumManette)
                    {
                        Joueur2.EncaisserDégâts(g as Projectile);
                        (g as Projectile).ADetruire = true;
                    }
                }
            }
            VieilÉtatCollisionPerso = Joueur.EstEnCollision(Joueur2);
            VieilÉtatAttaqueJoueur = Joueur.EstEnAttaque;
            VieilÉtatAttaqueJoueur2 = Joueur2.EstEnAttaque;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
        #endregion
    }
}