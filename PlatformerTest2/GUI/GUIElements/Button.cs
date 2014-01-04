using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace XyBorg.GUIElements
{
    class Button : Canvas
    {
        public Button(Vector2 center_position, Vector2 size, string image_path, string hover_image_path, string pressed_image_path, on_button_pressed _on_button_pressed_callback)
            : base(center_position, size, image_path)
        {
            on_button_pressed_callback = _on_button_pressed_callback;

            // Если аргументы hover_image_path и pressed_image_path не равны null загружаем их, иначе берём значение основного избражения.
            if (hover_image_path != null)
                hover = Content.Load<Texture2D>(hover_image_path);
            else
                hover = image;

            if (pressed_image_path != null)
                pressed = Content.Load<Texture2D>(pressed_image_path);
            else
                pressed = image;
        }
        public delegate void on_button_pressed();
        public on_button_pressed on_button_pressed_callback;
        private Texture2D hover;
        private Texture2D pressed;
        enum state_t
        {
            hover,
            pressed,
            normal
        };
        state_t state = state_t.normal;
        public override void Draw(SpriteBatch spriteBatch)
        {
            switch (state)
            {
                case state_t.normal:
                    spriteBatch.Draw(image, new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y), Color.White);
                    break;
                case state_t.hover:
                    spriteBatch.Draw(hover, new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y), Color.White);
                    break;
                case state_t.pressed:
                    spriteBatch.Draw(pressed, new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y), Color.White);
                    break;
            }
        }
        public override void HandleInput(MouseState mstate, KeyboardState kstate)
        {
            //if (mstate.X > _pos.X && mstate.X < _pos.X + _size.X && mstate.Y > _pos.Y && mstate.Y < _pos.Y + _size.Y)
            if (IsCanvasContains(mstate.X, mstate.Y))
            {
                if (mstate.LeftButton == ButtonState.Pressed)
                {
                    state = state_t.pressed;
                }
                else
                {
                    if (state == state_t.pressed)
                        on_button_pressed_callback();
                    state = state_t.hover;
                }
            }
            else
            {
                state = state_t.normal;
            }
        }
    }
}
