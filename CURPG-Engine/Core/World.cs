using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Xml;

namespace CURPG_Engine.Core
{
    public class World
    {
        public int Index;
        public Tile[,] Grid;
        public List<Tile> TileSet;
        public string Name;
        public int Day = 0;

        public World(int WorldIndex, int GridX, int GridY, List<Tile> Set, string name)
        {
            Index = WorldIndex;
            Grid = new Tile[GridX, GridY];
            TileSet = Set;
            Name = name;
        }
    }

    public class Tile
    {
        public int Index;
        public string EntityName;
        public Color TileColor;
        public string TileName;

        public Tile(int index, string entname, Color color, string tilename)
        {
            Index = index;
            EntityName = entname;
            TileColor = color;
            TileName = tilename;
        }
    }


    public class WorldTools
    {
        static public World GenerateWorld(int WorldIndex, int GridX, int GridY, List<Tile> TileSet, string Name)
        {
            World world = new World(WorldIndex, GridX, GridY, TileSet, Name);
            
            Random r = new Random();

            for (int i = 0; i < world.Grid.GetLength(0); i++)
            {
                for(int j = 0; j < world.Grid.GetLength(1); j++)
                {
                    var k = r.Next(0, world.TileSet.Count);
                    Tile tile = new Tile(world.TileSet[k].Index, world.TileSet[k].EntityName, world.TileSet[k].TileColor, world.TileSet[k].TileName);
                    world.Grid[i, j] = tile;
                }
            }

            return world;
        }

        static public List<Tile> TileSetBuilder(XmlDocument xml)
        {
            List<Tile> tileset = new List<Tile>();
            XmlNodeList nodes = xml.DocumentElement.SelectNodes("/tiles/tile");

            foreach(XmlNode node in nodes)
            {
                var index = Convert.ToInt32(node.SelectSingleNode("id").InnerText);
                var entityName = node.SelectSingleNode("entityName").InnerText;
                var tileColor = node.SelectSingleNode("tileColor").InnerText;
                var tileName = node.SelectSingleNode("tileName").InnerText;

                Tile tile = new Tile(index, entityName, ValidColor(tileColor), tileName);
                tileset.Add(tile);
            }
            return tileset;
        }

        static Color ValidColor(string nameOfColor)
        {
            var prop = typeof(Color).GetProperty(nameOfColor);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return Color.HotPink;

        }
    }
}