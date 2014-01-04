using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XyBorg
{
    class WorldMap
    {
        public delegate void start_game_delegate(int level_to_start, bool editor_enabled);
        public delegate void go_to_menu_mode_delegate();
        bool isEnabled = false;
        bool isInitialised = false;
        Texture2D cursor_tex;
        const string guiSetName = "Карта мира";
        /// <summary>
        /// Загрузка содержимого - фон и другое.
        /// </summary>
        /// <param name="screen_size">Размеры экрана.</param>
        internal void LoadContent(Vector2 screen_size)
        {
            isInitialised = true;

            cursor_tex = Content.Load<Texture2D>("Menu/cursor");
        }
        /// <summary>
        /// Восстановление интерфейса.
        /// </summary>
        /// <param name="screen_size">Размеры экрана.</param>
        /// <param name="start_game_function">Функция для начала игры.</param>
        /// <param name="go2menu">Функция для возврата к главному меню.</param>
        internal void Reset(Vector2 screen_size, start_game_delegate start_game_function, go_to_menu_mode_delegate go2menu)
        {
            GUI.Add(guiSetName, new GUIElements.VideoCanvas(Vector2.Zero, screen_size, "WorldMap/background", false));
            GUI.Add(guiSetName, new GUIElements.Button(new Vector2(0, -150 * screen_size.Y / 720), new Vector2(0, 0), "Menu/start_normal", "Menu/start_hover", "Menu/start_pressed", delegate() { isEnabled = false; GUI.Destroy(guiSetName); start_game_function(1, true); }));
            GUI.Add(guiSetName, new GUIElements.Button(new Vector2(0, -30 * screen_size.Y / 720), new Vector2(0, 0), "Menu/exit_normal", "Menu/exit_hover", "Menu/exit_pressed", delegate() { GUI.Destroy(guiSetName); go2menu(); }));

            isEnabled = true;
        }
        internal void Update(GameTime gameTime)
        {
            if (!isInitialised)
                throw new Exception("Карта мира не инициализирована, но обновляется.");
            XyBorg.GUI.HandleInput();
        }
        internal void Draw(SpriteBatch spriteBatch)
        {
            if (!isInitialised)
                throw new Exception("Карта мира не инициализирована, но отрисовывается.");
            MouseState mstate = Mouse.GetState();

            spriteBatch.Begin();
            {
                GUI.Draw(spriteBatch);
                spriteBatch.Draw(cursor_tex, new Vector2(mstate.X, mstate.Y), Color.White);
            }
            spriteBatch.End();
        }
    }
}
