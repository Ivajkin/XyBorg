using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace XyBorg
{
    /// <summary>
    /// Предмет который может поднять игрок, чтобы получить очки.
    /// </summary>
    class Item
    {
        public enum item_type
        {
            item,
            minigun,
            lasergun,
            speed_up_bonus,
            slow_down_bonus,
            armor_light,
            armor_middle,
            armor_heavy,
            grenades_pack,
            health_pack
        };
        protected Animation texture;
        private AnimationPlayer sprite;

        private Vector2 origin;
        protected SoundEffect collectedSound;

        public const int PointValue = 3;
        // Цвет окраски.
        public readonly Color Color = RandomColor;
        // Выдаёт случайный цвет.
        static private Color RandomColor
        {
            get {
                Random random = new Random();
                Color ret;
                int n = random.Next(4);
                if(n==0) {
                    ret = Color.Blue;
                }
                else if (n == 1)
                {
                    ret = Color.Green;
                }
                else if (n == 2)
                {
                    ret = Color.Red;
                }
                else
                {
                    ret = Color.Yellow;
                }
                return ret;
            }
        }

        // Предмет анимирован относительно своей позиции по оси Y.
        private Vector2 basePosition;
        private float bounce;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Выдаёт текущее положение в мировых координатах.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return basePosition + new Vector2(0.0f, bounce);
            }
        }

        /// <summary>
        /// Выдаёт окружность, которая окружает предмет в мировых координатах.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }
        
        /// <summary>
        /// Создаёт новый предмет.
        /// </summary>
        public Item(Level level, Vector2 position)
        {
            ConstructorCommon(level, position);
            LoadContent("Sprites/Item", "Sounds/ItemCollected");
        }
        /// <summary>
        /// Создаёт новый предмет.
        /// </summary>
        public Item(Level level, Vector2 position, string texture_path, string collected_sound_path)
        {
            ConstructorCommon(level, position);
            LoadContent(texture_path, collected_sound_path);
        }

        /// <summary>
        /// Общая инициализация.
        /// </summary>
        private void ConstructorCommon(Level level, Vector2 position)
        {
            this.level = level;
            this.basePosition = position;

            bounce = 0;
            sprite.PlayAnimation(texture);
        }

        /// <summary>
        /// Загружает текстуру и звук.
        /// </summary>
        public void LoadContent( string texture_path, string collected_sound_path)
        {
            // Загружаем анимированную текстуру.
            texture = new Animation(/*Level.*/Content.Load<Texture2D>(texture_path), 0.07f, true);
            //texture = Level.Content.Load<Texture2D>("Sprites/Item");
            origin = new Vector2(texture.FrameWidth / 2.0f, texture.FrameHeight / 2.0f);
            collectedSound = /*Level.*/Content.Load<SoundEffect>(collected_sound_path);
        }

        /// <summary>
        /// Перемещение вверх-вниз.
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // Константы перемещения.
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Движемся по синусу.
            // Используем координату X, чтобы предметы не двигались синхронно, сцена выглядела оживлённой.
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.FrameHeight;

            sprite.PlayAnimation(texture);

            PostUpdate(gameTime);
        }
        public virtual void PostUpdate(GameTime gameTime)
        {
        }

        /// <summary>
        /// Вызывается когда игрок берёт предмет.
        /// </summary>
        /// <param name="collectedBy">
        /// Игрок который поднял предмет. Можно вызывать методы, связанные с
        /// бонусом из поднятых предметов, например неуязвимость и т.д.
        /// </param>
        /// <returns>Выдаёт показатель был ли предмет поднят или нет.</returns>
        public bool OnCollected(Player collectedBy)
        {
            if (WillBeCollected(collectedBy))
            {
                OnCollectedEffect(collectedBy);
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// Проверяется, может ли предмет быть поднятым.
        /// </summary>
        /// <param name="collectedBy">Игрок который поднял предмет.</param>
        /// <returns>Выдаёт показатель будет ли предмет поднят или нет.</returns>
        protected virtual bool WillBeCollected(Player collectedBy)
        {
            return true;
        }
        /// <summary>
        /// Вызывается когда игрок берёт предмет.
        /// </summary>
        /// <param name="collectedBy">
        /// Игрок который поднял предмет. Можно вызывать методы, связанные с
        /// бонусом из поднятых предметов, например неуязвимость и т.д.
        /// </param>
        /// <returns>Выдаёт показатель был ли предмет поднят или нет.</returns>
        protected virtual void OnCollectedEffect(Player collectedBy)
        {
            collectedSound.Play();
            //timeRemaining.Add(new TimeSpan(0,0,Item.PointValue));
            collectedBy.energy.CollectAdditionalBattery();
            Level.Score += Item.PointValue;
            AnimatedEffect.AddEffect(new AnimatedEffect("Effects/ItemCollectedEffect.afx.txt", basePosition + new Vector2(0, +20), SpriteEffects.None));
        }

        /// <summary>
        /// Рисует предмет используя цвет.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
            sprite.Draw(gameTime, spriteBatch, Position, SpriteEffects.None);
        }
    }
    namespace Items
    {
        class Minigun : Item
        {
            public Minigun(Level level, Vector2 position)
                : base(level, position, "Sprites/Items/MinigunItem", "Sprites/Items/MinigunCollectedSound")
            {
            }
            protected override void OnCollectedEffect(Player collectedBy)
            {
                collectedSound.Play();
                collectedBy.weapon.minigun.enabled = true;
                Message.HelpMessage(Message.minigun_collected_message_num);
            }
            protected override bool WillBeCollected(Player collectedBy)
            {
                return !collectedBy.weapon.minigun.enabled;
            }
        }
        class Lasergun : Item
        {
            public Lasergun(Level level, Vector2 position)
                : base(level, position, "Sprites/Items/LasergunItem", "Sprites/Items/MinigunCollectedSound")
            {
            }
            protected override void OnCollectedEffect(Player collectedBy)
            {
                collectedSound.Play();
                collectedBy.weapon.lasergun.enabled = true;
                Message.HelpMessage(Message.lasergun_collected_message_num);
            }
            protected override bool WillBeCollected(Player collectedBy)
            {
                return !collectedBy.weapon.lasergun.enabled;
            }
        }
        /*class Rope : Item
        {
            public Rope(Level level, Vector2 position)
                : base(level, position, "Sprites/Items/RopeItem", "Sprites/Items/MinigunCollectedSound")
            {
            }
            protected override void OnCollectedEffect(Player collectedBy)
            {
                collectedSound.Play();
                collectedBy.rope_state.enabled = true;
                Message.HelpMessage(Message.rope_collected_message_num);
            }
            protected override bool WillBeCollected(Player collectedBy)
            {
                return !collectedBy.rope_state.enabled;
            }
        }*/
        /// <summary>
        /// Предмет - аптечка, вылечивает игрока.
        /// </summary>
        class HealthPack : Item
        {
            public HealthPack(Level level, Vector2 position)
                : base(level, position, "Sprites/Items/HealthPackItem", "Sprites/Items/HealthPackCollectedSound")
            {
            }
            protected override void OnCollectedEffect(Player collectedBy)
            {
                collectedSound.Play();
                collectedBy.AddHP(100);
                if (first_time_collected)
                {
                    first_time_collected = false;
                    Message.HelpMessage(Message.healthpack_collected_message_num);
                }
            }
            float dist_time = 0;
            public override void PostUpdate(GameTime gameTime)
            {
                dist_time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                const float tick = 100 * 0.07f;
                if (dist_time > tick)
                {
                    AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/HealthPackNormalMap.afx.txt", Position, SpriteEffects.None));
                    dist_time -= tick;
                }
            }
            protected override bool WillBeCollected(Player collectedBy)
            {
                return collectedBy.HealthPoints < 100;
            }
            /// <summary>
            /// Подобран ли предмет в первый раз.
            /// </summary>
            static bool first_time_collected = true;
        }
        /// <summary>
        /// Предмет - лёгкая броня, защищает игрока.
        /// </summary>
        class ArmorLight : Item
        {
            public ArmorLight(Level level, Vector2 position)
                : base(level, position, "Sprites/Items/ArmorLightItem", "Sprites/Items/ArmorItemTakeSound")
            {
            }
            protected override void OnCollectedEffect(Player collectedBy)
            {
                collectedSound.Play();
                if (first_time_collected)
                {
                    first_time_collected = false;
                    Message.HelpMessage(Message.armor_collected_message_num);
                }
            }
            protected override bool WillBeCollected(Player collectedBy)
            {
                return collectedBy.SetArmor(XyBorg.PlayerProperties.DamageSystem.armor_light);
            }
            /// <summary>
            /// Подобран ли предмет в первый раз.
            /// </summary>
            static bool first_time_collected = true;
        }
        /// <summary>
        /// Предмет - средняя броня, защищает игрока.
        /// </summary>
        class ArmorMid : Item
        {
            public ArmorMid(Level level, Vector2 position)
                : base(level, position, "Sprites/Items/ArmorMidItem", "Sprites/Items/ArmorItemTakeSound")
            {
            }
            protected override void OnCollectedEffect(Player collectedBy)
            {
                collectedSound.Play();
                if (first_time_collected)
                {
                    first_time_collected = false;
                    Message.HelpMessage(Message.armor_collected_message_num);
                }
            }
            protected override bool WillBeCollected(Player collectedBy)
            {
                return collectedBy.SetArmor(XyBorg.PlayerProperties.DamageSystem.armor_middle);
            }
            /// <summary>
            /// Подобран ли предмет в первый раз.
            /// </summary>
            static bool first_time_collected = true;
        }
        /// <summary>
        /// Предмет - тяжёлая броня, защищает игрока.
        /// </summary>
        class ArmorHeavy : Item
        {
            public ArmorHeavy(Level level, Vector2 position)
                : base(level, position, "Sprites/Items/ArmorHeavyItem", "Sprites/Items/ArmorItemTakeSound")
            {
            }
            protected override void OnCollectedEffect(Player collectedBy)
            {
                collectedSound.Play();
                if (first_time_collected)
                {
                    first_time_collected = false;
                    Message.HelpMessage(Message.armor_collected_message_num);
                }
            }
            protected override bool WillBeCollected(Player collectedBy)
            {
                return collectedBy.SetArmor(XyBorg.PlayerProperties.DamageSystem.armor_heavy);
            }
            /// <summary>
            /// Подобран ли предмет в первый раз.
            /// </summary>
            static bool first_time_collected = true;
        }
        /// <summary>
        /// Предмет - набор гранат.
        /// </summary>
        class Grenades : Item
        {
            public Grenades(Level level, Vector2 position)
                : base(level, position, "Sprites/Player/Grenade", "Sprites/Items/MinigunCollectedSound")
            {
            }
            protected override void OnCollectedEffect(Player collectedBy)
            {
                collectedSound.Play();
                if (first_time_collected)
                {
                    first_time_collected = false;
                    Message.HelpMessage(Message.grenades_collected_message_num);
                }
            }
            protected override bool WillBeCollected(Player collectedBy)
            {
                if (collectedBy.weapon.grenades.grenade_count < 10)
                {
                    collectedBy.weapon.grenades.grenade_count = Math.Min(3 + collectedBy.weapon.grenades.grenade_count, 10);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            /// <summary>
            /// Подобран ли предмет в первый раз.
            /// </summary>
            static bool first_time_collected = true;
        }
        /// <summary>
        /// Ускоряющий бонус.
        /// </summary>
        class SpeedUpBonus : Item
        {
            class SpeedUpEffect : SpellEffect<Player>
            {
                public SpeedUpEffect(ref Player player)
                    : base(ref player, 10)
                {
                }
                int current_gfx_count = 0;
                protected override void affect(float time_since_last_frame, ref Player object_to_affect)
                {

                    int gfx_count = (int)(Math.Max(0, max_fx_durance_time - fx_durance_time) / 0.01);
                    if (current_gfx_count < gfx_count)
                    {
                        current_gfx_count++;

                        AnimatedEffect.AddEffect(new AnimatedEffect("Effects/SpeedupEffect.afx.txt", object_to_affect.Position + new Vector2(RandomValue.get_float(-30, 30), RandomValue.get_float(-30, 30)), SpriteEffects.None));
                    }
                }
                protected override void on_start(ref Player object_to_affect, SpellEffect<Player> this_fx)
                {
                    object_to_affect.MaxMoveSpeed *= 10;
                    object_to_affect.MoveAcceleration += 9000;
                }
                protected override void on_end(ref Player object_to_affect, SpellEffect<Player> this_fx)
                {
                    object_to_affect.MaxMoveSpeed /= 10;
                    object_to_affect.MoveAcceleration -= 9000;
                }
            }
            public SpeedUpBonus(Level level, Vector2 position)
                : base(level, position, "Sprites/Items/SpeedUpBonusItem", "Sprites/Items/SpeedUpBonusCollectedSound")
            {
            }
            protected override void OnCollectedEffect(Player collectedBy)
            {
                collectedSound.Play();
                SpeedUpEffect se = new SpeedUpEffect(ref collectedBy);
            }
        }
        /// <summary>
        /// Замедляющий антибонус.
        /// </summary>
        class SlowDownBonus : Item
        {
            class SlowEffect : SpellEffect<Player>
            {
                public SlowEffect(ref Player player)
                    : base(ref player, 10)
                {
                }
                int current_gfx_count = 0;
                protected override void affect(float time_since_last_frame, ref Player object_to_affect)
                {

                    int gfx_count = (int)(Math.Max(0, max_fx_durance_time - fx_durance_time) / 0.01);
                    if (current_gfx_count < gfx_count)
                    {
                        current_gfx_count++;

                        AnimatedEffect.AddEffect(new AnimatedEffect("Effects/SlowdownEffect.afx.txt", object_to_affect.Position + new Vector2(RandomValue.get_float(-30, 30), RandomValue.get_float(-30, 30)), SpriteEffects.None));
                    }
                }
                protected override void on_start(ref Player object_to_affect, SpellEffect<Player> this_fx)
                {
                    object_to_affect.MaxMoveSpeed *= 0.1f;
                    object_to_affect.MoveAcceleration -= 9000;
                }
                protected override void on_end(ref Player object_to_affect, SpellEffect<Player> this_fx)
                {
                    object_to_affect.MaxMoveSpeed /= 0.1f;
                    object_to_affect.MoveAcceleration += 9000;
                }
            }
            public SlowDownBonus(Level level, Vector2 position)
                : base(level, position, "Sprites/Items/SlowDownBonusItem", "Sprites/Items/SlowDownBonusCollectedSound")
            {
            }
            protected override void OnCollectedEffect(Player collectedBy)
            {
                collectedSound.Play();
                SlowEffect se = new SlowEffect(ref collectedBy);
            }
        }
    }
}
