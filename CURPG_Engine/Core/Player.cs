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
        protected string Name;
        public int LocationX;
        public int LocationY;
        protected char Gender;
        protected int Age;
        protected int Weight;
        protected int Height;
        public int Health = 100;
        public readonly Inventory.Inventory Inventory;
        public bool Testing;
        protected bool IsLocked;

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
        public bool MovePlayer(int x, int y, World world)
        {
            if (IsLocked) return false;
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
                                                return true;
                                        }
                                    }
                                }
                                Craftable logs = (Craftable)Inventory.ItemDb[1];
                                logs.StackHeight = i;
                                Inventory.AddItem(logs);
                            }
                            break;
                        case 2:
                            //Mountains
                            return true;
                        case 3:
                            //Water
                            return true;
                        case 4:
                            //Buildings
                            return true;
                    }
                }
            }
            return true;
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

        public void SetLock(bool value) => IsLocked = value;
    }
}