using System.Drawing;

namespace CURPG_Engine.AI.Pathfinding.AStar
{
    /// <summary>
    /// Defines the parameters which will be used to find a path across a section of the map
    /// </summary>
    public class SearchParameters
    {
        public Point StartLocation { get; set; }

        public Point EndLocation { get; set; }
        
        public bool[,] Map { get; set; }

        public SearchParameters(Point startLocation, Point endLocation, bool[,] map)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
            Map = map;
        }
    }
}
