using System.Drawing;

namespace CURPG_Engine.AI.Pathfinding.AStar
{
    /// <summary>
    /// Defines the parameters which will be used to find a path across a section of the map
    /// </summary>
    public class SearchParameters
    {
        public Point StartLocation { get; }

        public Point EndLocation { get; }
        
        public bool[,] Map { get; }

        public SearchParameters(Point startLocation, Point endLocation, bool[,] map)
        {
            StartLocation = startLocation;
            EndLocation = endLocation;
            Map = map;
        }
    }
}
