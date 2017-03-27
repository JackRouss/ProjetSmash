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
using AtelierXNA.Menus;
using AtelierXNA.Éléments_Tuile;

namespace AtelierXNA
{
    public class Atelier : Microsoft.Xna.Framework.Game
    {
        #region Constantes.
        public const float INTERVALLE_CALCUL_FPS = 1f;
        public const float INTERVALLE_MAJ_STANDARD = 1f / 60f;
        const float INTERVALLE_MAJ_ANIMATION = 1f / 25f;
        readonly string[] NomCartes = { "BackGround1", "BackGround2", "BackGround3", "BackGround4" };
        readonly Vector2[] DimensionCarte = { new Vector2(843, 316),new Vector2(844, 358), new Vector2(844, 364), new Vector2(844, 362) };
        readonly Color[] CouleurCartes = { Color.ForestGreen, Color.DeepSkyBlue, Color.Beige, Color.YellowGreen };

        public Vector3 VECTEUR_ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE = ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE * (Vector3.Down);
        public Vector3 CIBLE_INITIALE_CAMÉRA = new Vector3(1, 0, -1);
        public Vector3 POSITION_INITIALE_CAMÉRA = Vector3.Zero;
        public const float ACCÉLÉRATION_GRAVITATIONNELLE_PERSONNAGE = 40f;
        public const float ACCÉLÉRATION_GRAVITATIONNELLE_PROJECTILE = 0.5f; 

        public string[] NOMS_SPRITES_NINJA = { "Attack__00", "Climb_00", "Dead__00", "Glide_00", "Idle__00", "Jump__00", "Jump_Attack__00", "Jump_Throw__00", "Run__00", "Slide__00", "Throw__00" };
        public string[] NOMS_SPRITES_ROBOT = { "Melee ", "RunShoot ", "Dead ", "Jump ", "Idle ", "Jump ", "JumpMelee ", "JumpShoot ", "Run ", "Slide ", "Shoot " };
        public int[] NB_FRAMES_SPRITES_ROBOT = { 8, 9, 10, 10, 10, 10, 8, 5, 8, 10, 4 };
        public int[] NB_FRAMES_SPRITES_NINJA = { 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10 };


        enum GameState { MENU_PRINCIPAL, MENU_PERSONNAGE, MENU_DIFFICULTÉ, MENU_CARTE, MENU_PAUSE, JEU }


        #endregion

        #region Services et propriétés de base.
        GraphicsDeviceManager PériphériqueGraphique { get; set; }
        SpriteBatch GestionSprites { get; set; }
        RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        RessourcesManager<SoundEffect> GestionnaireDeSons { get; set; }
        RessourcesManager<Song> GestionnaireDeChansons { get; set; }




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



        #endregion

        #region Composants de jeu.
        PersonnageAnimé Joueur { get; set; }
        PersonnageAnimé Bot { get; set; }
        Map Carte { get; set; }
        TuileTexturée BackGround { get; set; }
        ArrièrePlanDéroulant ArrièrePlan { get; set; }
        bool VieilÉtatCollisionPerso { get; set; }
        #endregion

        #region Initialisation.
        public Atelier()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            IsFixedTimeStep = false;
            IsMouseVisible = true;
        }


        protected override void Initialize()
        {
            InitialiserServices();
            Menu = new MenuPrincipal(this);
            Components.Add(Menu);
            base.Initialize();
            //MediaPlayer.Play(GestionnaireDeChansons.Find("Pixelland"));
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

            //GestionnaireDeSons.Add("gameover", this.Content.Load<SoundEffect>("Sounds/SoundEffects/gameover"));
            //GestionnaireDeSons.Add("punch", this.Content.Load<SoundEffect>("Sounds/SoundEffects/punch"));
            //GestionnaireDeSons.Add("screaminggoat", this.Content.Load<SoundEffect>("Sounds/SoundEffects/screaminggoat"));
            //GestionnaireDeSons.Add("wilhelm", this.Content.Load<SoundEffect>("Sounds/SoundEffects/wilhelm"));
        }
        private void ChargerModèles()
        {

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


            Services.AddService(typeof(RessourcesManager<SoundEffect>), GestionnaireDeSons);
            Services.AddService(typeof(RessourcesManager<Song>), GestionnaireDeChansons);
            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
            Services.AddService(typeof(InputControllerManager), GestionManettes);
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(SpriteBatch), GestionSprites);



