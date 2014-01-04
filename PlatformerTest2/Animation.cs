using System;
using Microsoft.Xna.Framework.Graphics;

namespace XyBorg
{
    /// <summary>
    /// Представляет анимированную текстуру.
    /// </summary>
    /// <remarks>
    /// Ограничение - каждый кадр представлен квадратом
    /// ( ширина равна высоте ).
    /// </remarks>
    class Animation
    {
        /// <summary>
        /// Все кадры анимации в одной текстуре, по горизонтали.
        /// </summary>
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        /// <summary>
        /// Длительность одного кадра.
        /// </summary>
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        /// <summary>
        /// Повторяется ли анимация?
        /// </summary>
        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        /// <summary>
        /// Выдаёт количество кадров в анимации.
        /// </summary>
        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        /// <summary>
        /// Выдаёт ширину кадра анимации.
        /// </summary>
        public int FrameWidth
        {
            // Assume square frames.
            get { return Texture.Height; }
        }

        /// <summary>
        /// Выдаёт высоту кадра анимации.
        /// </summary>
        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        /// <summary>
        /// Создаёт новую анимацию.
        /// </summary>        
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
        }
    }
}
