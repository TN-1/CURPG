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
using NLua;

namespace CURPG_Graphical_MonoGame_Windows.Screens
{
    public partial class PlayScreen : GameScreen
    {
        Dictionary<string, Texture2D> TileTextures = new Dictionary<string, Texture2D>();
        public World world;
        List<Tile> TileSet;
        public Player player;
        private KeyboardState oldState;
        System.Drawing.Rectangle MapArea;
        Camera Camera;
        bool PlayerLoc;
        Panel bottomPanel;
        Panel rightPanel;
        Lua lua = new Lua();
        public new Color BackgroundColor = Color.White;

        public override void Initialize()
        {
            MapArea.Height = (int)Math.Ceiling((ScreenManager.ScreenArea.Height * .7) / 24);
            MapArea.Width = (int)Math.Ceiling((ScreenManager.ScreenArea.Width * .5) / 24);

            if (Persistance.CanLoad())
            {
                world = Persistance.LoadWorld();
                player = Persistance.LoadPlayer();
                TileSet = world.TileSet;

                if (world == null || player == null)
                {
                    var tilesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"DataFiles\Tiles.xml");
                    var itemsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"DataFiles\Items.xml");
                    TileSet = WorldTools.TileSetBuilder(tilesPath);
                    world = WorldTools.GenerateWorld(0, 128, 128, TileSet, "World", 24);
                    var pt = PlayerTools.GetSpawn(world, MapArea.Width / 2, MapArea.Height / 2);
                    player = PlayerTools.RandomPlayer(pt.X, pt.Y);
                    player.Inventory.BuildDatabase(itemsPath);
                }
            }
            else
            {
                var tilesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"DataFiles\Tiles.xml");
                var itemsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"DataFiles\Items.xml");
                TileSet = WorldTools.TileSetBuilder(tilesPath);
                world = WorldTools.GenerateWorld(0, 128, 128, TileSet, "World", 24);
                var pt = PlayerTools.GetSpawn(world, MapArea.Width / 2, MapArea.Height / 2);
                player = PlayerTools.RandomPlayer(pt.X, pt.Y);
                player.Inventory.BuildDatabase(itemsPath);
            }

            Camera = new Camera(0, 0, MapArea, world, player);

            UserInterface.Initialize(ScreenManager.ContentMgr, BuiltinThemes.hd);
            //Draw UI here
            //Right Panel
            rightPanel = new Panel(new Vector2(Convert.ToInt32((ScreenManager.ScreenArea.Width * .5) - 16), Convert.ToInt32(ScreenManager.ScreenArea.Height - 70)), PanelSkin.Default, Anchor.TopRight, offset: new Vector2(0, 70));
            PanelTabs tabs = new PanelTabs();
            PanelTabs.TabData invTab = tabs.AddTab("Inventory");
            PanelTabs.TabData statTab = tabs.AddTab("Skills");
            invTab.panel.AddChild(new Header("Hello inventory!"));
            statTab.panel.AddChild(new Header("Hello stats!"));
            rightPanel.AddChild(tabs);
            UserInterface.AddEntity(rightPanel);
            //Bottom Panel
            bottomPanel = new Panel(new Vector2(Convert.ToInt32((ScreenManager.ScreenArea.Width * .5) + (16 - Convert.ToInt32((ScreenManager.ScreenArea.Height * .3) - 16))), Convert.ToInt32((ScreenManager.ScreenArea.Height * .3) - 16)), PanelSkin.Default, Anchor.BottomRight, new Vector2(Convert.ToInt32((ScreenManager.ScreenArea.Width * .5) - 16), 0));
            UserInterface.AddEntity(bottomPanel);

            //Add console functions
            ScreenManager.interpreter.AddVariable("player", player);
            ScreenManager.interpreter.AddVariable("inventory", player.Inventory);
            ScreenManager.interpreter.AddVariable("world", world);
            ScreenManager.interpreter.AddVariable("this", this);

            //Setup lua
            lua.LoadCLRPackage();
            lua["this"] = this;
            
            base.Initialize();
        }

        public override void LoadAssets()
        {
            foreach (Tile tile in TileSet)
            {
                try
                {
                    TileTextures.Add(tile.EntityName, ScreenManager.ContentMgr.Load<Texture2D>(tile.EntityName));
                }
                catch
                {
                    try
                    {
                        TileTextures.Add(tile.EntityName, ScreenManager.ContentMgr.Load<Texture2D>("TileMissing"));
                    }
                    catch { }
                }
            }

            base.LoadAssets();
        }

        public override void Update(GameTime gameTime)
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
                ScreenManager.console.ToggleOpenClose();


            oldState = newState;  // set the new state as the old state for next time

            UserInterface.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            World DrawArea = Camera.GetDrawArea();
            System.Drawing.Point pt = Camera.playerCoord;

            ScreenManager.Sprites.Begin();

            for (int i = 0; i < MapArea.Width; i++)
            {
                for (int j = 0; j < MapArea.Height; j++)
                {
                    ScreenManager.Sprites.Draw(TileTextures[DrawArea.Grid[i, j].EntityName], new Rectangle(i * world.TileSize, j * world.TileSize, world.TileSize, world.TileSize), Color.White);
                }
            }

            Texture2D PlayerTexture = new Texture2D(ScreenManager.GraphicsDeviceMgr.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            PlayerTexture.SetData<Color>(new Color[] { Color.Red });
            ScreenManager.Sprites.Draw(PlayerTexture, new Rectangle(pt.X * world.TileSize, pt.Y * world.TileSize, world.TileSize, world.TileSize), Color.Red);

            ScreenManager.Sprites.DrawString(ScreenManager.ContentMgr.Load<SpriteFont>("DevConsoleFont"), "Minimap goes here :)", new Vector2(20, ScreenManager.ScreenArea.Height - 100), Color.Black);

            ScreenManager.Sprites.End();
            UserInterface.Draw(ScreenManager.Sprites);

            base.Draw(gameTime);
        }
    }
}