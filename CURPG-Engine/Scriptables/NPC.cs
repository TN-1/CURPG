using System;
using CURPG_Engine.Core;

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

        public Npc(int index, string name, char gender, int age, int height, int weight, int x, int y, int maxx, int maxy, World world) : base(name, gender, age, height, weight, x, y)
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
        }

        public void Update()
        {
            Random r = new Random();
            switch (r.Next(0, 3))
            {
                case 0:
                    if (LocationX + 1 <= _maxX)
                        MovePlayer(1, 0, _world);
                    break;
                case 1:
                    if (LocationY + 1 <= _maxY)
                        MovePlayer(0, 1, _world);
                    break;
                case 2:
                    if (LocationX - 1 >= _lowX)
                        MovePlayer(-1, 0, _world);
                    break;
                case 3:
                    if (LocationY - 1 >= _lowY)
                        MovePlayer(0, -1, _world);
                    break;
            }
        }
    }
}
