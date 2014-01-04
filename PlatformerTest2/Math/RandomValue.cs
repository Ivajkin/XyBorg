using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace XyBorg
{
    class RandomValue
    {
        static Random rand = new Random();

        internal static float get_float(float min,float max)
        {
            return (float)rand.NextDouble() * (max - min) + min;
        }
        internal static int get_int(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}
