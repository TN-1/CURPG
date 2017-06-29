using System;
using System.Collections.Generic;
using CURPG_Engine.Core;
using CURPG_Windows.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using QuakeConsole;
// ReSharper disable UnusedMember.Global

namespace CURPG_Windows
{
    public class ScreenManager : Game
    {
        public static GraphicsDeviceManager GraphicsDeviceMgr;
        public static SpriteBatch Sprites;
        public static ContentManager ContentMgr;
        public static ConsoleComponent Console;
        public static PythonInterpreter Interpreter;
        public static System.Drawing.Rectangle ScreenArea;
        public static Game Game;
        private static Dictionary<string, GameScreen> _screens;
        [NonSerialized] private static Dictionary<string, Texture2D> _textures2D;
        [NonSerialized] private static Dictionary<string, SpriteFont> _fonts;
        [NonSerialized] private static List<GameScreen> _screenList;
        [NonSerialized] private KeyboardState _oldState;
        [NonSerialized] public static World WorldTrans;
        [NonSerialized] public static string[] flags;

        public ScreenManager(string[] Flags)
        {
            var pt = new System.Drawing.Point(0, 0);
            ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(pt);
            GraphicsDeviceMgr = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true,
                PreferredBackBufferHeight = ScreenArea.Height,
                PreferredBackBufferWidth = ScreenArea.Width
            };
            GraphicsDeviceMgr.IsFullScreen = false;
            Window.Title = "CURPG";
            Window.AllowUserResizing = false;

            Content.RootDirectory = "Content";

            //Setup console
            Console = new ConsoleComponent(this);
            Components.Add(Console);
            Console.FontColor = Color.Aqua;
            Console.InputPrefixColor = Color.Aqua;
            Console.InputPrefix = ">";
            Interpreter = new PythonInterpreter();
            Console.Interpreter = Interpreter;

            Interpreter.AddVariable("base", this);

            _screens = new Dictionary<string, GameScreen> {{"Menu", new MenuScreen()}, {"Play", new PlayScreen()}, {"Load", new LoadScreen()}};

            Game = this;
            flags = Flags;
        }

        protected override void Initialize()
        {
            _textures2D = new Dictionary<string, Texture2D>();
            _fonts = new Dictionary<string, SpriteFont>();

            foreach (var s in flags)
            {
                if (s.Contains("--screen-size"))
                {
                    var values = s.Split('-');
                    GraphicsDeviceMgr.IsFullScreen = false;
                    GraphicsDeviceMgr.PreferredBackBufferWidth = Convert.ToInt32(values[2]);
                    GraphicsDeviceMgr.PreferredBackBufferHeight = Convert.ToInt32(values[4]);
                    //TODO: Fix these values based on what the array actually is!
                }
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            ContentMgr = Content;
            Sprites = new SpriteBatch(GraphicsDevice);

            // Load any full game assets here
            AddScreen(_screens["Load"]);
        }

        protected override void UnloadContent()
        {
            foreach (var screen in _screenList)
            {
                screen.UnloadAssets();
            }
            _textures2D.Clear();
            _fonts.Clear();
            _screenList.Clear();
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            var newState = Keyboard.GetState();  // get the newest state

            try
            {
                if (Keyboard.GetState().IsKeyDown(Keys.F12))
                {
                    Exit();
                }
                if (_oldState.IsKeyUp(Keys.OemTilde) && newState.IsKeyDown(Keys.OemTilde))
                    Console.ToggleOpenClose();

                var startIndex = _screenList.Count - 1;
                while (GameScreen.IsPopup && GameScreen.IsActive)
                {
                    startIndex--;
                }
                for (var i = startIndex; i < _screenList.Count; i++)
                {
                    _screenList[i].Update(gameTime);
                }
            }
            finally
            {
                base.Update(gameTime);
            }
            _oldState = newState;
        }

        protected override void Draw(GameTime gameTime)
        {
            var startIndex = _screenList.Count - 1;
            while (GameScreen.IsPopup)
            {
                startIndex--;
            }

            GraphicsDevice.Clear(_screenList[startIndex].BackgroundColor);
            GraphicsDeviceMgr.GraphicsDevice.Clear(_screenList[startIndex].BackgroundColor);

            for (var i = startIndex; i < _screenList.Count; i++)
            {
                _screenList[i].Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        public static void AddFont(string fontName)
        {
            if (_fonts == null)
            {
                _fonts = new Dictionary<string, SpriteFont>();
            }
            if (!_fonts.ContainsKey(fontName))
            {
                _fonts.Add(fontName, ContentMgr.Load<SpriteFont>(fontName));
            }
        }

        public static void RemoveFont(string fontName)
        {
            if (_fonts.ContainsKey(fontName))
            {
                _fonts.Remove(fontName);
            }
        }

        public static void AddTexture2D(string textureName)
        {
            if (_textures2D == null)
            {
                _textures2D = new Dictionary<string, Texture2D>();
            }
            if (!_textures2D.ContainsKey(textureName))
            {
                _textures2D.Add(textureName, ContentMgr.Load<Texture2D>(textureName));
            }
        }

        public static void RemoveTexture2D(string textureName)
        {
            if (_textures2D.ContainsKey(textureName))
            {
                _textures2D.Remove(textureName);
            }
        }

        private static void AddScreen(GameScreen gameScreen)
        {
            if (_screenList == null)
            {
                _screenList = new List<GameScreen>();
            }
            _screenList.Add(gameScreen);
            gameScreen.Initialize();
            gameScreen.LoadAssets();
        }

        private static void RemoveScreen(GameScreen gameScreen)
        {
            if (gameScreen is PlayScreen play)
            {
                var status = Persistance.SaveGame(play.World, play.Player);
                if (status == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Save Failed. Do you want to close?", "Error", System.Windows.Forms.MessageBoxButtons.RetryCancel);
                }
            }

            gameScreen.UnloadAssets();
            _screenList.Remove(gameScreen);
        }

        public static void ChangeScreens(GameScreen currentScreen, GameScreen targetScreen)
        {
            RemoveScreen(currentScreen);
            AddScreen(targetScreen);
        }

        public static void ChangeScreens(string currentScreen, string targetScreen)
        {
            RemoveScreen(_screens[currentScreen]);
            AddScreen(_screens[targetScreen]);
        }

        /// <summary>
        /// Hooks into the OnExiting event to save the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            foreach(GameScreen screen in _screenList)
            {
                if(screen is PlayScreen play)
                {
                    var status = Persistance.SaveGame(play.World, play.Player);
                    if (status == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Save Failed. Do you want to close?", "Error", System.Windows.Forms.MessageBoxButtons.RetryCancel);
                    }
                }
            }
        }
    }
}