using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CURPG_Engine.Core
{
    public class Persistence
    {
        static public void SaveGame(Player player, World world)
        {
            var destination = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"\Games\CURPG");
            //First, We save the player....
            using (var sw = new StreamWriter(Path.Combine(destination, "player.sav")))
            {
                sw.WriteLine(player.name);
                sw.WriteLine(player.height);
                sw.WriteLine(player.weight);
                sw.WriteLine(player.gender);
                sw.WriteLine(player.age);
                sw.WriteLine(player.health);
                sw.WriteLine((int)player.locationX);
                sw.WriteLine((int)player.locationY);
                sw.Flush();
                sw.Close();
            }
            //Now we save the world...
            using (var sw = new StreamWriter(Path.Combine(destination, "world.sav")))
            {

            }

        }

        public void LoadGame()
        {

        }
    }
}
