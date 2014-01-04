using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Configurator
{
    /*
     * # Основные настройки
     * # fullscreen - полноэкранный режим - true|false - все значения кроме true считаются false
     * fullscreen = false
     * # screen_x, screen_y - разрешение экрана - по умолчанию ( движок на это рассчитан ) 1280 на 720
     * screen_x = 1280
     * screen_y = 720
     * use_hdr = true
     */
    /*class Config
    {
        public const string base_comment = "# Основные настройки";

        Property<bool> fullscreen = new Property<bool>(
            "# fullscreen - полноэкранный режим - true|false - все значения кроме true считаются false",
            "",
            "",
            true,
            ;

        class Property<T>
        {
            public Property(string _comment, string _internal_name, string _name_to_show, T _value, parse_delegate _value_parser)
            {
                comment = _comment;
                internal_name = _internal_name;
                name_to_show = _name_to_show;
                value = _value;
                value_parser = _value_parser;
            }
            parse_delegate value_parser;
            private delegate string parse_delegate(T val);
            private string comment = "# fullscreen - полноэкранный режим - true|false - все значения кроме true считаются false";
            private string internal_name = "fullscreen = ";
            private string name_to_show = "Запускать в полноэкранном режиме:";
            public T value;
        }
    }*/
    public static class Config
    {
        public static void Write(string path)
        {
            path = "Content/" + path;
            File.SetAttributes(path, FileAttributes.Normal);
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine("# Основные настройки");
            sw.WriteLine("# fullscreen - полноэкранный режим - true|false - все значения кроме true считаются false");
            sw.WriteLine("fullscreen = " + (fullscreen ? "true" : "false"));
            sw.WriteLine("# screen_x, screen_y - разрешение экрана - по умолчанию ( движок на это рассчитан ) 1280 на 720");
            sw.WriteLine("screen_x = " + screen_x);
            sw.WriteLine("screen_y = " + screen_y);
            sw.WriteLine("# use_hdr - используется ли эффект HDR - true|false - все значения кроме true считаются false");
            sw.WriteLine("use_hdr = " + (use_hdr ? "true" : "false"));
            sw.Close();
        }
        public static void Read(string path)
        {
            XyBorg.ConfigParser cp = new XyBorg.ConfigParser(path);
            fullscreen = cp.GetBool("fullscreen");
            screen_x = cp.GetInt("screen_x");
            screen_y = cp.GetInt("screen_y");
            use_hdr = cp.GetBool("use_hdr");
        }
        public static bool fullscreen = false;
        public static int screen_x = 1280;
        public static int screen_y = 720;
        public static bool use_hdr = true;
    }
}
