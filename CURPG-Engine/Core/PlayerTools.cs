using System;

namespace CURPG_Engine.Core
{
    /// <summary>
    /// PlayerTools class. Includes the basic tools we need to interact with our player class.
    /// </summary>
    public static class PlayerTools
    {
        /// <summary>
        /// Generates a random player character
        /// </summary>
        /// <returns>Returns a randomised player class</returns>
        public static Player RandomPlayer(int x, int y)
        {
            var r = new Random();
            var maleNames = new[] { "Arron", "Anthony", "Bob", "Billy", "Charlie", "Scott", "Virgil", "Alan", "Gordon", "John" };
            var femaleNames = new[] { "Christine", "Jenny", "Tin-Tin", "Nicola", "Ashley", "Jennifer", "Abby", "Charlotte", "Addison", "Catherine" };
            string name;

            //First we assign a random gender....
            var n = r.Next(0, 1);
            var gender = n == 0 ? 'M' : 'F';

            //Now we need a name.
            if (gender == 'M')
            {
                n = r.Next(0, 9);
                name = maleNames[n];
            }
            else
            {
                n = r.Next(0, 9);
                name = femaleNames[n];
            }

            //Age
            n = r.Next(15, 80);
            var age = n;

            //Weight
            n = r.Next(30, 150);
            var weight = n;

            //Height
            n = r.Next(100, 250);
            var height = n;

            Player player = new Player(name, gender, age, height, weight, x, y);
            return player;
        }

        /// <summary>
        /// Finds a safe place to spawn our player
        /// </summary>
        /// <param name="world">Current world object</param>
        /// <param name="x">Initial spawn X</param>
        /// <param name="y">Initial spawn Y</param>
        /// <returns></returns>
        public static System.Drawing.Point GetSpawn(World world, int x, int y)
        {
            System.Drawing.Point pt;
            for (var i = x + 5; i >= (x - 5); i--)
            {
                if (world.Grid[i, y].TerrainModifier != 0) continue;
                pt = new System.Drawing.Point(i, y);
                return pt;
            }
            for (var i = y + 5; i >= (y - 5); i--)
            {
                if (world.Grid[x, i].TerrainModifier != 0) continue;
                pt = new System.Drawing.Point(x, i);
                return pt;
            }
            pt = new System.Drawing.Point(0, 0);
            return pt;
        }
    }
}
