using CURPG_Engine.Inventory;
using System;

namespace CURPG_Engine.Core
{
    /// <summary>
    /// Our player class. Includes all the properties we need to create a player character
    /// </summary>
    [Serializable]
    public class Player
    {
        //Values our player needs
        public string name;
        public int locationX;
        public int locationY;
        public char gender;
        public int age;
        public int weight;
        public int height;
        public int health = 100;
        public Inventory.Inventory Inventory;
        public bool Testing;

        /// <summary>
        /// Constructs a player from user assigned values
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gender"></param>
        /// <param name="age"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public Player(string Name, char Gender, int Age, int Height, int Weight, int x, int y)
        {
            name = Name;
            gender = Gender;
            age = Age;
            height = Height;
            weight = Weight;
            locationX = x;
            locationY = y;
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
            Random r = new Random();
            var CurX = locationX;
            var CurY = locationY;
            var NewX = CurX + x;
            var NewY = CurY + y;

            if (NewX < 0 || NewY < 0 || NewX > world.Grid.GetLength(0) - 1 || NewY > world.Grid.GetLength(1) - 1)
            {
                return;
            }
            else
            {
                if (Testing == true)
                {
                    locationX = NewX;
                    locationY = NewY;
                }
                else
                {
                    switch (world.Grid[NewX, NewY].TerrainModifier)
                    {
                        case 0:
                            //Flat ground
                            locationX = NewX;
                            locationY = NewY;
                            break;
                        case 1:
                            foreach (Item item in Inventory.Items)
                            {
                                if (item is Tool)
                                {
                                    Tool tool = (Tool)item;
                                    if (tool.TerrainMod == 1)
                                    {
                                        locationX = NewX;
                                        locationY = NewY;
                                        world.ChangeTile(locationX, locationY, 24);
                                        Craftable logs = null;
                                        foreach(Item _item in Inventory.ItemDB)
                                            if (item.ID == 1)
                                                logs = (Craftable)_item;
                                        logs.StackHeight = r.Next(1, 5);
                                        if (Inventory.Contains(1))
                                        {
                                            foreach (Item log in Inventory.Items)
                                                if (log.ID == 1)
                                                {
                                                    Craftable exists = (Craftable)log;
                                                    exists.AddQuantity(logs.StackHeight);
                                                }
                                        }
                                        else
                                            Inventory.AddItem(logs);
                                    }
                                }
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
            locationX = x;
            locationY = y;
            return;
        }

        /// <summary>
        /// Getd a nice string with the players current location
        /// </summary>
        /// <returns>x,y</returns>
        public string Location()
        {
            var s = locationX + "," + locationY;
            return s;
        }
    }

    /// <summary>
    /// PlayerTools class. Includes the basic tools we need to interact with our player class.
    /// </summary>
    public class PlayerTools
    {
        /// <summary>
        /// Generates a random player character
        /// </summary>
        /// <returns>Returns a randomised player class</returns>
        static public Player RandomPlayer(int x, int y)
        {
            Random r = new Random();
            string[] MaleNames = new string[10] { "Arron", "Anthony", "Bob", "Billy", "Charlie", "Scott", "Virgil", "Alan", "Gordon", "John" };
            string[] FemaleNames = new string[10] { "Christine", "Jenny", "Tin-Tin", "Nicola", "Ashley", "Jennifer", "Abby", "Charlotte", "Addison", "Catherine" };
            char gender;
            string name;
            int height;
            int weight;
            int age;

            //First we assign a random gender....
            var n = r.Next(0, 1);
            if (n == 0)
                gender = 'M';
            else
                gender = 'F';

            //Now we need a name.
            if (gender == 'M')
            {
                n = r.Next(0, 9);
                name = MaleNames[n];
            }
            else
            {
                n = r.Next(0, 9);
                name = FemaleNames[n];
            }

            //Age
            n = r.Next(15, 80);
            age = n;

            //Weight
            n = r.Next(30, 150);
            weight = n;

            //Height
            n = r.Next(100, 250);
            height = n;

            Player player = new Player(name, gender, age, height, weight, x, y);
            return player;
        }

        /// <summary>
        /// Finds a safe place to spawn our player
        /// </summary>
        /// <param name="world">Current world object</param>
        /// <param name="X">Initial spawn X</param>
        /// <param name="Y">Initial spawn Y</param>
        /// <returns></returns>
        static public System.Drawing.Point GetSpawn(World world, int X, int Y)
        {
            System.Drawing.Point pt;
            for (int i = X + 5; i >= (X - 5); i--)
            {
                if (world.Grid[i, Y].TerrainModifier == 0)
                {
                    pt = new System.Drawing.Point(i, Y);
                    return pt;
                }
            }
            for (int i = Y + 5; i >= (Y - 5); i--)
            {
                if (world.Grid[X, i].TerrainModifier == 0)
                {
                    pt = new System.Drawing.Point(X, i);
                    return pt;
                }
            }
            pt = new System.Drawing.Point(0, 0);
            return pt;
        }
    }
}