using System.Collections.Generic;

namespace CURPG_Engine.Core
{
    /// <summary>
    /// Camera allows for the drawing of a subset of the World class based on usbale screen area. TODO: Support minimap
    /// </summary>
    public class Camera
    {
        private int _x;
        private int _y;
        private int _maxX;
        private int _maxY;
        private System.Drawing.Rectangle _viewPort;
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
        /// <param name="world">World object to draw</param>
        /// <param name="player">Player object to use</param>
        public Camera(int x, int y, System.Drawing.Rectangle viewPort, World world, Player player)
        {
            _x = x;
            _y = y;
            _viewPort = viewPort;
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
            World drawArea = new World(1, _viewPort.Width + 1, _viewPort.Height + 1, _world.TileSet, "DrawArea", _world.TileSize);
            bool extremeBound = false;
            bool xl = false;
            bool yl = false;
            bool xh = false;
            bool yh = false;
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

        public void GetNpCs(List<Scriptables.Npc> npcs)
        {
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