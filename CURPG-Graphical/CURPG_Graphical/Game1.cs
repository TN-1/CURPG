using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using CURPG_Engine.Core;
using System;
using System.Xml;
using System.IO;
using System.Reflection;

namespace CURPG_Graphical
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
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

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var tilesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"DataFiles\Tiles.xml");
            TileSet = WorldTools.TileSetBuilder(tilesPath);
            world = WorldTools.GenerateWorld(0, 128, 128, TileSet, "World", 24);
            var pt = PlayerTools.GetSpawn(world, MapArea.Width / 2, MapArea.Height / 2);
            player = PlayerTools.RandomPlayer(pt.X, pt.Y);
            Camera = new Camera(0, 0, MapArea, world, player);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            foreach (Tile tile in TileSet)
            {
                try
                {
                    TileTextures.Add(tile.EntityName, Content.Load<Texture2D>(tile.EntityName));
                }
                catch
                {
                    //TODO: Add Debug to log
                    try
                    {
                        TileTextures.Add(tile.EntityName, Content.Load<Texture2D>("TileMissing"));
                    }
                    catch { }
                }
            }

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState newState = Keyboard.GetState();  // get the newest state

            // handle the input
            if (oldState.IsKeyUp(Keys.Left) && newState.IsKeyDown(Keys.Left))
                player.MovePlayer(-1, 0, world);
            if (oldState.IsKeyUp(Keys.Right) && newState.IsKeyDown(Keys.Right))
                player.MovePlayer(1, 0, world);
            if (oldState.IsKeyUp(Keys.Up) && newState.IsKeyDown(Keys.Up))
                player.MovePlayer(0, -1, world);
            if (oldState.IsKeyUp(Keys.Down) && newState.IsKeyDown(Keys.Down))
                player.MovePlayer(0, 1, world);


            oldState = newState;  // set the new state as the old state for next time

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
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

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}