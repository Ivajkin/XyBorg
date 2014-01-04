using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using System.IO;

namespace XyBorg
{
    static class Message
    {
        //public static void Show()
        //{
        //    /*XyBorg.GUI.Add("Pause Game GUI Element Set", new XyBorg.GUIElements.Canvas(new Vector2(0, 0), new Vector2(0, 0), "Overlays/game_paused"));
        //    XyBorg.GUI.Add("Pause Game GUI Element Set", new XyBorg.GUIElements.Text("Pause Game Overlay Test", new Vector2(-135, -300)));
        //    XyBorg.GUI.Add("Pause Game GUI Element Set", new XyBorg.GUIElements.Button(new Vector2(0, 0), new Vector2(0, 0), "Overlays/OK_button_normal", "Overlays/OK_button_hover", "Overlays/OK_button_pressed", delegate() { Hide(); PauseGame(false); }));*/
        //    //GUI.Create(GUI.element_t.canvas);
            
        //}
        //public static void Hide()
        //{
        //    //GUI.Destroy("Pause Game GUI Element Set");
        //}

        public const int lasergun_collected_message_num = 5;
        public const int minigun_collected_message_num = 6;
        public const int healthpack_collected_message_num = 7;
        public const int armor_collected_message_num = 8;
        public const int grenades_collected_message_num = 9;
        public static void HelpMessage(int help_message_num)
        {
            help_message_num = Math.Min(Math.Max(help_message_num, 1),20);

            string path = "Overlays/Tips/Tip" + help_message_num + ".txt";
            path = Path.Combine(
                    Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation,
                    "Content/" + path);

            if (!File.Exists(path))
                throw new Exception("Подсказки #" + help_message_num + " не существует (файл '" + path + "').");
            StreamReader sr = new StreamReader(path);
            string help_message = sr.ReadToEnd();

            string set_name = "Tip" + help_message_num;
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Canvas(new Vector2(0, 0), new Vector2(0, 0), "Overlays/help_message"));
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Text("Подсказка #" + help_message_num+":", new Vector2(-245, -160), XyBorg.GUIElements.Text.font_big));
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Button(new Vector2(100, 100), new Vector2(0, 0), "Overlays/OK_button_normal", "Overlays/OK_button_hover", "Overlays/OK_button_pressed", delegate() { GUI.Destroy(set_name); }));

            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Button(new Vector2(-180, 100), new Vector2(0, 0), "Overlays/back_button_normal", "Overlays/back_button_hover", "Overlays/back_button_pressed", delegate() { GUI.Destroy(set_name); HelpMessage(help_message_num - 1); }));
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Button(new Vector2(-100, 100), new Vector2(0, 0), "Overlays/next_button_normal", "Overlays/next_button_hover", "Overlays/next_button_pressed", delegate() { GUI.Destroy(set_name); HelpMessage(help_message_num + 1); }));
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Text(help_message, new Vector2(-240, -100), XyBorg.GUIElements.Text.font_normal));
        }
        public static void ShowProfileSetName(string Question)
        {
            string set_name = "set profile name box";

            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Canvas(new Vector2(0, 0), new Vector2(0, 0), "Overlays/select_profile"));

            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Text(Question, new Vector2(-240, -100), XyBorg.GUIElements.Text.font_big));

            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.InputBox(new Vector2(0, 0), new Vector2(0, 0), XyBorg.GUIElements.InputBox.default_image_path, XyBorg.GUIElements.InputBox.default_focused_image_path, XyBorg.GUIElements.InputBox.default_cursor_image_path, GUIElements.Text.font_big));


            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Button(new Vector2(0, 80), new Vector2(0, 0), "Overlays/default_button_normal", "Overlays/default_button_hover", "Overlays/default_button_pressed", delegate() { GUI.Destroy(set_name); }));
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Text("Да", new Vector2(-140, 70), XyBorg.GUIElements.Text.font_normal));
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Button(new Vector2(0, 115), new Vector2(0, 0), "Overlays/default_button_normal", "Overlays/default_button_hover", "Overlays/default_button_pressed", delegate() { GUI.Destroy(set_name); ShowProfileSetName("Введите ваше имя:"); }));
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Text("Нет", new Vector2(-140, 105), XyBorg.GUIElements.Text.font_normal));
        }
        /*public static void ShowStatusBox(Player player)
        {
            string set_name = "StatusBox";
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Canvas(new Vector2(0, 0), new Vector2(0, 0), "Overlays/view_stats_box"));
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Text("Параметры игрока:", new Vector2(-245, -160), XyBorg.GUIElements.Text.font_big));
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Button(new Vector2(100, 100), new Vector2(0, 0), "Overlays/OK_button_normal", "Overlays/OK_button_hover", "Overlays/OK_button_pressed", delegate() { GUI.Destroy(set_name); }));
            
            XyBorg.GUI.Add(set_name, new XyBorg.GUIElements.Text("Количество энергии: " + player.energy.energy, new Vector2(-240, -100), XyBorg.GUIElements.Text.font_normal));
            
        }*/
    }
}
