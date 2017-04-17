using System;
using System.Collections.Generic;

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
        [NonSerialized]public List<Tile> TileSet;
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
}