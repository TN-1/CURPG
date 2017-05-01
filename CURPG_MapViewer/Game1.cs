using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CURPG_Engine.Core;

namespace CURPG_MapViewer
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        // ReSharper disable once NotAccessedField.Local
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly World _world;
        private List<Tile> _tileset;
        private Camera _camera;
        private System.Drawing.Rectangle _mapArea;
        private System.Drawing.Rectangle _screenArea;
        private System.Drawing.Point _pt;
        private readonly Dictionary<string, Texture2D> _tileTextures = new Dictionary<string, Texture2D>();
        private KeyboardState _oldState;
        private Texture2D _debugTexture;
        private SpriteFont _debugFont;

        public Game1(World world)
        {
            var pt = new System.Drawing.Point(0, 0);
            _screenArea = System.Windows.Forms.Screen.GetWorkingArea(pt);
            _graphics = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
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

            _mapArea.Height = (int)Math.Ceiling(_screenArea.Height / 24f);
            _mapArea.Width = (int)Math.Ceiling(_screenArea.Width / 24f);

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
            _debugTexture.SetData(new[] { Color.Black });
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
                throw new NotImplementedException();
            if (_oldState.IsKeyUp(Keys.PageUp) && newState.IsKeyDown(Keys.PageUp))
                throw new NotImplementedException();

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
            for (var i = 0; i < _mapArea.Width; i++)
            {
                for (var j = 0; j < _mapArea.Height; j++)
                {
                    _spriteBatch.Draw(_tileTextures[drawArea.Grid[i, j].EntityName],
                        new Rectangle(i * _world.TileSize, j * _world.TileSize, _world.TileSize, _world.TileSize),
                        Color.White);
                }
            }

            _spriteBatch.Draw(_debugTexture, new Rectangle(0, 0, _screenArea.Width, 60), Color.Black * .7f);
            _spriteBatch.DrawString(_debugFont, "Move: [Arrow Keys]     Zoom Out: [PGDN]    Zoom In: [PGUP]", new Vector2(20, 20), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
