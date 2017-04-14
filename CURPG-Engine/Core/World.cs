using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Xml;
// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedMember.Global

namespace CURPG_Engine.Core
{
    /// <summary>
    /// This is our world class!
    /// </summary>
    [Serializable]
    public class World
    {
        public readonly int Index;
        public readonly Tile[,] Grid;
        public readonly List<Tile> TileSet;
        // ReSharper disable once NotAccessedField.Global
        private string _name;
        public readonly int TileSize;
        
        /// <summary>
        /// Builds our world
        /// </summary>
        /// <param name="worldIndex">Index for our world</param>
        /// <param name="gridX">Total world size X</param>
        /// <param name="gridY">Total world size Y</param>
        /// <param name="set">Tileset for our world</param>
        /// <param name="name">World name</param>
        /// <param name="tilesize">World tilesize</param>
        public World(int worldIndex, int gridX, int gridY, List<Tile> set, string name, int tilesize)
        {
            Index = worldIndex;
            Grid = new Tile[gridX, gridY];
            TileSet = set;
            _name = name;
            TileSize = tilesize;
        }

        /// <summary>
        /// Changes our tile
        /// </summary>
        /// <param name="s">Coords to change, Expects String(x,y)</param>
        /// <param name="index">Index of new tile</param>
        public void ChangeTile(string s, int index)
        {
            var t = s.Split(',');
            var x = Convert.ToInt32(t[0]);
            var y = Convert.ToInt32(t[1]);
            Grid[x, y] = TileSet[index];
        }

        /// <summary>
        /// Changes our tile
        /// </summary>
        /// <param name="x">X coord of tile to change</param>
        /// <param name="y">Y coord of tile to change</param>
        /// <param name="index">Index of new tile</param>
        public void ChangeTile(int x, int y, int index)
        {
            Grid[x, y] = TileSet[index];
        }
    }

    /// <summary>
    /// These are our tiles that build our world
    /// </summary>
    [Serializable]
    public class Tile
    {
        private int _index;
        public readonly string EntityName;
        [NonSerialized] private Color _tileColor;
        private string _tileName;
        public readonly int TerrainModifier;

        /// <summary>
        /// Builds our tile objects
        /// </summary>
        /// <param name="index">Tile index</param>
        /// <param name="entname">Name of tile graphic</param>
        /// <param name="color">Basic tile color for minimap</param>
        /// <param name="tilename">Human reabable name of tile</param>
        /// <param name="terrainmod">Terrain modifyer for player movement and combat</param>
        public Tile(int index, string entname, Color color, string tilename, int terrainmod)
        {
            _index = index;
            EntityName = entname;
            _tileColor = color;
            _tileName = tilename;
            TerrainModifier = terrainmod;
        }
    }

    /// <summary>
    /// Just some misc tools to use with our world
    /// </summary>
    public static class WorldTools
    {
        /// <summary>
        /// Generates a random world
        /// </summary>
        /// <param name="worldIndex">World index</param>
        /// <param name="gridX">Map size X</param>
        /// <param name="gridY">Map size Y</param>
        /// <param name="tileSet">Tileset for our map</param>
        /// <param name="name">World name</param>
        /// <param name="tilesize">Tilesize</param>
        /// <returns></returns>
        public static World GenerateWorld(int worldIndex, int gridX, int gridY, List<Tile> tileSet, string name, int tilesize)
        {
            float[,] noiseValues;
            var a = 0;
            var world = new World(worldIndex, gridX, gridY, tileSet, name, tilesize);

            //Generate noise map
            start:
            try
            {
                var r = new Random();
                Simplex.Noise.Seed = r.Next(0, 999999999);
                noiseValues = Simplex.Noise.Calc2D(gridX * 10, gridY * 10, 0.065f);
                var point = r.Next(0, gridX * 8);
                var noise = new float[gridX, gridY];

                for (var i = point; i < (gridX + point); i++)
                {
                    for (var j = point; j < (gridY + point); j++)
                    {
                        noise[(i - point), (j - point)] = noiseValues[i, j];
                    }
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine("World Generation failed. Attempt #" + a);
                Debug.WriteLine("Exception: " + e);
                a++;
                goto start;
            }

            //Process noise map into game map
            for (var i = 0; i < world.Grid.GetLength(0); i++)
            {
                for (var j = 0; j < world.Grid.GetLength(1); j++)
                {
                    if (noiseValues[i, j] <= 25)
                        world.Grid[i, j] = world.TileSet[20];
                    if (noiseValues[i, j] > 25 && noiseValues[i, j] <= 65)
                        world.Grid[i, j] = world.TileSet[19];
                    if (noiseValues[i, j] > 65 && noiseValues[i, j] <= 100)
                        world.Grid[i, j] = world.TileSet[17];
                    if (noiseValues[i, j] > 100 && noiseValues[i, j] <= 150)
                        world.Grid[i, j] = world.TileSet[24];
                    if (noiseValues[i, j] > 150 && noiseValues[i, j] <= 200)
                        world.Grid[i, j] = world.TileSet[25];
                    if (noiseValues[i, j] > 200)
                        world.Grid[i, j] = world.TileSet[21];
                }
            }
            return world;
        }

        /// <summary>
        /// Build our tileset from an XML definition
        /// </summary>
        /// <param name="tilepath">Path to XML</param>
        /// <returns>A list of tiles</returns>
        public static List<Tile> TileSetBuilder(string tilepath)
        {
            var tiles = new XmlDocument();
            tiles.Load(tilepath);
            var tileset = new List<Tile>();
            Debug.Assert(tiles.DocumentElement != null, "tiles.DocumentElement != null");
            var nodes = tiles.DocumentElement.SelectNodes("/tiles/tile");

            Debug.Assert(nodes != null, "nodes != null");
            foreach(XmlNode node in nodes)
            {
                var idNode = node.SelectSingleNode("id");
                if (idNode == null) continue;
                var index = Convert.ToInt32(idNode.InnerText);
                var entityNameNode = node.SelectSingleNode("entityName");
                if (entityNameNode == null) continue;
                var entityName = entityNameNode.InnerText;
                var tileColorNode = node.SelectSingleNode("tileColor");
                if (tileColorNode == null) continue;
                var tileColor = tileColorNode.InnerText;
                var tileNameNode = node.SelectSingleNode("tileName");
                if (tileNameNode == null) continue;
                var tileName = tileNameNode.InnerText;
                var terrModNode = node.SelectSingleNode("terrainMod");
                if (terrModNode == null) continue;
                var terrmod = Convert.ToInt32(terrModNode.InnerText);

                var tile = new Tile(index, entityName, ValidColor(tileColor), tileName, terrmod);
                tileset.Add(tile);
            }
            return tileset;
        }

        /// <summary>
        /// Comverts a string to a valid color
        /// </summary>
        /// <param name="nameOfColor">String of color</param>
        /// <returns>A usable color</returns>
        private static Color ValidColor(string nameOfColor)
        {
            var prop = typeof(Color).GetProperty(nameOfColor);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return Color.HotPink;

        }
    }
}