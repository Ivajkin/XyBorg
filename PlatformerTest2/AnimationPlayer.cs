using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XyBorg
{
    /// <summary>
    /// Контролирует проигрывание анимации Animation.
    /// </summary>
    struct AnimationPlayer
    {
        /// <summary>
        /// Выдаёт анимацию, проигрываемую в данный момент.
        /// </summary>
        public Animation Animation
        {
            get { return animation; }
        }
        Animation animation;

        /// <summary>
        /// Выдаёт индекс текущего кадра в анимации.
        /// </summary>
        public int FrameIndex
        {
            get { return frameIndex; }
        }
        int frameIndex;

        /// <summary>
        /// Время в течении которого текущий кадр отображался.
        /// </summary>
        private float time;

        /// <summary>
        /// Выдаёт центр кадра - центр низа кадра.
        /// </summary>
        public Vector2 Origin
        {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
        }

        /// <summary>
        /// Начинает или продолжает проигрыш анимации.
        /// </summary>
        public void PlayAnimation(Animation animation)
        {
            // Если эта анимация уже запущена продолжаем.
            if (Animation == animation)
                return;

            // Запускаем другую анимацию.
            this.animation = animation;
            this.frameIndex = 0;
            this.time = 0.0f;
        }
        /// <summary>
        /// Обновляет время и отрисовывает текущий кадр анимации.
        /// </summary>
        /// <param name="gameTime">Время.</param>
        /// <param name="spriteBatch"></param>
        /// <param name="position">Точка отрисовки.</param>
        /// <param name="spriteEffects">Повороты - преобразования.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects)
        {
            Draw(gameTime, spriteBatch, position, spriteEffects, 1.0f);
        }

        /// <summary>
        /// Обновляет время и отрисовывает текущий кадр анимации.
        /// </summary>
        /// <param name="gameTime">Время.</param>
        /// <param name="spriteBatch"></param>
        /// <param name="position">Точка отрисовки.</param>
        /// <param name="spriteEffects">Повороты - преобразования.</param>
        /// <param name="scale">Масштаб спрайта.</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffects, float scale)
        {
            if (Animation == null)
                throw new NotSupportedException("No animation is currently playing.");
            int prev_frame_index = frameIndex;

            // Обновляем время.
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time > Animation.FrameTime)
            {
                time -= Animation.FrameTime;

                // Обновляем индекс кадра, поторяем анимацию или ставим равной пределу
                //( последнему кадру )
                frameIndex = ( Animation.IsLooping ?
                    (frameIndex + 1) % Animation.FrameCount : Math.Min(frameIndex + 1, Animation.FrameCount - 1));
            }

            // Определяем положение текущего и предыдущего кадра.
            Rectangle prev_source = new Rectangle(prev_frame_index * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);
            Rectangle source = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);

            // Отрисовываем текущий кадр.            
            spriteBatch.Draw(Animation.Texture, position, source, Color.White, 0.0f, Origin, scale, spriteEffects, 0.0f);
            //float lerp_fade = time / Animation.FrameTime;
        }
    }
}
