using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;

namespace XyBorg
{
    class MainMenu
    {
        public delegate void start_game_delegate();
        public delegate void exit_game_delegate();
        int screen_size_x;
        int screen_size_y;
        public MainMenu(int _screen_size_x, int _screen_size_y)
        {
            screen_size_x = _screen_size_x;
            screen_size_y = _screen_size_y;
            cursor_tex = Content.Load<Texture2D>("Menu/cursor");
        }
        public void Reset(start_game_delegate start_function, exit_game_delegate exit_function)
        {
            isEnabled = true;

            string set_name = "Главное меню";

            GUI.Add(set_name, new GUIElements.VideoCanvas(new Vector2(0, 0), new Vector2(screen_size_x, screen_size_y), "Menu/background", false));
            GUI.Add(set_name, new GUIElements.Button(new Vector2(0, -150 * screen_size_y / 720), new Vector2(0, 0), "Menu/start_normal", "Menu/start_hover", "Menu/start_pressed", delegate() { isEnabled = false; GUI.Destroy(set_name); start_function(); }));
            GUI.Add(set_name, new GUIElements.Button(new Vector2(0, -30 * screen_size_y / 720), new Vector2(0, 0), "Menu/exit_normal", "Menu/exit_hover", "Menu/exit_pressed", delegate() { GUI.Destroy(set_name); exit_function(); }));

        }
        Texture2D cursor_tex;
        public bool isEnabled { get; private set; }
        public void Draw(SpriteBatch spriteBatch)
        {
            MouseState mstate = Mouse.GetState();
            
            spriteBatch.Begin();
            {
                GUI.Draw(spriteBatch);
                spriteBatch.Draw(cursor_tex, new Vector2(mstate.X, mstate.Y), Color.White);
            }
            spriteBatch.End();
            XyBorg.GUI.HandleInput();
        }
    }
}
