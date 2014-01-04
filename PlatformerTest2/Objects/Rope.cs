using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XyBorg
{
    class Rope
    {
        public const int max_length = 300;
        public const int chain_size = 11;
        public Vector2 start_position { private set; get; }
        public Rectangle intersection_rect { private set; get; }
        public Rope(Vector2 position)
        {
            using_player = null;
            chain_sprite = Content.Load<Texture2D>("Sprites/Player/RopeChain");
            start_position = position;
            intersection_rect = new Rectangle((int)(position.X - 10), (int)(position.Y + 4), 20, max_length);
        }
        /*public void SetInvisible(bool invisible)
        {
            isInvisible = invisible;
        }
        /// <summary>
        /// Отображается ли верёвка.
        /// </summary>
        public bool isInvisible { private set; get; }*/
        /// <summary>
        /// Игрок, который использует верёвку, нет если null.
        /// </summary>
        public Player using_player { private set; get; }
        /// <summary>
        /// Вызывается при соприкосновении с игроком.
        /// </summary>
        /// <param name="collidedBy">Игрок, null если нет.</param>
        public void OnCollision(Player collidedBy)
        {
            using_player = collidedBy;
        }

        static Texture2D chain_sprite = null;
        internal void Draw(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (using_player == null)
            {
                for (float dy = 0; dy < intersection_rect.Height; dy += Rope.chain_size)
                {
                    spriteBatch.Draw(chain_sprite, start_position + new Vector2(0, dy), Color.White);
                }
            }
        }

        internal void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
        }
    }
}
