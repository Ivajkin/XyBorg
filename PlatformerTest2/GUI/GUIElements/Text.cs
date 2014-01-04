using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XyBorg.GUIElements
{
    class Text: BaseElement
    {
        public static SpriteFont font_normal = null;
        public static SpriteFont font_big = null;
        public Text(string text_to_show, Vector2 _position, SpriteFont _font)
        {
            font = _font;
            if (font == null)
            {
                font = font_normal;
            }
            text = text_to_show;
            position = _position;
        }
        private SpriteFont font = null;
        private string text = "";
        private Vector2 position;
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (font == null)
                throw new Exception("Шрифт не инициализирован до создания окна с тектом.");
            spriteBatch.DrawString(font, text, position + new Vector2(1.0f, 1.0f) + GUI.window_half_size, Color.Black);
            spriteBatch.DrawString(font, text, position + GUI.window_half_size, Color.White);
        }
        public override void HandleInput(MouseState mstate, KeyboardState kstate)
        {
        }
    }
}