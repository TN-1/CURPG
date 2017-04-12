using CURPG_Graphical_MonoGame_Windows;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace CURPG_Graphical
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //Make sure required data files are in place
            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"DataFiles\Tiles.xml");
            if (!File.Exists(fileName))
            {
                MessageBox.Show("DataFile is missing: \"Tiles.XML\" Progam terminating...");
                return;
            }
            fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"DataFiles\Items.xml");
            if (!File.Exists(fileName))
            {
                MessageBox.Show("DataFile is missing: \"Items.XML\" Progam terminating...");
                return;
            }
            //Well thats a good sign, isnt it? Now we can enter the game
            using (ScreenManager manager = new ScreenManager())
            {
                manager.Run();
            }
        }


    }
#endif
}