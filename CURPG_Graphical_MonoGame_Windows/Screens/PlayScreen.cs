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
    [Serializable]
    public partial class PlayScreen : GameScreen
    {
        [NonSerialized] private readonly Dictionary<string, Texture2D> _tileTextures = new Dictionary<string, Texture2D>();
        public World World;
        private List<Tile> _tileSet;
        public Player Player;
        [NonSerialized] private KeyboardState _oldState;
        [NonSerialized] private System.Drawing.Rectangle _mapArea;
        [NonSerialized] private Camera _camera;
        [NonSerialized] private Panel _bottomPanel;
        [NonSerialized] private Panel _rightPanel;
        [NonSerialized] private PanelTabs _tabs;
        [NonSerialized] private PanelTabs.TabData _invTab;
        [NonSerialized] private PanelTabs.TabData _statTab;

        [NonSerialized] private readonly Lua _lua = new Lua();

        public override void Initialize()
        {
            _mapArea.Height = (int)Math.Ceiling((ScreenManager.ScreenArea.Height * .7) / 24);
            _mapArea.Width = (int)Math.Ceiling((ScreenManager.ScreenArea.Width * .5) / 24);

            if (Persistance.CanLoad())
            {
                World = Persistance.LoadWorld();
                Player = Persistance.LoadPlayer();
                _tileSet = World.TileSet;
                var loc = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                if (loc == null) throw new Exception("Loc is null");
                var itemsPath = Path.Combine(loc, @"DataFiles\Items.xml");
                Player.Inventory.BuildDatabase(itemsPath);

                if (World == null || Player == null)
                {
                    loc = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    if(loc == null) throw new Exception("Loc is null");
                    var tilesPath = Path.Combine(loc, @"DataFiles\Tiles.xml");
                    itemsPath = Path.Combine(loc, @"DataFiles\Items.xml");
                    _tileSet = WorldTools.TileSetBuilder(tilesPath);
                    World = WorldTools.GenerateWorld(0, 128, 128, _tileSet, "World", 24);
                    var pt = PlayerTools.GetSpawn(World, _mapArea.Width / 2, _mapArea.Height / 2);
                    Player = PlayerTools.RandomPlayer(pt.X, pt.Y);
                    Player.Inventory.BuildDatabase(itemsPath);
                }
            }
            else
            {
                var loc = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                if (loc == null) throw new Exception("Loc is null");
                var tilesPath = Path.Combine(loc, @"DataFiles\Tiles.xml");
                var itemsPath = Path.Combine(loc, @"DataFiles\Items.xml");
                _tileSet = WorldTools.TileSetBuilder(tilesPath);
                World = WorldTools.GenerateWorld(0, 128, 128, _tileSet, "World", 24);
                var pt = PlayerTools.GetSpawn(World, _mapArea.Width / 2, _mapArea.Height / 2);
                Player = PlayerTools.RandomPlayer(pt.X, pt.Y);
                Player.Inventory.BuildDatabase(itemsPath);
            }

            _camera = new Camera(0, 0, _mapArea, World, Player);

            Ui();

            //Add console functions
            ScreenManager.Interpreter.AddVariable("player", Player);
            ScreenManager.Interpreter.AddVariable("inventory", Player.Inventory);
            ScreenManager.Interpreter.AddVariable("world", World);
            ScreenManager.Interpreter.AddVariable("this", this);

            //Setup lua
            _lua.LoadCLRPackage();
            _lua["this"] = this;
            
            base.Initialize();
        }

        public override void LoadAssets()
        {
            foreach (var tile in _tileSet)
            {
                try
                {
                    _tileTextures.Add(tile.EntityName, ScreenManager.ContentMgr.Load<Texture2D>(tile.EntityName));
                }
                catch
                {
                    try
                    {
                        _tileTextures.Add(tile.EntityName, ScreenManager.ContentMgr.Load<Texture2D>("TileMissing"));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            base.LoadAssets();
        }

        public override void Update(GameTime gameTime)
        {
            var newState = Keyboard.GetState();  // get the newest state

            Player.Inventory.PropertyChanged += Inventory_PropertyChanged;

            // handle the input
            if (_oldState.IsKeyUp(Keys.Left) && newState.IsKeyDown(Keys.Left))
            {
                Player.MovePlayer(-1, 0, World);
            }

            if (_oldState.IsKeyUp(Keys.Right) && newState.IsKeyDown(Keys.Right))
            {
                Player.MovePlayer(1, 0, World);

            }
            if (_oldState.IsKeyUp(Keys.Up) && newState.IsKeyDown(Keys.Up))
            {
                Player.MovePlayer(0, -1, World);
            }
            if (_oldState.IsKeyUp(Keys.Down) && newState.IsKeyDown(Keys.Down))
            {
                Player.MovePlayer(0, 1, World);
            }

            _oldState = newState;  // set the new state as the old state for next time

            UserInterface.Update(gameTime);

            base.Update(gameTime);
        }

        private void Inventory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DrawInv();
        }

        public override void Draw(GameTime gameTime)
        {
            World drawArea = _camera.GetDrawArea();
            System.Drawing.Point pt = _camera.PlayerCoord;

            ScreenManager.Sprites.Begin();

            for (int i = 0; i < _mapArea.Width; i++)
            {
                for (int j = 0; j < _mapArea.Height; j++)
                {
                    ScreenManager.Sprites.Draw(_tileTextures[drawArea.Grid[i, j].EntityName], new Rectangle(i * World.TileSize, j * World.TileSize, World.TileSize, World.TileSize), Color.White);
                }
            }

            Texture2D playerTexture = new Texture2D(ScreenManager.GraphicsDeviceMgr.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            playerTexture.SetData(new[] { Color.Red });
            ScreenManager.Sprites.Draw(playerTexture, new Rectangle(pt.X * World.TileSize, pt.Y * World.TileSize, World.TileSize, World.TileSize), Color.Red);

            ScreenManager.Sprites.DrawString(ScreenManager.ContentMgr.Load<SpriteFont>("DevConsoleFont"), "Minimap goes here :)", new Vector2(20, ScreenManager.ScreenArea.Height - 100), Color.Black);

            ScreenManager.Sprites.End();
            UserInterface.Draw(ScreenManager.Sprites);

            base.Draw(gameTime);
        }
    }
}