using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Microsoft.Xna.Framework;

namespace CURPG_Engine.Core
{
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
        public static World GenerateWorld(int worldIndex, int gridX, int gridY, List<Tile> tileSet, string name, int tilesize, string tilePath)
        {
            Logger.Info("Start World Gen", "CURPG_Engine");
            float[,] noiseValues;
            var a = 0;
            var world = new World(worldIndex, gridX, gridY, tileSet, name, tilesize);

            var tiles = new XmlDocument();
            tiles.Load(tilePath);
            Debug.Assert(tiles.DocumentElement != null, "tiles.DocumentElement != null");
            var nodes = tiles.DocumentElement.SelectNodes("/tiles/tile");

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
                        world.Grid[i, j] = NewTile(nodes, 20);
                    if (noiseValues[i, j] > 25 && noiseValues[i, j] <= 65)
                        world.Grid[i, j] = NewTile(nodes, 19);
                    if (noiseValues[i, j] > 65 && noiseValues[i, j] <= 100)
                        world.Grid[i, j] = NewTile(nodes, 17);
                    if (noiseValues[i, j] > 100 && noiseValues[i, j] <= 150)
                        world.Grid[i, j] = NewTile(nodes, 24);
                    if (noiseValues[i, j] > 150 && noiseValues[i, j] <= 200)
                        world.Grid[i, j] = NewTile(nodes, 25);
                    if (noiseValues[i, j] > 200)
                        world.Grid[i, j] = NewTile(nodes, 21);
                    world.Grid[i, j].NoiseVal = noiseValues[i, j];
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

        private static Tile NewTile(XmlNodeList nodes, int tileId)
        {
            Debug.Assert(nodes != null, "nodes != null");
            foreach (XmlNode node in nodes)
            {
                var idNode = node.SelectSingleNode("id");
                if (idNode == null) continue;
                var index = Convert.ToInt32(idNode.InnerText);
                if (index != tileId) continue;
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
                return tile;
            }
            return null;
        }


        /// <summary>
        /// Comverts a string to a valid color
        /// </summary>
        /// <param name="nameOfColor">String of color</param>
        /// <returns>A usable color</returns>
        public static Color ValidColor(string nameOfColor)
        {
            var prop = typeof(Color).GetProperty(nameOfColor);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return Color.HotPink;

        }
    }
}