using CURPG_Engine.Core;

namespace CURPG_Windows
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (ScreenManager manager = new ScreenManager(args))
            {
                Logger.Info("Starting...", "CURPG_Windows");
                manager.Run();
            }
        }
    }
}