using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace XyBorg.PlayerProperties
{
    class Grenade
    {
        SoundEffect grenadeBounce;
        SoundEffect grenadeExplosion;
        /// <summary>
        /// Конструктор.
        /// </summary>
        public Grenade(Vector2 shootPosition, Vector2 _velocity)
        {
            body = new AnimatedEffect("Player/Grenade.afx.txt", shootPosition, SpriteEffects.None);
            AnimatedEffect.AddEffect(body);
            velocity = _velocity;
            dead = false;

            grenadeBounce = Content.Load<SoundEffect>("Sprites/Player/GrenadeBounce");
            grenadeExplosion = Content.Load<SoundEffect>("Sprites/Player/GrenadeExplosion");
        }
        private AnimatedEffect body;
        public void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timeToLive -= time;
            if (timeToLive <= 0)
            {
                dead = true;
                ParticleSystem.Global.PSExplode(5, body.position);
            }
            else
            {
                HandleCollisions();
                velocity *= friction;
                body.position += velocity * time;
                velocity.Y += gravity;
                // Обработка столкновений.
                if (body.position.Y < 0)
                {
                    velocity.Y = -velocity.Y;
                }
            }
        }
        Vector2 velocity;
        public bool dead
        {
            private set
            {
                _dead = value; if (_dead == true)
                {
                    grenadeExplosion.Play();
                    KillAreaEffect kae = new KillAreaEffect(new Rectangle(
                        (int)(body.position.X - 64),
                        (int)(body.position.Y - 64),
                        (int)(128),
                        (int)(128)), 0.4f);
                    Level.current_level.Players.set_vibration(0.2f, 0.2f);
                    AnimatedEffect.RemoveEffect(body);
                }
            }
            get { return _dead; }
        }
        private bool _dead;
        private const float gravity = 20.5f;
        float timeToLive = fullTimeToLive;
        const float fullTimeToLive = 3;
        const float friction = 0.98f;
        const float dumping = 0.6871f;
        int previousBottom;
        Rectangle BoundingRectangle { get { return new Rectangle((int)body.position.X - 16, (int)body.position.Y - 16, 32, 32); } }
        /// <summary>
        /// Определяет и обрабатывает столкновения.
        /// </summary>
        private void HandleCollisions()
        {
            bool bouncedX = false;
            bool bouncedY = false;

            Rectangle bounds = BoundingRectangle;

            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;
            int hMidTile = (topTile + bottomTile) / 2;

            // Для каждого потенциално пересекающего тайла
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // Смотрим проходимость тайла.
                    TileCollision collision = Level.current_level.GetCollision(x, y);

                    if (collision != TileCollision.Passable && collision != TileCollision.Stair)
                    {
                        // Определяем глубину проникновения.
                        Rectangle tileBounds = Level.current_level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Разрешаем столкновение.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // Если пересекли верх тайла
                                if (previousBottom <= tileBounds.Top)
                                {
                                    velocity.Y = -velocity.Y * dumping;
                                    bouncedY = true;
                                }

                                // Игнорируем платформу пока не окажется, что мы на земле.
                                if (collision == TileCollision.Impassable)
                                {
                                    // Разрешаем по Y.
                                    body.position = new Vector2(body.position.X, body.position.Y + depth.Y);
                                    velocity.Y = -velocity.Y * dumping;
                                    bouncedY = true;

                                    // Продолжаем.
                                    bounds = BoundingRectangle;
                                }
                                // Если убивающая платформа - отнимем жизни.
                                if (collision == TileCollision.DeadEnd)
                                {
                                    dead = true;
                                    bouncedY = true;
                                }
                            }
                            else if (collision == TileCollision.Impassable && Math.Abs(velocity.X) > Math.Abs(velocity.Y) && Math.Abs(velocity.Y) > 0.1f)
                            {
                                // Разрешаем по X.
                                body.position = new Vector2(body.position.X + depth.X, body.position.Y);
                                velocity.X = -velocity.X * dumping;
                                bouncedX = true;

                                // Продолжаем.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }
            previousBottom = bounds.Bottom;
            if (bouncedY)
            {
                grenadeBounce.Play(MathHelper.Clamp(Math.Abs(velocity.Y / 180.0f) - 0.2f, 0, 1), 0, 0);
            }
            else if (bouncedX)
            {
                grenadeBounce.Play(MathHelper.Clamp(Math.Abs(velocity.X / 180.0f) - 0.2f, 0, 1), 0, 0);
            }
        }
    }
    class GrenadeManager
    {
        public int grenade_count = 0;
        /// Звуковой эффект (бросок гранаты).
        private SoundEffect grenadeThrow = null;
        protected void Add(Grenade grenade)
        {
            // Звуковой эффект (бросок гранаты) - проверка и загрузка.
            if (grenadeThrow == null)
            {
                grenadeThrow = Content.Load<SoundEffect>("Sprites/Player/GrenadeThrow");
            }
            grenadeThrow.Play();

            // Сам бросок гранаты.
            grenades.Add(grenade);
        }
        private List<Grenade> grenades = new List<Grenade>();
        private bool shoot_btn_was_pressed = false;
        /// <summary>
        /// Обновление.
        /// </summary>
        /// <param name="grenade_shoot_btn_pressed">Нажата ли клавиша запуска гранаты.</param>
        /// <param name="gameTime">Игровое время.</param>
        /// <param name="grenader">Игрок, запускающий гранаты.</param>
        public void Update(GameTime gameTime, bool grenade_shoot_btn_pressed, Player grenader)
        {
            if (grenade_shoot_btn_pressed)
            {
                if (!shoot_btn_was_pressed)
                {
                    // Проверяем - хватит ли гранат.
                    if (grenade_count == 0)
                    {
                    }
                    else
                    {
                        --grenade_count;
                        Add(new Grenade(grenader.Position + new Vector2(grenader.sprite.Animation.FrameWidth / 8 * grenader.Direction, -grenader.sprite.Animation.FrameHeight / 2), grenader.Velocity + new Vector2(800 * grenader.Direction, -800)));
                    }
                    shoot_btn_was_pressed = true;
                }
            }
            else
            {
                shoot_btn_was_pressed = false;
            }
            List<Grenade> RemoveList = new List<Grenade>();
            foreach (Grenade grenade in grenades)
            {
                grenade.Update(gameTime);
                if (grenade.dead)
                {
                    RemoveList.Add(grenade);
                }
            }
            foreach (Grenade grenade in RemoveList)
            {
                grenades.Remove(grenade);
            }
        }
    }
}