            Components.Add(GestionInput);
            Components.Add(GestionManettes);
        }
        void InitialiserJeu()
        {
            Components.Remove(MenuDiff);


            AjouterCaméra();
            BackGround = new TuileTexturée(this, 1, new Vector3(0, 0, 0), new Vector3(0, -60, -200), DimensionCarte[MenuCa.ChoixCarte], NomCartes[MenuCa.ChoixCarte], 0);
            Components.Add(BackGround);



            AjouterCarte();
            AjouterJoueurs();

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
            Carte = new Map(this, 1, Vector3.Zero, Vector3.Zero, CouleurCartes[MenuCa.ChoixCarte]);

            Components.Add(Carte);
        }
        void AjouterJoueurs()
        {
            Keys[] CONTRÔLES_JOUEUR = { Keys.D, Keys.A, Keys.LeftShift, Keys.Space, Keys.P, Keys.J };
            Keys[] CONTRÔLES_BOT = { Keys.H, Keys.F, Keys.RightShift, Keys.Enter, Keys.L, Keys.N };
            if (MenuPerso.État == MenuPersonnage.ÉTAT.NINJA)
            {
                Joueur = new PersonnageAnimé(this, 5f, 35f, 50, new Vector3(15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_JOUEUR, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_NINJA, "Ninja", NB_FRAMES_SPRITES_NINJA);
            }
            if (MenuPerso.État == MenuPersonnage.ÉTAT.ROBOT)
            {
                Joueur = new PersonnageAnimé(this, 5f, 35f, 100, new Vector3(15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_JOUEUR, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_ROBOT, "Robot", NB_FRAMES_SPRITES_ROBOT);
            }
            if (MenuDiff.CHOIX == MenuDifficulté.ÉTAT.FACILE)
            {
                Bot = new PersonnageAnimé(this, 5f, 35f, 100, new Vector3(-15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_BOT, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_ROBOT, "Robot", NB_FRAMES_SPRITES_ROBOT);
            }
            if (MenuDiff.CHOIX == MenuDifficulté.ÉTAT.NORMAL)
            {
                Bot = new PersonnageAnimé(this, 5f, 35f, 100, new Vector3(-15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_BOT, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_ROBOT, "Robot", NB_FRAMES_SPRITES_ROBOT);
            }
            if (MenuDiff.CHOIX == MenuDifficulté.ÉTAT.DIFFICILE)
            {
                Bot = new PersonnageAnimé(this, 5f, 35f, 100, new Vector3(-15, 0, 0), INTERVALLE_MAJ_STANDARD, CONTRÔLES_BOT, INTERVALLE_MAJ_ANIMATION, NOMS_SPRITES_ROBOT, "Robot", NB_FRAMES_SPRITES_ROBOT);
            }
            Components.Add(Bot);
            Components.Add(Joueur);

        }
        void InitialiserMenuPersonnages()
        {
            Components.Remove(Menu);
            MenuPerso = new MenuPersonnage(this, INTERVALLE_MAJ_ANIMATION);
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
        #endregion

        #region Boucle de jeu.
        protected override void Update(GameTime gameTime)
        {
            GérerTransition();
            GérerMusique();
            base.Update(gameTime);
            if (ÉtatJeu == GameState.JEU)
            {
                GérerCollisions();
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
                        ÉtatJeu = GameState.MENU_PERSONNAGE;
                        Menu.PasserMenuSuivant = false;
                        InitialiserMenuPersonnages();
                    }
                    break;
                case GameState.MENU_PERSONNAGE:
                    if (MenuPerso.PasserMenuSuivant)
                    {
                        ÉtatJeu = GameState.MENU_CARTE;
                        MenuPerso.PasserMenuSuivant = false;
                        InitialiserMenuCartes();
                    }
                    break;
                case GameState.MENU_CARTE:
                    if (MenuCa.PasserMenuSuivant)
                    {
                        ÉtatJeu = GameState.MENU_DIFFICULTÉ;
                        MenuPerso.PasserMenuSuivant = false;
                        InitialiserMenuDifficulté();
                    }
                    break;
                case GameState.MENU_DIFFICULTÉ:
                    if (MenuDiff.PasserMenuSuivant)
                    {
                        ÉtatJeu = GameState.JEU;
                        MenuDiff.PasserMenuSuivant = false;
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
            Services.RemoveService(typeof(RessourcesManager<Song>));
            Services.RemoveService(typeof(RessourcesManager<SpriteFont>));
            Services.RemoveService(typeof(RessourcesManager<Texture2D>));
            Services.RemoveService(typeof(RessourcesManager<Model>));
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
                //MediaPlayer.Play(GestionnaireDeChansons.Find("Cyborg Ninja"));
            }
        }

        void GérerCollisions()
        {
            if (Joueur.EstEnCollision(Bot) && VieilÉtatCollisionPerso != Joueur.EstEnCollision(Bot))
            {
                Joueur.VecteurVitesse += Bot.VecteurVitesse * Bot.Masse / Joueur.Masse;
                Bot.VecteurVitesse += Joueur.VecteurVitesse * Joueur.Masse / Bot.Masse;
            }
            if (Joueur.EstEnCollision(Bot))
            {
                if (Joueur.EstEnAttaque)
                {
                    Bot.EncaisserDégâts(Joueur);
                }
                if (Bot.EstEnAttaque)
                {
                    Joueur.EncaisserDégâts(Bot);
                }
            }
        
            foreach (GameComponent g in Components)
            {
                if (g is Projectile)
                {
                    if ((g as Projectile).EstEnCollision(Joueur))
                    {
                        Joueur.EncaisserDégâts(g as Projectile);
                    }
                    else if ((g as Projectile).EstEnCollision(Bot))
                    {
                        Bot.EncaisserDégâts(g as Projectile);
                    }
                }
            }

            VieilÉtatCollisionPerso = Joueur.EstEnCollision(Bot);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
        #endregion
    }
}