using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XyBorg
{
    class Layer
    {
        public Texture2D Texture { get; private set; }
        /// <summary>
        /// Скорость прокрутки, зависит от расстояния.
        /// </summary>
        public Vector2 ScrollRate { get; private set; }
        private Vector2 selfScrollPosition = Vector2.Zero;
        private Vector2 selfScrollSpeed = Vector2.Zero;
        private float start_position_y;
        public void setSelfScrollSpeed(Vector2 _selfScrollSpeed)
        {
            selfScrollSpeed = _selfScrollSpeed;
        }

        public Layer(ContentManager content, int stage, int layer, float _scrollRate, int level_size_in_tiles)
        {
            Vector2 scrollRate = new Vector2(_scrollRate, _scrollRate);
            //// 3 сегмента на слой
            //Textures = new Texture2D[3];
            //for (int i = 0; i < 3; ++i)
            //    Textures[i] = content.Load<Texture2D>("Backgrounds/Level" + stage + "_layer" + layer + "_part" + i);
            //Texture = content.Load<Texture2D>("Levels/Backgrounds/Level" + stage + "_layer" + layer);
            Texture = content.Load<Texture2D>("Levels/Level" + stage + "/BackgroundLayer" + layer);


            start_position_y = level_size_in_tiles * Tile.Size.Y - Texture.Height;

            ScrollRate = scrollRate;
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition, float ellapsedTime)
        {
            selfScrollPosition += selfScrollSpeed * ellapsedTime;
            cameraPosition.Y = -cameraPosition.Y - start_position_y;
            cameraPosition += selfScrollPosition;

            // Ширина должна быть одинаковая для всех слоёв.
            // а вот и нет теперь
            int segmentWidth = Texture.Width;
            int segmentHeight = Texture.Height;

            //Рассчитываем что рендерить и как сильно это сдвигать.
            Vector2 pos = cameraPosition * ScrollRate;
            int leftSegment = (int)Math.Floor(pos.X / segmentWidth);
            int rightSegment = leftSegment + 1;
            int topSegment = (int)Math.Floor(pos.Y / segmentHeight);
            int bottomSegment = topSegment + 1;
            pos.X = (pos.X / segmentWidth - leftSegment) * -segmentWidth;
            pos.Y = (pos.Y / segmentHeight - topSegment) * -segmentHeight;

            // Сама отрисовка.
            /*
            spriteBatch.Draw(Texture, new Vector2(pos.X, pos.Y + segmentHeight), Color.White);
            spriteBatch.Draw(Texture, new Vector2(pos.X + segmentWidth, pos.Y + segmentHeight), Color.White);
            spriteBatch.Draw(Texture, new Vector2(pos.X, pos.Y + 2*segmentHeight), Color.White);
            spriteBatch.Draw(Texture, new Vector2(pos.X + segmentWidth, pos.Y + 2*segmentHeight), Color.White);*/

            spriteBatch.Draw(Texture, new Vector2(pos.X, pos.Y), Color.White);
            spriteBatch.Draw(Texture, new Vector2(pos.X + segmentWidth, pos.Y), Color.White);

            spriteBatch.Draw(Texture, new Vector2(pos.X, pos.Y + segmentHeight), Color.White);
            spriteBatch.Draw(Texture, new Vector2(pos.X + segmentWidth, pos.Y + segmentHeight), Color.White);
        }
    }
}
