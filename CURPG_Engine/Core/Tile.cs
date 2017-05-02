using System;
using System.Drawing;
using Color = Microsoft.Xna.Framework.Color;

namespace CURPG_Engine.Core
{
    /// <summary>
    /// These are our tiles that build our world
    /// </summary>
    [Serializable]
    public class Tile
    {
        private int _index;
        public readonly string EntityName;
        private string _tileColor;
        private string _tileName;
        public readonly int TerrainModifier;
        public float NoiseVal;

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
            TileColor = color;
            _tileName = tilename;
            TerrainModifier = terrainmod;
        }

        public Color TileColor
        {
            get => WorldTools.ValidColor(_tileColor);
            private set => _tileColor = GetColorName(value);
        }

        private static string GetColorName(Color color)
        {
            var c = System.Drawing.Color.FromArgb(color.R, color.G, color.B);
            foreach (KnownColor kc in Enum.GetValues(typeof(KnownColor)))
            {
                var known = System.Drawing.Color.FromKnownColor(kc);
                if (c.ToArgb() == known.ToArgb())
                {
                    return known.Name;
                }
            }
            return "";
        }
    }
}