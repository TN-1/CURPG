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
        Texture2D TileMissing;
        World world;
        List<Tile> TileSet;
        int TileSize;
        Player player;
        private KeyboardState oldState;

        public CURPG()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 384;
            graphics.PreferredBackBufferWidth = 384;
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
            TileSize = 24;
            var tilesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"DataFiles\Tiles.xml");
            XmlDocument tiles = new XmlDocument();
            tiles.Load(tilesPath);
            TileSet = WorldTools.TileSetBuilder(tiles);
            world = WorldTools.GenerateWorld(0, 16, 16, TileSet, "World");
            player = PlayerTools.RandomPlayer();

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
            TileMissing = Content.Load<Texture2D>("TileMissing");
            foreach (Tile tile in TileSet)
            {
                try
                {
                    TileTextures.Add(tile.EntityName, Content.Load<Texture2D>(tile.EntityName));
                }
                catch
                {
                    //TODO: Add Debug to log
                    TileTextures.Add(tile.EntityName, TileMissing);
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
                player.MovePlayer(-1 * TileSize, 0);
            if (oldState.IsKeyUp(Keys.Right) && newState.IsKeyDown(Keys.Right))
                player.MovePlayer(1 * TileSize, 0);
            if (oldState.IsKeyUp(Keys.Up) && newState.IsKeyDown(Keys.Up))
                player.MovePlayer(0, -1 * TileSize);
            if (oldState.IsKeyUp(Keys.Down) && newState.IsKeyDown(Keys.Down))
                player.MovePlayer(0, 1 * TileSize);


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

            spriteBatch.Begin();

            for (int i = 0; i < world.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < world.Grid.GetLength(1); j++)
                {
                    spriteBatch.Draw(TileTextures[world.Grid[i,j].EntityName], new Rectangle(i * TileSize, j * TileSize, TileSize, TileSize), Color.White);
                }
            }

            Texture2D PlayerTexture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            PlayerTexture.SetData<Color>(new Color[] { Color.Red });
            spriteBatch.Draw(PlayerTexture, new Rectangle(player.locationX, player.locationY, TileSize, TileSize), Color.Red);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

//Texture2D texture = new Texture2D(graphics.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
//#texture.SetData<Color>(new Color[] { world.Grid[i, j].TileColor });
//spriteBatch.Draw(texture, new Rectangle(i* TileSize, j* TileSize, TileSize, TileSize), world.Grid[i, j].TileColor);