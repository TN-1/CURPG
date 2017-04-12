using System;
using System.Collections.Generic;
using CURPG_Graphical_MonoGame_Windows.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using QuakeConsole;

namespace CURPG_Graphical_MonoGame_Windows
{
    public class ScreenManager : Game
    {
        public static GraphicsDeviceManager GraphicsDeviceMgr;
        public static SpriteBatch Sprites;
        public static Dictionary<string, Texture2D> Textures2D;
        public static Dictionary<string, SpriteFont> Fonts;
        public static List<GameScreen> ScreenList;
        public static ContentManager ContentMgr;
        public static ConsoleComponent console;
        public static PythonInterpreter interpreter;
        public static System.Drawing.Rectangle ScreenArea;


        public ScreenManager()
        {
            var pt = new System.Drawing.Point(0, 0);
            ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(pt);
            GraphicsDeviceMgr = new GraphicsDeviceManager(this);
            GraphicsDeviceMgr.IsFullScreen = true;
            GraphicsDeviceMgr.PreferredBackBufferHeight = ScreenArea.Height;
            GraphicsDeviceMgr.PreferredBackBufferWidth = ScreenArea.Width;
            GraphicsDeviceMgr.IsFullScreen = false;
            Window.Title = "CURPG";
            Window.AllowUserResizing = false;

            Content.RootDirectory = "Content";

            //Setup console
            console = new ConsoleComponent(this);
            Components.Add(console);
            console.FontColor = Color.Aqua;
            console.InputPrefixColor = Color.Aqua;
            console.InputPrefix = ">";
            interpreter = new PythonInterpreter();
            console.Interpreter = interpreter;

        }

        protected override void Initialize()
        {
            Textures2D = new Dictionary<string, Texture2D>();
            Fonts = new Dictionary<string, SpriteFont>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ContentMgr = Content;
            Sprites = new SpriteBatch(GraphicsDevice);

            // Load any full game assets here

            AddScreen(new PlayScreen());
        }

        protected override void UnloadContent()
        {
            foreach (var screen in ScreenList)
            {
                screen.UnloadAssets();
            }
            Textures2D.Clear();
            Fonts.Clear();
            ScreenList.Clear();
            Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            try
            {
                // TODO Remove temp code
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    Exit();
                }

                var startIndex = ScreenList.Count - 1;
                while (ScreenList[startIndex].IsPopup && ScreenList[startIndex].IsActive)
                {
                    startIndex--;
                }
                for (var i = startIndex; i < ScreenList.Count; i++)
                {
                    ScreenList[i].Update(gameTime);
                }
            }
            catch (Exception ex)
            {
                // ErrorLog.AddError(ex);
                throw ex;
            }
            finally
            {
                base.Update(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            var startIndex = ScreenList.Count - 1;
            while (ScreenList[startIndex].IsPopup)
            {
                startIndex--;
            }

            GraphicsDevice.Clear(ScreenList[startIndex].BackgroundColor);
            GraphicsDeviceMgr.GraphicsDevice.Clear(ScreenList[startIndex].BackgroundColor);

            for (var i = startIndex; i < ScreenList.Count; i++)
            {
                ScreenList[i].Draw(gameTime);
            }

            base.Draw(gameTime);
        }

        public static void AddFont(string fontName)
        {
            if (Fonts == null)
            {
                Fonts = new Dictionary<string, SpriteFont>();
            }
            if (!Fonts.ContainsKey(fontName))
            {
                Fonts.Add(fontName, ContentMgr.Load<SpriteFont>(fontName));
            }
        }

        public static void RemoveFont(string fontName)
        {
            if (Fonts.ContainsKey(fontName))
            {
                Fonts.Remove(fontName);
            }
        }

        public static void AddTexture2D(string textureName)
        {
            if (Textures2D == null)
            {
                Textures2D = new Dictionary<string, Texture2D>();
            }
            if (!Textures2D.ContainsKey(textureName))
            {
                Textures2D.Add(textureName, ContentMgr.Load<Texture2D>(textureName));
            }
        }

        public static void RemoveTexture2D(string textureName)
        {
            if (Textures2D.ContainsKey(textureName))
            {
                Textures2D.Remove(textureName);
            }
        }

        public static void AddScreen(GameScreen gameScreen)
        {
            if (ScreenList == null)
            {
                ScreenList = new List<GameScreen>();
            }
            ScreenList.Add(gameScreen);
            gameScreen.Initialize();
            gameScreen.LoadAssets();
        }

        public static void RemoveScreen(GameScreen gameScreen)
        {
            gameScreen.UnloadAssets();
            ScreenList.Remove(gameScreen);
            if (ScreenList.Count < 1)
                AddScreen(new GameScreen()); // Default screen
        }

        public static void ChangeScreens(GameScreen currentScreen, GameScreen targetScreen)
        {
            RemoveScreen(currentScreen);
            AddScreen(targetScreen);
        }

        /// <summary>
        /// Hooks into the OnExiting event to save the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            foreach(GameScreen screen in ScreenList)
            {
                if(screen is PlayScreen play)
                {
                    var status = CURPG_Engine.Core.Persistance.SaveGame(play.world, play.player);
                    if (status == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Save Failed. Do you want to close?", "Error", System.Windows.Forms.MessageBoxButtons.RetryCancel);
                    }
                }
            }
        }
    }
}