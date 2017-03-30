using System;
using CURPG_Engine.Core;
namespace CURPG_Engine.Core
{
    /// <summary>
    /// Our player class. Includes all the properties we need to create a player character
    /// </summary>
    public class Player
    {
        //Values our player needs
        public string name;
        public int locationX = 0;
        public int locationY = 0;
        public char gender;
        public int age;
        public int weight;
        public int height;
        public int health = 100;

        /// <summary>
        /// Constructs a player from user assigned values
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gender"></param>
        /// <param name="age"></param>
        /// <param name="height"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        public Player(string Name, char Gender, int Age, int Height, int Weight)
        {
            name = Name;
            gender = Gender;
            age = Age;
            height = Height;
            weight = Weight;
        }

        public void MovePlayer(int x, int y)
        {
            var CurX = locationX;
            var CurY = locationY;
            var NewX = CurX + x;
            var NewY = CurY + y;
            locationX = NewX;
            locationY = NewY;
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
        static public Player RandomPlayer()
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

            Player player = new Player(name, gender, age, height, weight);
            return player;
        }
    }
}
