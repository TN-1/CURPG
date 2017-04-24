using System;
using System.Collections.Generic;
using CURPG_Engine.Core;
using System.Drawing;
using CURPG_Engine.AI.Pathfinding.AStar;
// ReSharper disable PrivateFieldCanBeConvertedToLocalVariable
// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace CURPG_Engine.Scriptables
{
    public class Npc : Player
    {
        public int Index;
        private int _maxX;
        private int _maxY;
        private int _lowX;
        private int _lowY;
        private World _world;
        private int _step;
        private List<Point> _path;
        private bool[,] _map;
        private PathFinder _pathFinder;
        private SearchParameters _searchParameters;
        private Point _startLocation;
        private Point _endLocation;

        public Npc(int index, string name, char gender, int age, int height, int weight, int x, int y, int maxx,
            int maxy, World world) : base(name, gender, age, height, weight, x, y)
        {
            Index = index;
            Name = name;
            Gender = gender;
            Age = age;
            Height = height;
            Weight = weight;
            LocationX = x;
            _lowX = x;
            LocationY = y;
            _lowY = y;
            _maxX = maxx;
            _maxY = maxy;
            _world = world;
            _map = new bool[_world.Grid.GetLength(0), _world.Grid.GetLength(1)];

            for (var i = 0; i < _world.Grid.GetLength(0); i++)
            {
                for (var j = 0; j < _world.Grid.GetLength(1); j++)
                {
                    if (_world.Grid[i, j].TerrainModifier == 0)
                        _map[i, j] = true;
                    else
                        _map[i, j] = false;
                }
            }

            _startLocation = new Point(_lowX, _lowY);
            _endLocation = new Point(_maxX, _maxY);
            _searchParameters = new SearchParameters(_startLocation, _endLocation, _map);
            _pathFinder = new PathFinder(_searchParameters);
            _path = _pathFinder.FindPath();
        }

        public void Update()
        {
            if (_step == _path.Count)
            {
                var r = new Random();
                _step = 0;
                _startLocation = new Point(LocationX, LocationY);
                _endLocation = PlayerTools.GetSpawn(_world, r.Next(0,20), r.Next(0,20));
                _searchParameters = new SearchParameters(_startLocation, _endLocation, _map);
                _pathFinder = new PathFinder(_searchParameters);
                _path = _pathFinder.FindPath();
            }
            LocationX = _path[_step].X;
            LocationY = _path[_step].Y;
            _step++;
        }
    }
}