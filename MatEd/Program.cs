using System;

namespace MatEd
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MatEdMain game = new MatEdMain())
            {
                game.Run();
            }
        }
    }
}

