using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace XyBorg.GUIElements
{
    class Canvas : BaseElement/*, IDisposable*/
    {
        public Canvas(Vector2 center_position, Vector2 size, string image_path)
        {
            image = Content.Load<Texture2D>(image_path);
            if (size.LengthSquared() > 0)
            {
                _size = size;
            }
            else
            {
                _size = new Vector2(image.Width, image.Height);
            }
            center = center_position;
        }
        protected Texture2D image;
        protected Vector2 _pos;
        protected Vector2 _size;
        public Vector2 center
        {
            get { return _pos + _size * 0.5f - GUI.window_half_size; }
            set { _pos = value - _size * 0.5f + GUI.window_half_size; }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(image, new Rectangle((int)_pos.X, (int)_pos.Y, (int)_size.X, (int)_size.Y), Color.White);
        }
        public override void HandleInput(MouseState mstate, KeyboardState kstate)
        {
        }
        /// <summary>
        /// Выдаёт значение, находится ли точка внутри границ элемента.
        /// </summary>
        /// <param name="pointX">Координата X точки.</param>
        /// <param name="pointY">Координата Y точки.</param>
        /// <returns>Находится ли точка внутри границ элемента.</returns>
        protected bool IsCanvasContains(int pointX, int pointY)
        {
            pointX += 18;
            pointY += 18;
            return (pointX > _pos.X
                && pointX < _pos.X + _size.X
                && pointY > _pos.Y
                && pointY < _pos.Y + _size.Y);
        }

        /*#region Члены IDisposable

        public void Dispose()
        {
            //image.Dispose();
        }

        #endregion*/
    }
}