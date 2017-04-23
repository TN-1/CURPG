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
        private System.Drawing.Point _miniPlayerCoord;
        private List<System.Drawing.Point> _npcCoord;
        private List<System.Drawing.Point> _miniNpcCoord;
        public System.Drawing.Point PlayerCoord => _playerCoord;
        public System.Drawing.Point MiniPlayerCoord => _miniPlayerCoord;
        public List<System.Drawing.Point> NpcCoord => _npcCoord;
        public List<System.Drawing.Point> MiniNPCCoord => _miniNpcCoord;

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
            _miniNpcCoord = new List<System.Drawing.Point>();
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

        public Color[,] GetMiniMap(int zoom, List<Scriptables.Npc> npcs)
        {
            if (_miniViewPort.Width > _world.Grid.GetLength(0) || _miniViewPort.Height > _world.Grid.GetLength(1))
                throw new System.Exception("Well, Oops. Minimap is bigger than the map...");

            Color[,] color = new Color[_miniViewPort.Width + 1, _miniViewPort.Height + 1];
            var xM = _player.LocationX - (_miniViewPort.Width / zoom);
            var yM = _player.LocationY - (_miniViewPort.Height / zoom);
            var maXm = _player.LocationX + (_miniViewPort.Width / zoom);
            var maxYm = _player.LocationY + (_miniViewPort.Height / zoom);
            var extremeBound = false;
            var xl = false;
            var yl = false;
            var xh = false;
            var yh = false;

            //Are we trying to draw outside the lower bounds of the map?
            if (xM <= 0)
            {
                xM = 0;
                maXm = _miniViewPort.Width;
                extremeBound = true;
                xl = true;
            }
            if (yM <= 0)
            {
                yM = 0;
                //Y Is height dummy, Not width. CHECK YO VARIABLES FOOL!
                maxYm = _miniViewPort.Height;
                extremeBound = true;
                yl = true;
            }
            //Are we trying to draw outside the upper bounds of the map?
            if (xM >= _world.Grid.GetLength(0) - _miniViewPort.Width)
            {
                maXm = _world.Grid.GetLength(0) - 1;
                xM = (_world.Grid.GetLength(0) - 1) - _miniViewPort.Width;
                extremeBound = true;
                xh = true;
            }
            if (yM >= _world.Grid.GetLength(1) - _miniViewPort.Height)
            {
                maxYm = _world.Grid.GetLength(1) - 1;
                yM = (_world.Grid.GetLength(1) - 1) - _miniViewPort.Height;
                extremeBound = true;
                yh = true;
            }

            //Figure out where to draw the player relative to the viewport.
            if (!extremeBound)
            {
                _miniPlayerCoord.X = _miniViewPort.Width / 2;
                _miniPlayerCoord.Y = _miniViewPort.Height / 2;
            }
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            else if (extremeBound)
            {
                if (xl)
                {
                    _miniPlayerCoord.X = _player.LocationX;
                    _miniPlayerCoord.Y = _viewPort.Height / 2;
                }
                if (yl)
                {
                    _miniPlayerCoord.X = _viewPort.Width / 2;
                    _miniPlayerCoord.Y = _player.LocationY;
                }
                if (xh)
                {
                    _miniPlayerCoord.X = _player.LocationX - (_world.Grid.GetLength(0) - _viewPort.Width);
                    _miniPlayerCoord.Y = _miniViewPort.Height / 2;
                }
                if (yh)
                {
                    _miniPlayerCoord.X = _miniViewPort.Width / 2;
                    _miniPlayerCoord.Y = _player.LocationY - (_world.Grid.GetLength(1) - _viewPort.Height);
                }
                if (xl && yl)
                {
                    //Top Left
                    _miniPlayerCoord.X = _player.LocationX;
                    _miniPlayerCoord.Y = _player.LocationY;
                }
                if (xl && yh)
                {
                    //Bottom Left
                    _miniPlayerCoord.X = _player.LocationX;
                    _miniPlayerCoord.Y = _player.LocationY - (_world.Grid.GetLength(1) - _miniViewPort.Height);
                }
                if (xh && yl)
                {
                    //Top Right
                    _miniPlayerCoord.X = _player.LocationX - (_world.Grid.GetLength(0) - _miniViewPort.Width);
                    _miniPlayerCoord.Y = _player.LocationY;
                }
                if (xh && yh)
                {
                    //Bottom Right
                    _miniPlayerCoord.X = _player.LocationX - (_world.Grid.GetLength(0) - _miniViewPort.Width);
                    _miniPlayerCoord.Y = _player.LocationY - (_world.Grid.GetLength(1) - _miniViewPort.Height);
                }
            }

            xM = xM / zoom;
            maXm = maXm / zoom;
            yM = yM / zoom;
            maxYm = maxYm / zoom;

            for (var x = xM; x <= maXm; x++)
            {
                for (var y = yM; y <= maxYm; y++)
                {
                    color[x - xM, y - yM] = _world.Grid[x, y].TileColor;
                }
            }

            var actives = new List<Scriptables.Npc>();
            _miniNpcCoord.Clear();

            foreach (var npc in npcs)
            {
                if (npc.LocationX <= maXm && npc.LocationX >= xM && npc.LocationY <= maxYm && npc.LocationY >= yM)
                    actives.Add(npc);
            }

            foreach (var npc in actives)
            {
                _miniNpcCoord.Add(new System.Drawing.Point(npc.LocationX, npc.LocationY));

            }

            return color;
        }

        public void GetNpCs(List<Scriptables.Npc> npcs)
        {
            //BUG: NPC follows screen when scrolled to extremes
            var actives = new List<Scriptables.Npc>();
            _npcCoord.Clear();

            foreach (var npc in npcs)
            {
                if (npc.LocationX <= _maxX && npc.LocationX >= _x && npc.LocationY <= _maxY && npc.LocationY >= _y)
                    actives.Add(npc);
            }

            foreach(var npc in actives)
            {
                _npcCoord.Add(new System.Drawing.Point(npc.LocationX, npc.LocationY));

            }
        }
    }
}