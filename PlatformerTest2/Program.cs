using System;

namespace XyBorg
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            /*using (MenuGameMode menu = new MenuGameMode())
            {
                menu.Run();
                menu.Dispose();
            }*/
            XyBorg.Security.SecuritySystem sys = new XyBorg.Security.SecuritySystem("./Content/");
            if (sys.fails())
            {
                return;
            }
            using (PlatformerGame game = new PlatformerGame())
            {
                game.Run();
            }
            XyBorg.Utility.Profiler.FlushTimeData();
        }
    }
}

