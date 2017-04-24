using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CURPG_Engine.Core;
using CURPG_Engine.Scriptables;
using System;
using System.Diagnostics;
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
        public World World;
        public Player Player;
        [NonSerialized] private List<Tile> _tileSet;
        [NonSerialized] private readonly Dictionary<string, Texture2D> _tileTextures = new Dictionary<string, Texture2D>();
        [NonSerialized] private List<Npc> _npcs;
        [NonSerialized] private KeyboardState _oldState;
        [NonSerialized] private System.Drawing.Rectangle _mapArea;
        [NonSerialized] private System.Drawing.Rectangle _miniMapArea;
        [NonSerialized] private Camera _camera;
        [NonSerialized] private Panel _bottomPanel;
        [NonSerialized] private Panel _rightPanel;
        [NonSerialized] private PanelTabs _tabs;
        [NonSerialized] private PanelTabs.TabData _invTab;
        [NonSerialized] private PanelTabs.TabData _statTab;
        [NonSerialized] private float _timeSinceLastUpdate;
        [NonSerialized] private readonly string _exeLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        [NonSerialized] private readonly Lua _lua = new Lua();
        [NonSerialized] private Texture2D _playerTexture;
        [NonSerialized] private Texture2D _npcTexture;
        [NonSerialized] private Texture2D _pixelTexture;
        [NonSerialized] private Texture2D _debugTexture;
        [NonSerialized] private SpriteFont _debugFont;
        [NonSerialized] private bool _debug;
        [NonSerialized] private int _frameRate;
        [NonSerialized] private int _frameCounter;
        [NonSerialized] private int _miniMapZoom = 4;
        [NonSerialized] private TimeSpan _elapsedTime = TimeSpan.Zero;
        [NonSerialized] private readonly Process _currentProc = Process.GetCurrentProcess();




        public override void Initialize()
        {
            if (_exeLocation == null) throw new Exception("_exeLocation is null");
            _mapArea.Height = (int)Math.Ceiling((ScreenManager.ScreenArea.Height * .7) / 24);
            _mapArea.Width = (int)Math.Ceiling((ScreenManager.ScreenArea.Width * .7) / 24);
            _miniMapArea.Height = (int)(ScreenManager.ScreenArea.Height - (Math.Floor((ScreenManager.ScreenArea.Height * .7) / 24) * 24));
            _miniMapArea.Width = (int)(ScreenManager.ScreenArea.Height * .3 - 16);
            var tilesPath = Path.Combine(_exeLocation, @"DataFiles\Tiles.xml");
            var itemsPath = Path.Combine(_exeLocation, @"DataFiles\Items.xml");

            if (Persistance.CanLoad())
            {
                World = Persistance.LoadWorld();
                Player = Persistance.LoadPlayer();
                _tileSet = WorldTools.TileSetBuilder(tilesPath);
                Player.Inventory.BuildDatabase(itemsPath);

                if (World == null || Player == null)
                {
                    _tileSet = WorldTools.TileSetBuilder(tilesPath);
                    World = WorldTools.GenerateWorld(0, 500, 500, _tileSet, "World", 24);
                    var pt = PlayerTools.GetSpawn(World, _mapArea.Width / 2, _mapArea.Height / 2);
                    Player = PlayerTools.RandomPlayer(pt.X, pt.Y);
                    Player.Inventory.BuildDatabase(itemsPath);
                }
            }
            else
            {
                _tileSet = WorldTools.TileSetBuilder(tilesPath);
                World = WorldTools.GenerateWorld(0, 500, 500, _tileSet, "World", 24);
                var pt = PlayerTools.GetSpawn(World, _mapArea.Width / 2, _mapArea.Height / 2);
                Player = PlayerTools.RandomPlayer(pt.X, pt.Y);
                Player.Inventory.BuildDatabase(itemsPath);
            }

            _camera = new Camera(0, 0, _mapArea, _miniMapArea, World, Player);
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
            _lua.DoFile(Path.Combine(_exeLocation, "Scripts", "DefineNPCs.lua"));
            base.Initialize();

            Player.Inventory.PropertyChanged += Inventory_PropertyChanged;
        }

        public override void LoadAssets()
        {
            _playerTexture = new Texture2D(ScreenManager.GraphicsDeviceMgr.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _npcTexture = new Texture2D(ScreenManager.GraphicsDeviceMgr.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixelTexture = new Texture2D(ScreenManager.GraphicsDeviceMgr.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _debugTexture = new Texture2D(ScreenManager.GraphicsDeviceMgr.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _playerTexture.SetData(new[] { Color.Red });
            _npcTexture.SetData(new[] { Color.HotPink });
            _pixelTexture.SetData(new[] {Color.White});
            _debugTexture.SetData(new[] {Color.Black});
            _debugFont = ScreenManager.ContentMgr.Load<SpriteFont>("Arial");


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

            _timeSinceLastUpdate += (float)gameTime.ElapsedGameTime.TotalSeconds;
            _elapsedTime += gameTime.ElapsedGameTime;

            if (_timeSinceLastUpdate > 1f)
            {
                foreach (Npc npc in _npcs)
                {
                    npc.Update();
                }
                _timeSinceLastUpdate = 0f;
            }
            if (_elapsedTime > TimeSpan.FromSeconds(1))
            {
                _elapsedTime -= TimeSpan.FromSeconds(1);
                _frameRate = _frameCounter;
                _frameCounter = 0;
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
            if (_oldState.IsKeyUp(Keys.F3) && newState.IsKeyDown(Keys.F3))
            {
                //TODO: Make activation more reliable
                if (_debug)
                {
                    _debug = false;
                    return;
                }
                if (!_debug)
                {
                    _debug = true;
                    return;
                }
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
                if (Enumerable.Range(-3, 6).Contains(x) && Enumerable.Range(-3, 6).Contains(y))
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

        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;
            _camera.GetNpCs(_npcs);
            var drawArea = _camera.GetDrawArea();
            var miniMap = _camera.GetMiniMap(_miniMapZoom, _npcs);
            var pt = _camera.PlayerCoord;
            var npcpt = _camera.NpcCoord;
            var mpt = _camera.MiniPlayerCoord;
            var mnpcpt = _camera.MiniNPCCoord;

            ScreenManager.Sprites.Begin();

            for (var i = 0; i < _mapArea.Width; i++)
            {
                for (var j = 0; j < _mapArea.Height; j++)
                {
                    ScreenManager.Sprites.Draw(_tileTextures[drawArea.Grid[i, j].EntityName],
                        new Rectangle(i * World.TileSize, j * World.TileSize, World.TileSize, World.TileSize),
                        Color.White);
                }
            }

            for (var i = 0; i < _miniMapArea.Width; i++)
            {
                for (var j = 0; j < _miniMapArea.Height; j++)
                {
                    ScreenManager.Sprites.Draw(_pixelTexture,
                        new Rectangle(i * _miniMapZoom, _mapArea.Height * 24 + j * _miniMapZoom, _miniMapZoom, _miniMapZoom), miniMap[i, j]);
                }
            }

            ScreenManager.Sprites.Draw(_playerTexture,
                new Rectangle(mpt.X * _miniMapZoom, mpt.Y * _miniMapZoom + _mapArea.Height * 24, _miniMapZoom, _miniMapZoom), Color.Red);

            ScreenManager.Sprites.Draw(_playerTexture,
                new Rectangle(pt.X * World.TileSize, pt.Y * World.TileSize, World.TileSize, World.TileSize), Color.Red);

            if (npcpt != null)
                foreach (var npt in npcpt)
                {
                    ScreenManager.Sprites.Draw(_npcTexture,
                        new Rectangle(npt.X * World.TileSize, npt.Y * World.TileSize, World.TileSize, World.TileSize),
                        Color.HotPink);
                }

            if (mnpcpt != null)
                foreach (var npt in mnpcpt)
                {
                    ScreenManager.Sprites.Draw(_npcTexture,
                        new Rectangle(npt.X * _miniMapZoom, _mapArea.Height * 24 + npt.Y * _miniMapZoom, _miniMapZoom, _miniMapZoom),
                        Color.HotPink);
                }

            if (_debug)
            {
                string s;
                _currentProc.Refresh();
                ScreenManager.Sprites.Draw(_debugTexture,
                    new Rectangle(0, 0, _mapArea.Width * 24, (int) (ScreenManager.ScreenArea.Height * .3)),
                    Color.Black * .7f);
                ScreenManager.Sprites.DrawString(_debugFont, "FPS: " + _frameRate, new Vector2(20,20), Color.White);
                s = "PL: " + Player.LocationX + ", " + Player.LocationY;
                ScreenManager.Sprites.DrawString(_debugFont, s, new Vector2(20, 40), Color.White);
                if(npcpt != null) s = "NCount: " + npcpt.Count;
                else s = "NCount: null";
                ScreenManager.Sprites.DrawString(_debugFont, s, new Vector2(20, 60), Color.White);
                s = "Testing: " + Player.Testing;
                ScreenManager.Sprites.DrawString(_debugFont, s, new Vector2(20, 80), Color.White);
                s = "WDim: " + World.Grid.GetLength(0) + ", " + World.Grid.GetLength(1);
                ScreenManager.Sprites.DrawString(_debugFont, s, new Vector2(20, 100), Color.White);
                s = "TS: " + World.TileSize;
                ScreenManager.Sprites.DrawString(_debugFont, s, new Vector2(20, 120), Color.White);
                s = "MZ: " + _miniMapZoom;
                ScreenManager.Sprites.DrawString(_debugFont, s, new Vector2(20, 140), Color.White);
                s = "CURRMEM: " + GC.GetTotalMemory(false) / 1024f / 1024f + "MB"; 
                ScreenManager.Sprites.DrawString(_debugFont, s, new Vector2(20, 160), Color.White);
                s = "ALLOCMEM: " + _currentProc.PrivateMemorySize64 / 1024f / 1024f + "MB"; 
                ScreenManager.Sprites.DrawString(_debugFont, s, new Vector2(20, 180), Color.White);

            }

            ScreenManager.Sprites.End();

            UserInterface.Draw(ScreenManager.Sprites);

            base.Draw(gameTime);
        }
    }
}