using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CURPG_Engine.Core;
using System;
using System.IO;
using System.Reflection;
using GeonBit.UI;
using GeonBit.UI.Entities;
using QuakeConsole;

namespace CURPG_Graphical
{
    public class CURPG : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Dictionary<string, Texture2D> TileTextures = new Dictionary<string, Texture2D>();
        World world;
        List<Tile> TileSet;
        Player player;
        private KeyboardState oldState;
        System.Drawing.Rectangle ScreenArea;
        System.Drawing.Rectangle MapArea;
        Camera Camera;
        ConsoleComponent console;
        PythonInterpreter interpreter;
        bool PlayerLoc;

        public CURPG()
        {
            var pt = new System.Drawing.Point(0, 0);
            ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(pt);
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = ScreenArea.Height;
            graphics.PreferredBackBufferWidth = ScreenArea.Width;
            MapArea.Height = (int)Math.Ceiling((ScreenArea.Height * .7) / 24);
            MapArea.Width = (int)Math.Ceiling((ScreenArea.Width * .5) / 24);
            Content.RootDirectory = "Content";
            Window.Title = "CURPG";
            Window.AllowUserResizing = false;
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(Window.Handle);
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;

            //Setup the console
            console = new ConsoleComponent(this);
            Components.Add(console);
            console.FontColor = Color.Black;
            interpreter = new PythonInterpreter();
            console.Interpreter = interpreter;
        }

