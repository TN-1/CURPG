﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CURPG_Engine.Core;
// ReSharper disable NotAccessedField.Local

namespace CURPG_MapViewer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        private readonly Dictionary<string, Texture2D> _tileTextures = new Dictionary<string, Texture2D>();
        private readonly World _world;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<Tile> _tileset;
        private Camera _camera;
        private System.Drawing.Rectangle _mapArea;
        private System.Drawing.Rectangle _screenArea;
        private System.Drawing.Point _pt;
        private KeyboardState _oldState;
        private Texture2D _debugTexture;
        private Texture2D _pixelTexture;
        private SpriteFont _debugFont;
        private bool _hm;

        public Game1(World world)
        {
            var pt = new System.Drawing.Point(0, 0);
            _screenArea = System.Windows.Forms.Screen.GetWorkingArea(pt);
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = true,
                PreferredBackBufferHeight = _screenArea.Height,
                PreferredBackBufferWidth = _screenArea.Width
            };
            Window.Title = "CURPG Map Viewer";
            Window.AllowUserResizing = false;
            Content.RootDirectory = "Content";
            _world = world;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _tileset = _world.TileSet;
            if (_world == null)
                throw new NotImplementedException();

            _mapArea.Height = (int)Math.Ceiling(_screenArea.Height / (float)_world.TileSize);
            _mapArea.Width = (int)Math.Ceiling(_screenArea.Width / (float)_world.TileSize);

            _pt = new System.Drawing.Point(_mapArea.Width / 2, _mapArea.Height / 2);
            _camera = new Camera(0, 0, _mapArea, _world, _pt);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (var tile in _tileset)
            {
                try
                {
                    _tileTextures.Add(tile.EntityName, Content.Load<Texture2D>(tile.EntityName));
                }
                catch
                {
                    try
                    {
                        _tileTextures.Add(tile.EntityName, Content.Load<Texture2D>("TileMissing"));
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            _debugTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _debugTexture.SetData(new[] { Color.Black });
            _pixelTexture.SetData(new[] { Color.White });
            _debugFont = Content.Load<SpriteFont>("Arial");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var newState = Keyboard.GetState();  // get the newest state

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_oldState.IsKeyUp(Keys.Left) && newState.IsKeyDown(Keys.Left))
                _pt = new System.Drawing.Point(_pt.X - 1, _pt.Y);
            if (_oldState.IsKeyUp(Keys.Right) && newState.IsKeyDown(Keys.Right))
                _pt = new System.Drawing.Point(_pt.X + 1, _pt.Y);
            if (_oldState.IsKeyUp(Keys.Up) && newState.IsKeyDown(Keys.Up))
                _pt = new System.Drawing.Point(_pt.X, _pt.Y - 1);
            if (_oldState.IsKeyUp(Keys.Down) && newState.IsKeyDown(Keys.Down))
                _pt = new System.Drawing.Point(_pt.X, _pt.Y + 1);
            if (_oldState.IsKeyUp(Keys.PageDown) && newState.IsKeyDown(Keys.PageDown))
            {
                _world.TileSize = _world.TileSize - 2;
                _mapArea.Height = (int)Math.Ceiling(_screenArea.Height / (float)_world.TileSize);
                _mapArea.Width = (int)Math.Ceiling(_screenArea.Width / (float)_world.TileSize);
                if (_mapArea.Height > _world.Grid.GetLength(0) || _mapArea.Width > _world.Grid.GetLength(1))
                {
                    _world.TileSize = _world.TileSize + 2;
                    _mapArea.Height = (int) Math.Ceiling(_screenArea.Height / (float) _world.TileSize);
                    _mapArea.Width = (int) Math.Ceiling(_screenArea.Width / (float) _world.TileSize);
                }
            }
            if (_oldState.IsKeyUp(Keys.PageUp) && newState.IsKeyDown(Keys.PageUp))
            {
                _world.TileSize = _world.TileSize + 2;
                _mapArea.Height = (int)Math.Ceiling(_screenArea.Height / (float)_world.TileSize);
                _mapArea.Width = (int)Math.Ceiling(_screenArea.Width / (float)_world.TileSize);
            }
            if (_oldState.IsKeyUp(Keys.F5) && newState.IsKeyDown(Keys.F5))
            {
                _hm = !_hm;
            }

                _oldState = newState;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            var drawArea = _camera.GetViewerDrawArea(_pt);

            _spriteBatch.Begin();

            if(!_hm)
                for (var i = 0; i < _mapArea.Width; i++)
                {
                    for (var j = 0; j < _mapArea.Height; j++)
                    {
                        if (_world.TileSize == 24)
                            _spriteBatch.Draw(_tileTextures[drawArea.Grid[i, j].EntityName],
                                new Rectangle(i * _world.TileSize, j * _world.TileSize, _world.TileSize, _world.TileSize),
                                Color.White);
                        else
                            _spriteBatch.Draw(_pixelTexture,
                                new Rectangle(i * _world.TileSize, j * _world.TileSize, _world.TileSize, _world.TileSize),
                                _world.Grid[i, j].TileColor);
                    }
                }
            else
                for (var i = 0; i < _mapArea.Width; i++)
                {
                    for (var j = 0; j < _mapArea.Height; j++)
                    {
                        double v = _world.Grid[i,j].NoiseVal / 2.54;
                        v = v / 100;
                        var c = ColorFromHsv(0, 0, v);
                        _spriteBatch.Draw(_pixelTexture,
                            new Rectangle(i * _world.TileSize, j * _world.TileSize, _world.TileSize, _world.TileSize),
                            new Color(c.R, c.G, c.B));
                    }
                }


            _spriteBatch.Draw(_debugTexture, new Rectangle(0, 0, _screenArea.Width, 60), Color.Black * .7f);
            _spriteBatch.DrawString(_debugFont,
                $"Move: [Arrow Keys]     Zoom Out: [PGDN]    Zoom In: [PGUP]    HMView: [F5]     TS: {_world.TileSize}", 
                new Vector2(20, 20), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private static System.Drawing.Color ColorFromHsv(double hue, double saturation, double value)
        {
            var hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            var f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            var v = Convert.ToInt32(value);
            var p = Convert.ToInt32(value * (1 - saturation));
            var q = Convert.ToInt32(value * (1 - f * saturation));
            var t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0:
                    return System.Drawing.Color.FromArgb(255, v, t, p);
                case 1:
                    return System.Drawing.Color.FromArgb(255, q, v, p);
                case 2:
                    return System.Drawing.Color.FromArgb(255, p, v, t);
                case 3:
                    return System.Drawing.Color.FromArgb(255, p, q, v);
                case 4:
                    return System.Drawing.Color.FromArgb(255, t, p, v);
                default:
                    return System.Drawing.Color.FromArgb(255, v, p, q);
            }
        }

    }
}