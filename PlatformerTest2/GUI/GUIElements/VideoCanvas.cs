using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace XyBorg.GUIElements
{
    /// <summary>
    /// Холст содержащий видео в себе.
    /// </summary>
    class VideoCanvas : BaseElement
    {
        /// <summary>
        /// Конструктор для видеохолста.
        /// </summary>
        /// <param name="center_position">Координаты центра изображения, {0,0} - середина экрана.</param>
        /// <param name="size">Размер, {0,0} если как у видео.</param>
        /// <param name="video_path">Путь к видеофайлу.</param>
        /// <param name="mute">Проигрывать беззвучно или нет.</param>
        public VideoCanvas(Vector2 center_position, Vector2 size, string video_path, bool mute)
        {
            video = Content.Load<Video>(video_path);
            video_player = new VideoPlayer();
            video_player.Play(video);
            video_player.IsLooped = true;
            video_player.IsMuted = mute;
            if (size.X != 0 || size.Y != 0)
            {
                _size = size;
            }
            else
            {
                _size = new Vector2(video.Width, video.Height);
            }
            center = center_position;
        }
        internal override void Dispose()
        {
            if (!video_player.IsDisposed)
                video_player.Stop();
            video_player.Dispose();
            base.Dispose();
        }
    
        protected Video video;
        protected VideoPlayer video_player;
        protected Vector2 _pos;
        protected Vector2 _size;
        public Vector2 center
        {
            get { return _pos + _size * 0.5f - GUI.window_half_size; }
            set { _pos = value - _size * 0.5f + GUI.window_half_size; }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D image = video_player.GetTexture();
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
    }
}