using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CURPG_Engine.Core;
using CURPG_Engine.Scriptables;
using System;
using System.IO;
using System.Reflection;
using GeonBit.UI;
using GeonBit.UI.Entities;
using NLua;
using System.Linq;

namespace CURPG_Graphical_MonoGame_Windows.Screens
{
    [Serializable]
    public partial class PlayScreen : GameScreen
    {
        [NonSerialized] private readonly Dictionary<string, Texture2D> _tileTextures = new Dictionary<string, Texture2D>();
        public World World;
        private List<Tile> _tileSet;
        public Player Player;
        [NonSerialized] private List<Npc> _npcs;
        [NonSerialized] private KeyboardState _oldState;
        [NonSerialized] private System.Drawing.Rectangle _mapArea;
        [NonSerialized] private Camera _camera;
        [NonSerialized] private Panel _bottomPanel;
        [NonSerialized] private Panel _rightPanel;
        [NonSerialized] private PanelTabs _tabs;
        [NonSerialized] private PanelTabs.TabData _invTab;
        [NonSerialized] private PanelTabs.TabData _statTab;
        [NonSerialized] private float _timeSinceLastUpdate;
        [NonSerialized] private readonly string _exeLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        [NonSerialized] private readonly Lua _lua = new Lua();

        public override void Initialize()
        {
            if (_exeLocation == null) throw new Exception("_exeLocation is nill");
            _mapArea.Height = (int)Math.Ceiling((ScreenManager.ScreenArea.Height * .7) / 24);
            _mapArea.Width = (int)Math.Ceiling((ScreenManager.ScreenArea.Width * .5) / 24);

            if (Persistance.CanLoad())
            {
                World = Persistance.LoadWorld();
                Player = Persistance.LoadPlayer();
                _tileSet = World.TileSet;
                var itemsPath = Path.Combine(_exeLocation, @"DataFiles\Items.xml");
                Player.Inventory.BuildDatabase(itemsPath);

                if (World == null || Player == null)
                {
                    var tilesPath = Path.Combine(_exeLocation, @"DataFiles\Tiles.xml");
                    itemsPath = Path.Combine(_exeLocation, @"DataFiles\Items.xml");
                    _tileSet = WorldTools.TileSetBuilder(tilesPath);
                    World = WorldTools.GenerateWorld(0, 128, 128, _tileSet, "World", 24);
                    var pt = PlayerTools.GetSpawn(World, _mapArea.Width / 2, _mapArea.Height / 2);
                    Player = PlayerTools.RandomPlayer(pt.X, pt.Y);
                    Player.Inventory.BuildDatabase(itemsPath);
                }
            }
            else
            {
                var tilesPath = Path.Combine(_exeLocation, @"DataFiles\Tiles.xml");
                var itemsPath = Path.Combine(_exeLocation, @"DataFiles\Items.xml");
                _tileSet = WorldTools.TileSetBuilder(tilesPath);
                World = WorldTools.GenerateWorld(0, 128, 128, _tileSet, "World", 24);
                var pt = PlayerTools.GetSpawn(World, _mapArea.Width / 2, _mapArea.Height / 2);
                Player = PlayerTools.RandomPlayer(pt.X, pt.Y);
                Player.Inventory.BuildDatabase(itemsPath);
            }

            _camera = new Camera(0, 0, _mapArea, World, Player);
            _npcs = new List<Npc>();
            Ui();

            //Add console functions
            ScreenManager.Interpreter.AddVariable("player", Player);
            ScreenManager.Interpreter.AddVariable("inventory", Player.Inventory);
            ScreenManager.Interpreter.AddVariable("world", World);
            ScreenManager.Interpreter.AddVariable("this", this);
            ScreenManager.Interpreter.AddVariable("lua", _lua);

            //Setup lua luanet.load_assembly("CURPG_Engine.Scriptables")
            _lua.LoadCLRPackage();
            _lua["this"] = this;
            _lua["state"] = 0;
            _lua.DoFile(Path.Combine(_exeLocation, "Scripts", "DefineNPCs.lua"));
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
            _timeSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timeSinceLastUpdate > 2f)
            {
                foreach (Npc npc in _npcs)
                {
                    npc.Update();
                }
                _timeSinceLastUpdate = 0f;
            }

