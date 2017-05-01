using System;
using CURPG_Engine;

namespace CURPG_MapViewer
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            CURPG_Engine.Core.World world = null;
            using (var game = new Game1(world))
                game.Run();
        }
    }
#endif
}
