using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;

namespace XyBorg.Utility
{
    /// <summary>
    /// Вспомогательный класс, выдаёт текущее реальное время.
    /// </summary>
    static public class Time
    {
        /// <summary>
        /// Выдаёт текущее время в секундах.
        /// </summary>
        /// <returns>Текущее время в секундах.</returns>
        static public float getTime() {
            DateTime dt = DateTime.Now;
            return dt.Hour * 3600.0f + dt.Minute * 60.0f + dt.Second + dt.Millisecond/1000.0f;
        }
    }
}
