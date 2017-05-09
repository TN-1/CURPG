// Based heavily on http://blog.two-cats.com/2014/06/a-star-example/

using System;
using System.Drawing;

namespace CURPG_Engine.AI.Pathfinding.AStar
{
    /// <summary>
    /// Represents a single node on a grid that is being searched for a path between two points
    /// </summary>
    public class Node
    {
        private Node _parentNode;

        /// <summary>
        /// The node's location in the grid
        /// </summary>
        public Point Location { get; }

        /// <summary>
        /// True when the node may be traversed, otherwise false
        /// </summary>
        public bool IsWalkable { get; }
        
        /// <summary>
        /// Cost from start to here
        /// </summary>
        public float G { get; private set; }

        /// <summary>
        /// Estimated cost from here to end
        /// </summary>
        private float H { get; }

        /// <summary>
        /// Flags whether the node is open, closed or untested by the PathFinder
        /// </summary>
        public NodeState State { get; set; }

        /// <summary>
        /// Estimated total cost (F = G + H)
        /// </summary>
        public float F => G + H;

        /// <summary>
        /// Gets or sets the parent node. The start node's parent is always null.
        /// </summary>
        public Node ParentNode
        {
            get => _parentNode;
            set
            {
                // When setting the parent, also calculate the traversal cost from the start node to here (the 'G' value)
                _parentNode = value;
                G = _parentNode.G + GetTraversalCost(Location, _parentNode.Location);
            }
        }

        /// <summary>
        /// Creates a new instance of Node.
        /// </summary>
        /// <param name="x">The node's location along the X axis</param>
        /// <param name="y">The node's location along the Y axis</param>
        /// <param name="isWalkable">True if the node can be traversed, false if the node is a wall</param>
        /// <param name="endLocation">The location of the destination node</param>
        public Node(int x, int y, bool isWalkable, Point endLocation)
        {
            Location = new Point(x, y);
            State = NodeState.Untested;
            IsWalkable = isWalkable;
            H = GetTraversalCost(Location, endLocation);
            G = 0;
        }

        public override string ToString()
        {
            return $"{Location.X}, {Location.Y}: {State}";
        }

        /// <summary>
        /// Gets the distance between two points
        /// </summary>
        internal static float GetTraversalCost(Point location, Point otherLocation)
        {
            try
            {
                float deltaX = otherLocation.X - location.X;
                float deltaY = otherLocation.Y - location.Y;
                return (float) Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
            
        }
    }
}
