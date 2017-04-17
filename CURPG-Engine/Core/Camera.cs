using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace CURPG_Engine.Core
{
    /// <summary>
    /// Camera allows for the drawing of a subset of the World class based on usable screen area.
    /// </summary>
    public class Camera
    {
        private int _x;
        private int _y;
        private int _maxX;
        private int _maxY;
        private System.Drawing.Rectangle _viewPort;
        private System.Drawing.Rectangle _miniViewPort;
        private readonly World _world;
        private readonly Player _player;
        private System.Drawing.Point _playerCoord;
        private List<System.Drawing.Point> _npcCoord;

        public System.Drawing.Point PlayerCoord => _playerCoord;
        public List<System.Drawing.Point> NpcCoord => _npcCoord;

        /// <summary>
        /// Constructs a camera object
        /// </summary>
        /// <param name="x">Set to 0</param>
        /// <param name="y">Set to 0</param>
        /// <param name="viewPort">Size of area to draw</param>
        /// <param name="miniViewPort">Size of minimap area</param>
        /// <param name="world">World object to draw</param>
        /// <param name="player">Player object to use</param>
        public Camera(int x, int y, System.Drawing.Rectangle viewPort, System.Drawing.Rectangle miniViewPort, World world, Player player)
        {
            _x = x;
            _y = y;
            _viewPort = viewPort;
            _miniViewPort = miniViewPort;
            _world = world;
            _player = player;
            _npcCoord = new List<System.Drawing.Point>();
        }

        /// <summary>
        /// Gets the current area to draw based on player location
        /// </summary>
        /// <returns>World object with subset to draw</returns>
        public World GetDrawArea()
        {
            var drawArea = new World(1, _viewPort.Width + 1, _viewPort.Height + 1, _world.TileSet, "DrawArea", _world.TileSize);
            var extremeBound = false;
            var xl = false;
            var yl = false;
            var xh = false;
            var yh = false;
            _x = _player.LocationX - (_viewPort.Width / 2);
            _y = _player.LocationY - (_viewPort.Height / 2);
            _maxX = _player.LocationX + (_viewPort.Width / 2);
            _maxY = _player.LocationY + (_viewPort.Height / 2);

            //Are we trying to draw outside the lower bounds of the map?
            if (_x <= 0)
            {
                _x = 0;
                _maxX = _viewPort.Width;
                extremeBound = true;
                xl = true;
            }
            if(_y <= 0)
            {
                _y = 0;
                //Y Is height dummy, Not width. CHECK YO VARIABLES FOOL!
                _maxY = _viewPort.Height;
                extremeBound = true;
                yl = true;
            } 
            //Are we trying to draw outside the upper bounds of the map?
            if(_x >= _world.Grid.GetLength(0) - _viewPort.Width)
            {
                _maxX = _world.Grid.GetLength(0) - 1;
                _x = (_world.Grid.GetLength(0) - 1)- _viewPort.Width;
                extremeBound = true;
                xh = true;
            }
            if (_y >= _world.Grid.GetLength(1) - _viewPort.Height)
            {
                _maxY = _world.Grid.GetLength(1) - 1;
                _y = (_world.Grid.GetLength(1) - 1) - _viewPort.Height;
                extremeBound = true;
                yh = true;
            }

            //Figure out where to draw the player relative to the viewport.
            if(!extremeBound)
            {
                _playerCoord.X = _viewPort.Width / 2;
                _playerCoord.Y = _viewPort.Height / 2;
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            else if(extremeBound)
            {
                if(xl)
                {
                    _playerCoord.X = _player.LocationX;
                    _playerCoord.Y = _viewPort.Height / 2;
                }
                if(yl)
                {
                    _playerCoord.X = _viewPort.Width / 2;
                    _playerCoord.Y = _player.LocationY;
                }
                if (xh)
                {
                    _playerCoord.X = _player.LocationX - (_world.Grid.GetLength(0) - _viewPort.Width);
                    _playerCoord.Y = _viewPort.Height / 2;
                }
                if (yh)
                {
                    _playerCoord.X = _viewPort.Width / 2;
                    _playerCoord.Y = _player.LocationY - (_world.Grid.GetLength(1) - _viewPort.Height);
                }
                if (xl && yl)
                {
                    //Top Left
                    _playerCoord.X = _player.LocationX;
                    _playerCoord.Y = _player.LocationY;
                }
                if (xl && yh)
                {
                    //Bottom Left
                    _playerCoord.X = _player.LocationX;
                    _playerCoord.Y = _player.LocationY - (_world.Grid.GetLength(1) - _viewPort.Height);
                }
                if (xh && yl)
                {
                    //Top Right
                    _playerCoord.X = _player.LocationX - (_world.Grid.GetLength(0) - _viewPort.Width);
                    _playerCoord.Y = _player.LocationY;
                }
                if (xh && yh)
                {
                    //Bottom Right
                    _playerCoord.X = _player.LocationX - (_world.Grid.GetLength(0) - _viewPort.Width);
                    _playerCoord.Y = _player.LocationY - (_world.Grid.GetLength(1) - _viewPort.Height);
                }

            }

            //Work out what to draw
            for (var x = _x; x <= _maxX; x++)
            {
                for(var y = _y; y <= _maxY; y++)
                {
                    drawArea.Grid[x - _x, y - _y] = _world.Grid[x, y];
                }
            }
            return drawArea;
        }

        public Color[,] GetMiniMap()
        {
            if (_miniViewPort.Width > _world.Grid.GetLength(0) || _miniViewPort.Height > _world.Grid.GetLength(1))
                throw new System.Exception("Well, Oops. Minimap is bigger than the map...");

            Color[,] color = new Color[_miniViewPort.Width + 1, _miniViewPort.Height + 1];
            var xM = _player.LocationX - (_miniViewPort.Width / 2);
            var yM = _player.LocationY - (_miniViewPort.Height / 2);
            var maXm = _player.LocationX + (_miniViewPort.Width / 2);
            var maxYm = _player.LocationY + (_miniViewPort.Height / 2);

            //Are we trying to draw outside the lower bounds of the map?
            if (xM <= 0)
            {
                xM = 0;
                maXm = _miniViewPort.Width;
            }
            if (yM <= 0)
            {
                yM = 0;
                //Y Is height dummy, Not width. CHECK YO VARIABLES FOOL!
                maxYm = _miniViewPort.Height;
            }
            //Are we trying to draw outside the upper bounds of the map?
            if (xM >= _world.Grid.GetLength(0) - _miniViewPort.Width)
            {
                maXm = _world.Grid.GetLength(0) - 1;
                xM = (_world.Grid.GetLength(0) - 1) - _miniViewPort.Width;
            }
            if (yM >= _world.Grid.GetLength(1) - _miniViewPort.Height)
            {
                maxYm = _world.Grid.GetLength(1) - 1;
                yM = (_world.Grid.GetLength(1) - 1) - _miniViewPort.Height;
            }

            for (var x = xM; x <= maXm; x++)
            {
                for (var y = yM; y <= maxYm; y++)
                {
                    color[x - xM, y - yM] = _world.Grid[x, y].TileColor;
                }
            }

            return color;
        }

        public void GetNpCs(List<Scriptables.Npc> npcs)
        {
            //BUG: NPC follows screen when scrolled to extremes
            List<Scriptables.Npc> actives = new List<Scriptables.Npc>();
            _npcCoord.Clear();

            foreach (Scriptables.Npc npc in npcs)
            {
                if (npc.LocationX <= _maxX && npc.LocationX >= _x && npc.LocationY <= _maxY && npc.LocationY >= _y)
                    actives.Add(npc);
            }

            foreach(Scriptables.Npc npc in actives)
            {
                _npcCoord.Add(new System.Drawing.Point(npc.LocationX, npc.LocationY));

            }
        }
    }
}