            // handle the input
            if (_oldState.IsKeyUp(Keys.Left) && newState.IsKeyDown(Keys.Left))
            {
                if(Player.MovePlayer(-1, 0, World))
                    CheckForNpcInteraction();
            }

            if (_oldState.IsKeyUp(Keys.Right) && newState.IsKeyDown(Keys.Right))
            {
                if(Player.MovePlayer(1, 0, World))
                    CheckForNpcInteraction();
            }
            if (_oldState.IsKeyUp(Keys.Up) && newState.IsKeyDown(Keys.Up))
            {
                if(Player.MovePlayer(0, -1, World))
                    CheckForNpcInteraction();
            }
            if (_oldState.IsKeyUp(Keys.Down) && newState.IsKeyDown(Keys.Down))
            {
                if(Player.MovePlayer(0, 1, World))
                    CheckForNpcInteraction();
            }

            _oldState = newState;  // set the new state as the old state for next time

            UserInterface.Update(gameTime);

            base.Update(gameTime);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void CheckForNpcInteraction()
        {
            foreach (var npc in _npcs)
            {
                var x = Player.LocationX - npc.LocationX;
                var y = Player.LocationY - npc.LocationY;
                if (Enumerable.Range(-2, 4).Contains(x) && Enumerable.Range(-2, 4).Contains(y))
                {
                    _lua.DoFile(Path.Combine(_exeLocation, "Scripts", "NPC", npc.Index.ToString(), "Dialogue.lua"));
                    _lua["Player"] = Player;
                    _lua["NPC"] = npc;
                    var greeting = _lua["Greeting"] as LuaFunction;
                    if (greeting == null) throw new Exception("greeting() is null!");
                    greeting.Call();
                }
            }
        }

        private void Inventory_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DrawInv();
        }

        public override void Draw(GameTime gameTime)
        {
            _camera.GetNpCs(_npcs);
            var drawArea = _camera.GetDrawArea();
            var pt = _camera.PlayerCoord;
            var npcpt = _camera.NpcCoord;

            ScreenManager.Sprites.Begin();

            for (var i = 0; i < _mapArea.Width; i++)
            {
                for (var j = 0; j < _mapArea.Height; j++)
                {
                    ScreenManager.Sprites.Draw(_tileTextures[drawArea.Grid[i, j].EntityName], new Rectangle(i * World.TileSize, j * World.TileSize, World.TileSize, World.TileSize), Color.White);
                }
            }

            var playerTexture = new Texture2D(ScreenManager.GraphicsDeviceMgr.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            var npcTexture = new Texture2D(ScreenManager.GraphicsDeviceMgr.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);

            playerTexture.SetData(new[] { Color.Red });
            npcTexture.SetData(new[] { Color.HotPink });

            ScreenManager.Sprites.Draw(playerTexture, new Rectangle(pt.X * World.TileSize, pt.Y * World.TileSize, World.TileSize, World.TileSize), Color.Red);
            if(npcpt != null)
                foreach (var npt in npcpt)
                {
                    ScreenManager.Sprites.Draw(npcTexture, new Rectangle(npt.X * World.TileSize, npt.Y * World.TileSize, World.TileSize, World.TileSize), Color.HotPink);
                }
            ScreenManager.Sprites.DrawString(ScreenManager.ContentMgr.Load<SpriteFont>("DevConsoleFont"), "Minimap goes here :)", new Vector2(20, ScreenManager.ScreenArea.Height - 100), Color.Black);

            ScreenManager.Sprites.End();

            UserInterface.Draw(ScreenManager.Sprites);

            base.Draw(gameTime);
        }

        public void AddNpc(double index, string name, string gender, double age, double height, double weight, double x, double y, double maxx, double maxy)
        {
            _npcs.Add(new Npc((int)index, name, gender.ToCharArray()[0], (int)age, (int)height, (int)weight, (int)x, (int)y, (int)maxx, (int)maxy, World));
        }
    }
}