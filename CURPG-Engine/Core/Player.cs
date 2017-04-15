using CURPG_Engine.Inventory;
using System;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Global

namespace CURPG_Engine.Core
{
    /// <summary>
    /// Our player class. Includes all the properties we need to create a player character
    /// </summary>
    [Serializable]
    public class Player
    {
        //Values our player needs
        public string Name;
        public int LocationX;
        public int LocationY;
        public char Gender;
        public int Age;
        public int Weight;
        public int Height;
        public int Health = 100;
        public readonly Inventory.Inventory Inventory;
        public bool Testing;

        /// <summary>
        /// Constructs a player from user assigned values
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gender"></param>
        /// <param name="age"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Player(string name, char gender, int age, int height, int weight, int x, int y)
        {
            Name = name;
            Gender = gender;
            Age = age;
            Height = height;
            Weight = weight;
            LocationX = x;
            LocationY = y;
            Inventory = new Inventory.Inventory(64);
            Testing = false;
        }

        /// <summary>
        /// Moves the player
        /// </summary>
        /// <param name="x">New X Coord</param>
        /// <param name="y">New Y Coord</param>
        /// <param name="world">Active world object</param>
        public void MovePlayer(int x, int y, World world)
        {
            var r = new Random();
            var curX = LocationX;
            var curY = LocationY;
            var newX = curX + x;
            var newY = curY + y;

            if (newX < 0 || newY < 0 || newX > world.Grid.GetLength(0) - 1 || newY > world.Grid.GetLength(1) - 1)
            {
            }
            else
            {
                if (Testing)
                {
                    LocationX = newX;
                    LocationY = newY;
                }
                else
                {
                    switch (world.Grid[newX, newY].TerrainModifier)
                    {
                        case 0:
                            //Flat ground
                            LocationX = newX;
                            LocationY = newY;
                            break;
                        case 1:
                            if (Inventory.Items[0] is Tool tool && tool.TerrainMod == 1)
                            {
                                var i = r.Next(1, 5);
                                LocationX = newX;
                                LocationY = newY;
                                world.ChangeTile(LocationX, LocationY, 24);
                                foreach (var check in Inventory.Items)
                                {
                                    if (check is Craftable log && log.Id == 1)
                                    {
                                        if (log.HowManyMore() >= i)
                                        {
                                            if(log.AddQuantity(i))
                                                return;
                                        }
                                    }
                                }
                                Craftable logs = (Craftable)Inventory.ItemDb[1];
                                logs.StackHeight = i;
                                Inventory.AddItem(logs);
                            }
                            return;
                        case 2:
                            //Mountains
                            return;
                        case 3:
                            //Water
                            return;
                        case 4:
                            //Buildings
                            return;
                    }
                }
            }
        }

        /// <summary>
        /// Teleports the player to a location
        /// </summary>
        /// <param name="x">Destination X Coord</param>
        /// <param name="y">Desintation Y Coord</param>
        public void Teleport(int x, int y)
        {
            LocationX = x;
            LocationY = y;
        }

        /// <summary>
        /// Getd a nice string with the players current location
        /// </summary>
        /// <returns>x,y</returns>
        public string Location()
        {
            var s = LocationX + "," + LocationY;
            return s;
        }
    }

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