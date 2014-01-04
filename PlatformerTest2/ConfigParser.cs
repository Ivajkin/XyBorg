using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;

namespace XyBorg
{
    /// <summary>
    /// Класс парсит конфигурационный файл, переданный ему при инициализации
    /// (в конструкторе).
    /// </summary>
    class ConfigParser
    {
        public ConfigParser(string file_name)
        {
            string name = Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation + "\\Content\\" + file_name;
            StreamReader r = new StreamReader(name);
            while (!r.EndOfStream)
            {
                string line = r.ReadLine();
                string[] line_pair = line.Split(" =:".ToCharArray());
                if (line_pair[0] != "#")
                {
                    names.Add(line_pair[0]);
                    values.Add(line_pair[line_pair.Length - 1]);
                }
            }
        }
        private List<string> names = new List<string>();
        private List<string> values = new List<string>();
        /// <summary>
        /// Выдаёт значение как строку.
        /// </summary>
        /// <param name="name">Искомый параметр.</param>
        /// <returns>Найденное свойство.</returns>
        public string GetValue(string name) {
            int index = names.FindIndex(name.Equals);
            return values[index];
        }
        public bool GetBool(string name)
        {
            return (GetValue(name) == "true");
        }
        public int GetInt(string name)
        {
            return int.Parse(GetValue(name));
        }
    }
}
