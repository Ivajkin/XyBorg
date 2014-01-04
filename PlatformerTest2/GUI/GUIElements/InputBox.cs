
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XyBorg.GUIElements
{
    class InputBox : Canvas
    {
        public InputBox(Vector2 center_position, Vector2 size, string image_path, string focused_image_path, string cursor_image_path, SpriteFont _font)
            : base(center_position, size, image_path)
        {
            focused_image = /*Level.current_level.*/Content.Load<Texture2D>(focused_image_path);
            cursor = /*Level.current_level.*/Content.Load<Texture2D>(cursor_image_path);
            font = _font;
            
            Array keys = System.Enum.GetValues(typeof(Keys));
            foreach (Keys key in keys)
            {
                this.last_pressed_state[key] = false;
            }
        }
        SpriteFont font = null;
        /// <summary>
        /// Изображение элемента ввода в случае, когда фокус направлен на него, можно вводить.
        /// </summary>
        private Texture2D focused_image;
        /// <summary>
        /// Изображение курсора.
        /// </summary>
        private Texture2D cursor;
        /// <summary>
        /// Позиция курсора, начиная с нуля.
        /// Например: SomeTe|xt - значит 6
        /// </summary>
        private uint cursor_position = 0;
        /// <summary>
        /// Имеет значение направлен ли фокус на него, можно ли вводить.
        /// </summary>
        private bool have_focus = false;
        /// <summary>
        /// Собственно сам введённый текст.
        /// </summary>
        private string text = "";
        /// <summary>
        /// Отрисовка элемента.
        /// </summary>
        public override void Draw(SpriteBatch spriteBatch)
        {
            if(have_focus)
                spriteBatch.Draw(focused_image, new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y), Color.White);
            else
                spriteBatch.Draw(image, new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y), Color.White);

            if (font == null)
                throw new Exception("Шрифт не инициализирован до создания окна ввода.");
            spriteBatch.DrawString(font, text, _pos + spacing + new Vector2(1.0f, 1.0f), Color.Black);
            if (have_focus)
                spriteBatch.DrawString(font, text, _pos + spacing, Color.White);
            else
                spriteBatch.DrawString(font, text, _pos + spacing, Color.Gray);

            Vector2 text_size = font.MeasureString(text.Substring(0, (int)cursor_position));
            spriteBatch.Draw(cursor, _pos + new Vector2(text_size.X, focused_image.Height/2 - cursor.Height / 2), Color.White);
        }
        /// <summary>
        /// отступ слева и сверху.
        /// </summary>
        Vector2 spacing = new Vector2(3, 3);
        /// <summary>
        /// Получение ввода.
        /// </summary>
        public override void HandleInput(MouseState mstate, KeyboardState kstate)
        {
            // Мышинная часть
            if (IsCanvasContains(mstate.X,mstate.Y))
            {
                if (mstate.LeftButton == ButtonState.Pressed)
                {
                    have_focus = true;
                }
            }
            else
            {
                if (mstate.LeftButton == ButtonState.Pressed)
                {
                    have_focus = false;
                }
            }
            // Клавиатурная часть.
            if (!have_focus)
                return;
            bool shiftIsPressed = kstate.IsKeyDown(Keys.LeftShift);
            //Keys[] keys = kstate.GetPressedKeys();
            //if (keys.Length == 0)
            //    return;

            Array keys = System.Enum.GetValues(typeof(Keys));
            foreach (Keys key in keys)
            {
                if (kstate.IsKeyDown(key))
                {
                    if (!last_pressed_state[key])
                    {
                        HandleKey(key, shiftIsPressed);
                        last_pressed_state[key] = true;
                    }
                }
                else
                {
                    last_pressed_state[key] = false;
                }
            }
            /*foreach (Keys key in keys)
            {
                HandleKey(key);
            }*/
        }
        /// <summary>
        /// Соварь содержит значения того, нажата ли была кнопка в прошлых кадрах (ещё не отпущена).
        /// </summary>
        Dictionary<Keys, bool> last_pressed_state = new Dictionary<Keys,bool>();
        private void HandleKey(Keys key, bool shiftIsPressed)
        {
            switch (key)
            {
                case Keys.Left:
                    if (cursor_position > 0)
                        cursor_position--;
                    return;
                case Keys.Right:
                    if (text.Length > cursor_position)
                        cursor_position++;
                    return;
                case Keys.Space:
                    text.Insert((int)cursor_position, " ");
                    return;
                case Keys.Delete:
                    if (cursor_position < text.Length)
                        text = text.Substring(0, (int)cursor_position) + text.Substring((int)cursor_position+1);
                    return;
                case Keys.Back:
                    if (cursor_position > 0)
                    {
                        text = text.Substring(0, (int)cursor_position - 1) + text.Substring((int)cursor_position);
                        cursor_position--;
                    }
                    return;
            }
            char ch;
            if (char.TryParse(key.ToString(), out ch))
            {
                if(shiftIsPressed)
                    text = text.Substring(0, (int)cursor_position) + ch.ToString().ToUpper() + text.Substring((int)cursor_position);
                else
                    text = text.Substring(0, (int)cursor_position) + ch.ToString().ToLower() + text.Substring((int)cursor_position);
                if (text.Length > cursor_position)
                    cursor_position++;
            }
        }
        public const string default_image_path          = "Overlays/inputbox";
        public const string default_focused_image_path  = "Overlays/inputbox_active";
        public const string default_cursor_image_path   = "Overlays/inputbox_cursor";
    }
}
