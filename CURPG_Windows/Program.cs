using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using CURPG_Engine.Core;

namespace CURPG_Windows
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            //Make sure required data files are in place
            var loc = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            if(loc == null) throw new Exception("Loc is null");
            var fileName = Path.Combine(loc, @"DataFiles\Tiles.xml");
            if (!File.Exists(fileName))
            {
                MessageBox.Show("DataFile is missing: \"Tiles.XML\" Progam terminating...");
                return;
            }
            fileName = Path.Combine(loc, @"DataFiles\Items.xml");
            if (!File.Exists(fileName))
            {
                MessageBox.Show("DataFile is missing: \"Items.XML\" Progam terminating...");
                return;
            }
            //Well thats a good sign, isnt it? Now we can enter the game
            using (ScreenManager manager = new ScreenManager())
            {
                Logger.Info("Starting...", "CURPG_Windows");
                manager.Run();
            }
        }
    }
#endif
}