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
            float[,] noiseValues;
            World world = new World(WorldIndex, GridX, GridY, TileSet, Name);

            //Generate noise map
            start:
            try
            {
                Random r = new Random();
                Simplex.Noise.Seed = r.Next(0, 999999999);
                noiseValues = Simplex.Noise.Calc2D(GridX * 100, GridY * 100, 0.065f);
                var Point = r.Next(0, GridX * 80);
                float[,] noise = new float[GridX, GridY];

                for (int i = Point; i < (GridX + Point); i++)
                {
                    for (int j = Point; j < (GridY + Point); j++)
                    {
                        noise[(i - Point), (j - Point)] = noiseValues[i, j];
                    }
                }
            }
            catch
            {
                //Generation failed, Oops. Add debug and try again
                goto start;
            }

            //Process noise map into game map
            for (int i = 0; i < world.Grid.GetLength(0); i++)
            {
                for (int j = 0; j < world.Grid.GetLength(1); j++)
                {
                    if (noiseValues[i, j] <= 25)
                        world.Grid[i, j] = new Tile(20, "WaterDeep", ValidColor("Blue"), "Deep Water");
                    if (noiseValues[i, j] > 25 && noiseValues[i, j] <= 65)
                        world.Grid[i, j] = new Tile(19, "WaterShallow", ValidColor("DarkTurquiose"), "Shallow Water");
                    if (noiseValues[i, j] > 65 && noiseValues[i, j] <= 100)
                        world.Grid[i, j] = new Tile(17, "SandYellow", ValidColor("Yellow"), "Yellow Sand");
                    if (noiseValues[i, j] > 100 && noiseValues[i, j] <= 150)
                        world.Grid[i, j] = new Tile(24, "Grass", ValidColor("Green"), "Grass");
                    if (noiseValues[i, j] > 150 && noiseValues[i, j] <= 200)
                        world.Grid[i, j] = new Tile(25, "Trees", ValidColor("Green"), "Trees");
                    if (noiseValues[i, j] > 200)
                        world.Grid[i, j] = new Tile(21, "Mountain1", ValidColor("Grey"), "Mountain1");
                }
            }
            return world;
        }

        static public List<Tile> TileSetBuilder(string tilepath)
        {
            XmlDocument tiles = new XmlDocument();
            tiles.Load(tilepath);
            List<Tile> tileset = new List<Tile>();
            XmlNodeList nodes = tiles.DocumentElement.SelectNodes("/tiles/tile");

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


//        for (int i = 0; i<world.Grid.GetLength(0); i++)
//            {
//                for(int j = 0; j<world.Grid.GetLength(1); j++)
//                {
//                    var k = r.Next(0, world.TileSet.Count);
//Tile tile = new Tile(world.TileSet[k].Index, world.TileSet[k].EntityName, world.TileSet[k].TileColor, world.TileSet[k].TileName);
//world.Grid[i, j] = tile;
//                }
//            }
