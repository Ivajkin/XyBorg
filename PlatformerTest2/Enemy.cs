using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace XyBorg
{
    /// <summary>
    /// Направление по оси X.
    /// </summary>
    enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }

    /// <summary>
    /// Монстр, преграждающий дорогу.
    /// </summary>
    class Enemy
    {
        // Набор звуков
        private SoundEffect killedSound;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Положение в мировых координатах центра низа врага.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;

        private Rectangle localBounds;
        /// <summary>
        /// Выдаёт прямоугольник, окружающий врага в мировых координатах.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        // Анимации
        private Animation runAnimation;
        private Animation idleAnimation;
        private Animation dieAnimation;
        private AnimationPlayer sprite;

        public bool IsAlive
        {
            get { return isAlive; }
        }
        private bool isAlive = true;
        /// <summary>
        /// Направление куда враг смотрит и движется.
        /// </summary>
        private FaceDirection direction = FaceDirection.Left;

        /// <summary>
        /// Как долго враг стоит прежде чем повернуться.
        /// </summary>
        private float waitTime;

        /// <summary>
        /// Как долго враг будет стоять прежде чем повернуться.
        /// </summary>
        private const float MinWaitTime = 0.5f;
        private const float MaxWaitTime = 3.5f;

        /// <summary>
        /// Скорость движения по оси X.
        /// </summary>
#if ZUNE
        private const float MoveSpeed = 55.0f;
#else
        private const float MoveSpeed = 110.0f;
#endif

        /// <summary>
        /// Создаёт нового врага.
        /// </summary>
        public Enemy(Level level, Vector2 position, string spriteSet)
        {
            this.level = level;
            this.position = position;

            LoadContent(spriteSet);
        }

        /// <summary>
        /// Заружает набор спрайтов и звуки для определённого типа врага.
        /// </summary>
        public void LoadContent(string spriteSet)
        {
            // Загрузка анимации.
            spriteSet = "Sprites/" + spriteSet + "/";
            runAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(spriteSet + "Run"), 0.04f, true);
            idleAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(spriteSet + "Idle"), 0.04f, true);
            dieAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(spriteSet + "Die"), 0.04f, false);
            sprite.PlayAnimation(idleAnimation);

            // Рассчитывает размеры исходя из размеров текстуры.
            int width = (int)(idleAnimation.FrameWidth * 0.35);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Загружаем звуки смерти.
            killedSound = /*Level.*/Content.Load<SoundEffect>(spriteSet + "Killed");

            // Загружаем звуки атаки.
            for (int i = 0; i < attack_sound.Length; i++)
            {
                attack_sound[i] = /*Level.*/Content.Load<SoundEffect>(spriteSet + "Attack" + i);
            }
        }


        /// <summary>
        /// Расхаживает взад-вперёд по платформе.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (!IsAlive) return;

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Рассчитываем позицию тайла, в зависимости от
            // направления в котором движемся.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if (waitTime > 0)
            {
                // Ждём некоторое время.
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    // Тогда поворачиваемся.
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else
            {
                // Если мы собираемся врезаться в стену или свалиться с обрыва - пора отдохнуть.
                if (Level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                    Level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable)
                {
                    waitTime = RandomValue.get_float( MinWaitTime, MaxWaitTime);
                }
                else
                {
                    // Движемся в текущем направлении.
                    Vector2 velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    position = position + velocity;
                }
            }
        }

        /// <summary>
        /// Вызываем когда враг убит.
        /// </summary>
        /// <param name="killedBy">
        /// Игрок.
        /// </param>
        public void OnKilled(Player killedBy)
        {
            if (IsAlive)
            {
                isAlive = false;
                sprite.PlayAnimation(dieAnimation);
                killedSound.Play();
            }
        }

        /// <summary>
        /// Отрисовываем анемированного врага.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            // Ждём если игра на паузе или пора отдохнуть.
            if (!IsAlive)
            {
                sprite.PlayAnimation(dieAnimation);
            }
            else if (!Level.Players.IsAlive ||
                Level.ReachedExit ||
                Level.TimeRemaining == TimeSpan.Zero ||
                waitTime > 0)
            {
                sprite.PlayAnimation(idleAnimation);
            }
            else
            {
                sprite.PlayAnimation(runAnimation);
            }


            // Отрисовываем, повернув ту сторону, в которую идём.
            SpriteEffects flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }

        private const float attack = 30;
        private const float attack_countdown = 0.5f;
        private float current_countdown = attack_countdown;
        private SoundEffect[] attack_sound = new SoundEffect[8];
        internal void OnPlayerIntersect(Player player, GameTime gameTime)
        {
            if (current_countdown >= attack_countdown)
            {
                current_countdown -= attack_countdown;
                attack_sound[RandomValue.get_int(0, attack_sound.Length)].Play();
                player.OnDamaged(this,attack);
                //if (player.HealthPoints <= 0)
                //    player.OnKilled(this);
            }
            else
            {
                current_countdown += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
    }
}