        protected override void Initialize()
        {
            if (Persistance.CanLoad())
            {
                world = Persistance.LoadWorld();
                player = Persistance.LoadPlayer();
                TileSet = world.TileSet;

                if (world == null || player == null)
                {
                    var tilesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"DataFiles\Tiles.xml");
                    TileSet = WorldTools.TileSetBuilder(tilesPath);
                    world = WorldTools.GenerateWorld(0, 128, 128, TileSet, "World", 24);
                    var pt = PlayerTools.GetSpawn(world, MapArea.Width / 2, MapArea.Height / 2);
                    player = PlayerTools.RandomPlayer(pt.X, pt.Y);
                }
            }
            else
            {
                var tilesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"DataFiles\Tiles.xml");
                TileSet = WorldTools.TileSetBuilder(tilesPath);
                world = WorldTools.GenerateWorld(0, 128, 128, TileSet, "World", 24);
                var pt = PlayerTools.GetSpawn(world, MapArea.Width / 2, MapArea.Height / 2);
                player = PlayerTools.RandomPlayer(pt.X, pt.Y);
            }

            Camera = new Camera(0, 0, MapArea, world, player);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            UserInterface.Initialize(Content, BuiltinThemes.hd);
            //Draw UI here
            //Right Panel
            Panel rightPanel = new Panel(new Vector2(Convert.ToInt32((ScreenArea.Width * .5) - 16), Convert.ToInt32(ScreenArea.Height)), PanelSkin.Default, Anchor.TopRight);
            UserInterface.AddEntity(rightPanel);
            rightPanel.AddChild(new Header("Example Panel"));
            rightPanel.AddChild(new HorizontalLine());
            rightPanel.AddChild(new Paragraph("This is a simple panel with a button."));
            rightPanel.AddChild(new Button("Click Me!", ButtonSkin.Default, Anchor.BottomCenter));
            //Bottom Panel
            Panel bottomPanel = new Panel(new Vector2(Convert.ToInt32((ScreenArea.Width * .5) + (16 - Convert.ToInt32((ScreenArea.Height * .3) - 16))), Convert.ToInt32((ScreenArea.Height * .3) - 16)), PanelSkin.Default, Anchor.BottomRight, new Vector2(Convert.ToInt32((ScreenArea.Width * .5) - 16), 0));
            UserInterface.AddEntity(bottomPanel);
            bottomPanel.AddChild(new Header("Example Panel"));
            bottomPanel.AddChild(new HorizontalLine());
            bottomPanel.AddChild(new Paragraph("This is a simple panel with a button."));
            bottomPanel.AddChild(new Button("Click Me!", ButtonSkin.Default, Anchor.BottomCenter));

            //Add console commands here
            interpreter.AddVariable("player", player);
            interpreter.AddVariable("inventory", player.Inventory);
            interpreter.AddVariable("world", world);
            interpreter.AddVariable("this", this);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            foreach (Tile tile in TileSet)
            {
                try
                {
                    TileTextures.Add(tile.EntityName, Content.Load<Texture2D>(tile.EntityName));
                }
                catch
                {
                    TileTextures.Add(tile.EntityName, Content.Load<Texture2D>("TileMissing"));
                }
            }
        }

        protected override void UnloadContent() { }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();  // get the newest state

            // handle the input
            if (oldState.IsKeyUp(Keys.Left) && newState.IsKeyDown(Keys.Left))
            {
                player.MovePlayer(-1, 0, world);
                if (PlayerLoc)
                    Console.WriteLine("X: " + player.locationX + ", Y: " + player.locationY);
            }

            if (oldState.IsKeyUp(Keys.Right) && newState.IsKeyDown(Keys.Right))
            {
                player.MovePlayer(1, 0, world);
                if (PlayerLoc)
                    Console.WriteLine("X: " + player.locationX + ", Y: " + player.locationY);

            }
            if (oldState.IsKeyUp(Keys.Up) && newState.IsKeyDown(Keys.Up))
            {
                player.MovePlayer(0, -1, world);
                if (PlayerLoc)
                    Console.WriteLine("X: " + player.locationX + ", Y: " + player.locationY);
            }
            if (oldState.IsKeyUp(Keys.Down) && newState.IsKeyDown(Keys.Down))
            {
                player.MovePlayer(0, 1, world);
                if (PlayerLoc)
                    Console.WriteLine("X: " + player.locationX + ", Y: " + player.locationY);
            }

            if (oldState.IsKeyUp(Keys.OemTilde) && newState.IsKeyDown(Keys.OemTilde))
                console.ToggleOpenClose();


            oldState = newState;  // set the new state as the old state for next time

            UserInterface.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            World DrawArea = Camera.GetDrawArea();
            System.Drawing.Point pt = Camera.playerCoord;

            spriteBatch.Begin();

            for (int i = 0; i < MapArea.Width; i++)
            {
                for (int j = 0; j < MapArea.Height; j++)
                {
                    spriteBatch.Draw(TileTextures[DrawArea.Grid[i, j].EntityName], new Rectangle(i * world.TileSize, j * world.TileSize, world.TileSize, world.TileSize), Color.White);
                }
            }

            Texture2D PlayerTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            PlayerTexture.SetData<Color>(new Color[] { Color.Red });
            spriteBatch.Draw(PlayerTexture, new Rectangle(pt.X * world.TileSize, pt.Y * world.TileSize, world.TileSize, world.TileSize), Color.Red);

            spriteBatch.DrawString(Content.Load<SpriteFont>("DevConsoleFont"), "Minimap goes here :)", new Vector2(20, ScreenArea.Height - 100), Color.Black);

            spriteBatch.End();

            UserInterface.Draw(spriteBatch);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Hooks into the OnExiting event to save the game.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            var status = Persistance.SaveGame(world, player);
            if (status == 0)
            {
                System.Windows.Forms.MessageBox.Show("Save Failed. Do you want to close?", "Error", System.Windows.Forms.MessageBoxButtons.RetryCancel);
            }
        }

        /// <summary>
        /// Clears the console
        /// </summary>
        public void Clear()
        {
            console.Clear();
        }

        /// <summary>
        /// Quits the game
        /// </summary>
        public void Quit()
        {
            Exit();
        }

        /// <summary>
        /// Creates a new item
        /// </summary>
        /// <param name="s">Type of item</param>
        /// <param name="id">Item ID</param>
        /// <param name="name">Item name</param>
        /// <param name="weight">Item weight</param>
        /// <param name="args">Optional parameters as needed by classes</param>
        /// <returns></returns>
        public CURPG_Engine.Inventory.Item NewItem(string s, int id, string name, int weight, string[] args = null)
        {
            switch(s)
            {
                case "tool":
                    CURPG_Engine.Inventory.Tool tool = new CURPG_Engine.Inventory.Tool(id, name, weight, Convert.ToInt32(args[0]));
                    return tool;
            }

            return null;
        }
    }
}