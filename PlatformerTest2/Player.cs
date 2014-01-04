using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using XyBorg.PlayerProperties;

namespace XyBorg
{
    /// <summary>
    /// Наш герой.
    /// </summary>
    class Player
    {
        // Анимации
        internal Animation idleAnimation;
        private Animation runAnimation;
        private Animation fastRunAnimation;
        private Animation jumpAnimation;
        private Animation byTheWallAnimation;
        private Animation celebrateAnimation;
        private Animation crouchAnimation;
        private Animation dieAnimation;
        private Animation rollAnimation;
        private Animation shootAnimation;
        private Animation onTheRopeAnimation;
        internal SpriteEffects flip = SpriteEffects.None;
        internal AnimationPlayer sprite;

        // Набор звуков
        private SoundEffect killedSound;
        private SoundEffect jumpSound;
        private SoundEffect fallSound;

        private XyBorg.PlayerProperties.DamageSystem health = new XyBorg.PlayerProperties.DamageSystem();
        public float HealthPoints
        {
            get
            {
                return health.health_points;
            }
            private set
            {
            }
        }
        public float ArmorPoints
        {
            get
            {
                return 1.0f/health.armor;
            }
            private set
            {
            }
        }
        public void AddHP(float value)
        {
            health.AddHP(value);
        }
        /// <summary>
        /// Заменяет тип брони, если предлагается лучший коэффициент защиты.
        /// </summary>
        /// <param name="value">Степень повреждений (множитель повреждений).</param>
        /// <returns>Возврыщает показатель - сменена ли защита.</returns>
        public bool SetArmor(float value)
        {
            if (health.armor > value)
            {
                health.armor = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Жив ли персонаж (публичное свойство).
        /// </summary>
        public bool IsAlive
        {
            get { return isAlive; }
        }
        /// <summary>
        /// Жив ли персонаж.
        /// </summary>
        bool isAlive;

        /// <summary>
        /// Залез ли персонаж на лестницу (публичное свойство с изменением скорости при его изменении).
        /// </summary>
        public bool IsUsingStair
        {
            get { return isUsingStair; }
            private set { isUsingStair = value;}
        }
        /// <summary>
        /// Лазил ли персонаж по лестнице только что.
        /// </summary>
        public bool wasUsingStair;
        /// <summary>
        /// Залез ли персонаж на лестницу.
        /// </summary>
        bool isUsingStair;
        /// <summary>
        /// Пересекает ли персонаж на лестницу.
        /// </summary>
        bool isIntersectingStair;
        /// <summary>
        /// Скорость с которой персонаж лезет по лестнице.
        /// </summary>
        const float climbSpeed = 10.1f;

        public int Direction
        {
            get { if (flip == SpriteEffects.None) return -1; else return 1; }
        }
        // Физическое состояние - расположение, скорость движения
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        private float previousBottom;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;
        public void AffectByForce(Vector2 force)
        {
            velocity += force;
        }

        private float lastFrameElapsedTime;
#if ZUNE
        ERROR! - Надо всё перенастроить.
        // Константы, контролирующие передвижение по горизонтали
        private const float MoveAcceleration = 7000.0f;
        private const float MaxMoveSpeed = 1000.0f;
        private const float GroundDragFactor = 0.38f;
        private const float AirDragFactor = 0.48f;
        
        // Константы, контролирующие передвижение по вертикали
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -2000.0f;
        private const float GravityAcceleration = 1700.0f;
        private const float MaxFallSpeed = 450.0f;
        private const float JumpControlPower = 0.13f;

        // Настройки ввода
        private const float MoveStickScale = 0.0f;
        private const Buttons JumpButton = Buttons.B;        
#else
        // Константы, контролирующие передвижение по горизонтали
        public float MoveAcceleration = 20000;//14000.0f;
        public const float AirMoveAccelerationMultiplier = 0.13f;
        public float MaxMoveSpeed = 2000.0f;
        private const float GroundDragFactor = 0.58f;
        private const float AirDragFactor = 0.984f;//0.65f;

        // Константы, контролирующие передвижение по вертикали
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -3000.0f;
        private const float ExtraCrouchJumpLaunchVelocity = JumpLaunchVelocity * 1.75f;
        private const float GravityAcceleration = 3500.0f;
        private const float MaxFallSpeed = 600.0f;
        private const float JumpControlPower = 0.14f;

        // Настройки ввода
        private const float MoveStickScale = 1.0f;
        private const Buttons JumpButton = Buttons.A;
#endif

        /// <summary>
        /// Выдаёт состояние - стоит ли игрок на земле
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
        }
        bool isOnGround;

        /// <summary>
        /// Выдаёт состояние - прыгает ли игрок прижавшись к стене
        /// </summary>
        public bool IsByTheWall
        {
            get { return isByTheWall; }
        }
        bool isByTheWall;
        /// <summary>
        /// Время в течении которого игрок прыгал оттолкнувшись от стены.
        /// </summary>
        float jumpFromTheWallTime = 0.0f;
        /// <summary>
        /// Минимальное время (в секундах) по прошествии которого можно оттолкнуться вновь.
        /// </summary>
        const float jumpFromTheWallReleaseMinTime = 0.4f;
        /// <summary>
        /// Направление отталкивания.
        /// </summary>
        float dodgeDirection = 0.0f;

        /// <summary>
        /// Выдаёт состояние - прыгает ли игрок прижавшись к стене
        /// </summary>
        public bool IsWallAhead
        {
            get { return isWallAhead; }
        }
        bool isWallAhead;

        /// <summary>
        /// Выдаёт состояние - присел ли игрок
        /// </summary>
        public bool IsCrouching
        {
            get { return isCrouching; }
        }
        bool isCrouching;
        /// <summary>
        /// Выдаёт состояние - сидел ли игрок перед прыжком
        /// </summary>
        bool wasCrouching = false;
        /// <summary>
        /// Выдаёт состояние - бежит ли игрок
        /// </summary>
        public bool IsRunning
        {
            get { return isRunning; }
        }
        bool isRunning;

        /// <summary>
        /// Выдаёт состояние - совершает ли игрок подкат
        /// </summary>
        public bool IsRolling
        {
            get { return isRolling; }
        }
        bool isRolling;


        /// <summary>
        /// Текущий ввод движения.
        /// </summary>
        private float movement;

        // Состояние прыжка
        private bool isJumping;
        private bool wasJumping;
        private float jumpTime;

        /// <summary>
        /// Класс по работе с оружием
        /// </summary>
        public class Weapon
        {
            public GrenadeManager grenades;
            public void LoadContent(string playerPath)
            {
                minigun.LoadContent(playerPath);
                lasergun.LoadContent();
                flamethrower.LoadContent();
                grenades = new GrenadeManager();
            }
            public class Lasergun
            {
                public bool enabled = false;

                private SoundEffect shootSound;
                /// <summary>
                /// Выдаёт состояние - стреляет ли игрок
                /// </summary>
                public bool IsShooting
                {
                    get { return isShooting; }
                }
                bool isShooting;
                float shooting_time;
                const float MaxShootingTime = 1.0f;

                internal void update(float lastFrameElapsedTime)
                {
                    shooting_time -= lastFrameElapsedTime;
                    if (shooting_time < 0)
                    {
                        isShooting = false;
                    }
                    if (!initialised)
                        throw new Exception("Лазерган не инициализирован, нельзя начинать стрельбу!");
                }
                internal void Shoot(Vector2 position, int direction, SpriteEffects flip, int mini_blow_size, Player owner)
                {
                    if (!initialised)
                        throw new Exception("Лазерган не инициализирован, нельзя начинать стрельбу!");
                    if (!IsShooting)
                    {
                        shootSound.Play();
                        isShooting = true;
                        shooting_time = MaxShootingTime;
                        for (int i = 0; i < 7; i++)
                        {
                            AnimatedEffect.AddEffect(new AnimatedEffect("Effects/LaserBeam.afx.txt", position + new Vector2(direction * i * i * mini_blow_size / 30 + direction * mini_blow_size / 2, 0), flip));

                            AnimatedEffect.AddEffect(new AnimatedEffect("Effects/ExplosionNormalMap.afx.txt", position + new Vector2(direction * i * i * mini_blow_size / 30 + direction * mini_blow_size / 2, 0), flip));
                        }

                        owner.input_data.set_vibration(0.17f, 0.6f);
                        //Xbox_360_Vibration.AddForce(0.17f, 0.6f);
                    }
                }
                public void LoadContent()
                {
                    shootSound = /*Level.current_level.*/Content.Load<SoundEffect>("Sprites/Player/Weapon/Laser-shoot");
                    initialised = true;
                }

                bool initialised = false;
            }
            public class Flamethrower
            {
                public bool enabled = true;

                //private SoundEffect shootSound;

                internal void Shoot(Vector2 direction, Player owner)
                {
                    if (!initialised)
                        throw new Exception("Огнемёт не инициализирован, нельзя начинать стрельбу!");

                    FlamePS[0].starter.position.X = (int)owner.Position.X + (int)direction.X * 40;
                    FlamePS[0].starter.position.Y = (int)owner.Position.Y - 40;
                    FlamePS[0].starter.start_velocity_angle = CoordConvert.scalar_into_polar(direction);

                    if ((direction.X > 0 && owner.Direction > 0) || (direction.X < 0 && owner.Direction < 0))
                    {
                        FlamePS[0].starter.start_velocity = direction.Length() * 100;
                        FlamePS[0].starter.particles_per_second = direction.LengthSquared() * 100;
                        if (direction.LengthSquared() > 0.95f)
                            //Xbox_360_Vibration.AddForce(0.01f, 0.2f);
                            owner.input_data.set_vibration(0.2f, 0.2f);

                        KillAreaEffect kae = new KillAreaEffect(new Rectangle(
                            (int)(owner.Position.X + direction.X * 40),
                            (int)(owner.Position.Y + direction.Y * 40),
                            (int)(FlamePS[0].starter.start_velocity*0.25),
                            (int)(FlamePS[0].starter.start_velocity*0.25)), 0.05f);
                        KillAreaEffect kae2 = new KillAreaEffect(new Rectangle(
                            (int)(owner.Position.X + direction.X * 200),
                            (int)(owner.Position.Y + direction.Y * 200),
                            (int)(FlamePS[0].starter.start_velocity*0.5),
                            (int)(FlamePS[0].starter.start_velocity*0.5)), 0.05f);
                    }
                    else
                    {
                        FlamePS[0].starter.start_velocity = 0;
                        FlamePS[0].starter.particles_per_second = 0;
                    }
                }
                public void LoadContent()
                {
                    //shootSound = /*Level.current_level.*/Content.Load<SoundEffect>("Sprites/Player/Weapon/Laser-shoot");
                    
                    ParticleSystem.FlamethrowerEmitter flame = new XyBorg.ParticleSystem.FlamethrowerEmitter();
                    FlamePS[0] = new XyBorg.ParticleSystem.ParticleSystem(flame, "Sprites/Effects/Flame.afx.txt");
                    FlamePS[1] = new XyBorg.ParticleSystem.ParticleSystem(flame, "Sprites/Effects/SmokeNormalMap.afx.txt");
                    XyBorg.ParticleSystem.Global.Add(FlamePS[0]);
                    XyBorg.ParticleSystem.Global.Add(FlamePS[1]);


                    initialised = true;
                }
                ParticleSystem.ParticleSystem[] FlamePS = new XyBorg.ParticleSystem.ParticleSystem[2];
                bool initialised = false;
            }
            public class Minigun
            {
                public bool enabled = false;
                public const float rotate_time = 0.4f;
                public const float fire_time = 0.1f;
                public const float drag_force = 0.9f;

                SoundEffect rotate_sound;
                SoundEffect minigun_fire_sound;
                Animation shoot_anim;
                Player player = null;

                bool shooting = false;
                bool initialised = false;

                // Сколько выстрелов было сделано.
                int current_shoot_count = 0;
                float shoot_time = 0;
                public void start_shooting(Player p)
                {
                    if (!enabled)
                        return;
                    if (shooting) return;
                    if (!initialised)
                        throw new Exception("Миниган не инициализирован, нельзя начинать стрельбу!");
                    player = p;
                    shooting = true;
                    current_shoot_count = 0;
                    shoot_time = 0;
                    player.sprite.PlayAnimation(shoot_anim);
                    rotate_sound.Play();
                }
                public void stop_shooting()
                {
                    if (!enabled)
                        return;
                    shooting = false;
                }

                internal void LoadContent(string playerPath)
                {
                    rotate_sound = /*Level.current_level.*/Content.Load<SoundEffect>("Sprites/Player/Weapon/Minigun-rotate");
                    minigun_fire_sound = /*Level.current_level.*/Content.Load<SoundEffect>("Sprites/Player/Weapon/Minigun-fire");
                    shoot_anim = new Animation(/*Level.current_level.*/Content.Load<Texture2D>(playerPath+"/Player-minigun-shooting"), 0.01f, true);

                    initialised = true;
                }
                private const float minigun_shoot_energy = 1.1f;
                public void update(float elapsed_time, Player owner)
                {
                    if (!enabled)
                        return;
                    if (!initialised)
                        throw new Exception("Миниган не инициализирован, нельзя начинать стрельбу!");
                    if (!shooting) return;
                    shoot_time += elapsed_time;

                    int shoot_count = (int)(Math.Max(0, shoot_time - rotate_time) / fire_time);
                    if ((shoot_time > rotate_time) && (current_shoot_count < shoot_count))
                    {
                        if (player.energy.energy < minigun_shoot_energy)
                            shooting = false;
                        // Выстрел происходит здесь
                        player.energy.energy -= minigun_shoot_energy;
                        minigun_fire_sound.Play();
                        current_shoot_count++;
                        player.Position -= new Vector2( player.Direction * drag_force, 0);
                        Vector2 ShootSpot = player.Position + new Vector2(player.Direction * player.idleAnimation.FrameWidth / 30 + player.Direction * player.idleAnimation.FrameWidth / 2, -player.idleAnimation.FrameHeight * 0.25f);
                        AnimatedEffect.AddEffect(new AnimatedEffect("Effects/MinigunProj.afx.txt", ShootSpot, player.flip));
                        AnimatedEffect.AddEffect(new AnimatedEffect("Effects/ExplosionNormalMap.afx.txt", ShootSpot, player.flip));
                        KillAreaEffect kae = new KillAreaEffect(new Rectangle(
                            (int)(ShootSpot.X - shoot_anim.FrameWidth * 3 + player.Direction * shoot_anim.FrameWidth * 1.5f),
                            (int)(ShootSpot.Y - shoot_anim.FrameWidth * 0.2f),
                            (int)(shoot_anim.FrameWidth * 6),
                            (int)(shoot_anim.FrameWidth * 0.4f)), fire_time);

                        owner.input_data.set_vibration(0.2f, 0.2f);
                        //Xbox_360_Vibration.AddForce(0.2f, 0.2f);
                    }
                    player.sprite.PlayAnimation(shoot_anim);
                }
            }
            public Minigun minigun = new Minigun();
            public Lasergun lasergun = new Lasergun();
            public Flamethrower flamethrower = new Flamethrower();
        } public Weapon weapon = new Weapon();
        /// Класс состояния для верёвки.
        public class rope_state_t
        {
            private Texture2D chain_sprite = null;
            public bool using_rope = false;
            private float rope_length = 0;
            private bool button_just_clicked = false;
            public Vector2 velocity = Vector2.Zero;
            public Vector2 position = Vector2.Zero;
            private Vector2 center_position = Vector2.Zero;
            // Звук выстрела гарпуном с верёвкой.
            private SoundEffect rope_shoot_01;
            private SoundEffect rope_shoot_02;
            public void LoadContent() {
                chain_sprite = /*Level.current_level.*/Content.Load<Texture2D>("Sprites/Player/RopeChain");
                rope_shoot_01 = /*Level.current_level.*/Content.Load<SoundEffect>("Sprites/Player/rope_shoot_01");
                rope_shoot_02 = /*Level.current_level.*/Content.Load<SoundEffect>("Sprites/Player/rope_shoot_02");
            }
            private Vector2 proj(Vector2 a, Vector2 b)
            {
                Vector2 ret = new Vector2(0, 0);
                float AB = a.X * b.X + a.Y * b.Y;
                float AA = a.X * a.X + a.Y * a.Y;
                ret.X = AB / AA * a.X;
                ret.Y = AB / AA * a.Y;
                return ret;
            }
            public void update(float elapsed_time) {
                Vector2 d = (position - center_position);
                if (using_rope)
                {
                    float nrange = (position - center_position).Length();
                    position = d / nrange * rope_length + center_position;

                    Vector2 nv = new Vector2(1, -d.X / d.Y);
                    float nvlen = nv.Length();

                    nv = proj(nv, velocity);
                    velocity = nv;
                }
                position += velocity * elapsed_time;

                d = (position - center_position);
                d.Normalize(); d *= rope_length;
                position = d + center_position;

                velocity *= (float)Math.Pow( 0.98f, elapsed_time);
            }
            /// <summary>
            /// При ударении об стену на лету.
            /// </summary>
            public void collide()
            {
                velocity.X = -0.8f*velocity.X;
            }
            /// <summary>
            /// Проверка выстрела гарпуна.
            /// </summary>
            /// <param name="pressed"></param>
            /// <param name="IsOnGround"></param>
            /// <param name="vel"></param>
            /// <param name="pos"></param>
            /// <param name="cursor_position_world_coords"></param>
            public void button_pressed_check(bool pressed, bool IsOnGround, Vector2 vel, Vector2 pos, Vector2 cursor_position_world_coords)
            {
                if (!enabled)
                    return;
                if (pressed)
                {
                    if (!IsOnGround)
                    {
                        if (!button_just_clicked)
                        {
                            button_just_clicked = true;

                            if (Level.current_level.GetCollision(cursor_position_world_coords) == TileCollision.Passable && !using_rope)
                                return;
                            using_rope = !using_rope;
                            if (using_rope)
                            {
                                if(RandomValue.get_float(0, 1)<0.5f)
                                    rope_shoot_01.Play();
                                else
                                    rope_shoot_02.Play();
                                velocity = vel;
                                position = pos;
                                center_position = cursor_position_world_coords/*pos + new Vector2(200 * dir, -200)*/;
                                rope_length = (position - center_position).Length();
                            }
                        }
                    }
                }
                else
                {
                    button_just_clicked = false;
                }
            }
            public void draw_rope(GameTime gameTime, SpriteBatch spriteBatch)
            {
                Vector2 fromCenter = position - center_position;
                fromCenter.Normalize();
                if (using_rope)
                {
                    for (float i = 0; i < rope_length - 5; i += Rope.chain_size)
                    {
                        spriteBatch.Draw(chain_sprite, center_position + fromCenter * i, Color.White);
                    }
                    if (rope_length < Rope.max_length)
                    {
                        for (float i = 0; i < Rope.max_length - rope_length; i += Rope.chain_size)
                        {
                            spriteBatch.Draw(chain_sprite, center_position + fromCenter * rope_length + new Vector2(0,i), Color.White);
                        }
                    }
                }
            }
            public enum set_ropelen_dir {
                up,
                down
            };
            const float rope_length_min = 50;
            const float rope_length_set_amount = 5;
            const float rope_length_max = 700;
            public void set_rope_length(set_ropelen_dir dir) {
                if (dir == set_ropelen_dir.up)
                {
                    rope_length = MathHelper.Clamp(rope_length - rope_length_set_amount, rope_length_min, rope_length_max);
                }
                else
                {
                    rope_length = MathHelper.Clamp(rope_length + rope_length_set_amount, rope_length_min, rope_length_max);
                }
            }
            public void set_speed(float dir)
            {
                velocity.X += dir;
            }
            /// <summary>
            /// Возвращает состояние - находится ли персонаж под верёвкой (иначе над).
            /// </summary>
            /// <returns>Состояние под верёвкой.</returns>
            internal bool under_the_rope()
            {
                return position.Y > center_position.Y;
            }
            public bool enabled = false;
        }; public rope_state_t rope_state = new rope_state_t();


        public struct energy_t {
            private const float maximum_energy = 20.0f;
            private const float energy_per_frame = 0.1f;
            private const float run_energy_per_frame = 0.2f;
            private const float battery_energy_low = 1.7f;
            internal const float shoot_energy = 5.4f;
            //private float energy = maximum_energy;
            public float energy { get; /*private */set; }
            public void CollectAdditionalBattery()
            {
                energy += battery_energy_low;
            }
            public void OnShoot() {
                energy -= shoot_energy;
            }
            public void on_frame( bool isRunning)
            {
                if (energy < maximum_energy)
                    energy += energy_per_frame;
                if (isRunning && energy > 0.0f)
                    energy -= run_energy_per_frame;
            }
            public void DrawEnergyBar(SpriteBatch spriteBatch, Texture2D energyBar, Vector2 pos)
            {
                const int bar_num = 7;
                int curr_bar_num = (int)Math.Floor( energy * bar_num / maximum_energy);
                for (int i = 0; i < curr_bar_num-1; i++, pos.X += energyBar.Width*0.5f)
                    spriteBatch.Draw(energyBar, pos, Color.Green);
                if (curr_bar_num > 0)
                {
                    //pos.X += energyBar.Width;
                    if (energy * bar_num / maximum_energy - curr_bar_num < 0.2)
                        spriteBatch.Draw(energyBar, pos, Color.Red);
                    else if (energy * bar_num / maximum_energy - curr_bar_num < 0.4)
                        spriteBatch.Draw(energyBar, pos, Color.Yellow);
                    else if (energy * bar_num / maximum_energy - curr_bar_num < 0.6)
                        spriteBatch.Draw(energyBar, pos, Color.Orange);
                    else if (energy * bar_num / maximum_energy - curr_bar_num < 0.8)
                        spriteBatch.Draw(energyBar, pos, Color.OrangeRed);
                    else
                        spriteBatch.Draw(energyBar, pos, Color.Green);
                }
            }
        };public energy_t energy = new energy_t();

        private Rectangle localBounds;
        /// <summary>
        /// Выдаёт прямоугольник окружающий героя в мировых координатах.
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

        public enum PlayerType {
            PlayerOne,
            PlayerTwo,
            PlayerThroughtLan
        };
        /// <summary>
        /// Создаём нового игрока.
        /// </summary>
        public Player(Level level, Vector2 position, PlayerType type)
        {
            this.level = level;


            switch (type)
            {
                case PlayerType.PlayerOne:
                    input_data = new FirstPlayerInputData();
                    LoadContent("Mario");
                    break;
                case PlayerType.PlayerTwo:
                    input_data = new SecondPlayerInputData();
                    LoadContent("WhiteSkin");
                    break;
            }

            Reset(position);

            this.level.players.Add(this);
        }

        /// <summary>
        /// Загружаем набор спрайтов и звуки для героя.
        /// </summary>
        public void LoadContent(string playerPath)
        {
            playerPath = "Sprites/Player/PlayerSkin/" + playerPath;
            // Загружаем анимированные текстуры.
            idleAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/Idle"), 0.2f, true);
            runAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/Run"), 0.1f, true);
            fastRunAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/Run"), 0.05f, true);
            jumpAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/Jump"), 0.1f, false);
            byTheWallAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/ByTheWall"), 0.05f, false);
            celebrateAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/Celebrate"), 0.1f, false);
            crouchAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/Crouch"), 0.1f, true);
            dieAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/Die"), 0.1f, false);
            rollAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/Roll"), 0.1f, false);
            shootAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/Shoot"), 0.05f, true);
            onTheRopeAnimation = new Animation(/*Level.*/Content.Load<Texture2D>(playerPath + "/OnTheRope"), 0.05f, true);
            
            
            // Рассчитываем размер исходя из размера текстуры.
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);

            // Загружаем звуки.            
            killedSound = /*Level.*/Content.Load<SoundEffect>("Sounds/PlayerKilled");
            jumpSound = /*Level.*/Content.Load<SoundEffect>("Sounds/PlayerJump");
            fallSound = /*Level.*/Content.Load<SoundEffect>("Sounds/PlayerFall");

            rope_state.LoadContent();
            weapon.LoadContent(playerPath);
        }

        /// <summary>
        /// Возвращаем игрока к жизни.
        /// </summary>
        /// <param name="position">Расположение куда мы возвращаем героя.</param>
        public void Reset(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
            isAlive = true;
            isUsingStair = false;
            wasUsingStair = false;
            sprite.PlayAnimation(idleAnimation);
            health.Reset();
        }

        /// <summary>
        /// Обрабатывает ввод, физику и анимацию перса.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            lastFrameElapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GetInput();

            weapon.grenades.Update(gameTime, input_data.minigun_shooting_button_pressed, this);

            ApplyPhysics(gameTime);


            if (isUsingStair)
            {
            }
            else
            {
                isRolling = false;
                if (IsAlive && IsOnGround)
                {
                    if (IsCrouching)
                    {
                        if (isRunning && movement != 0.0f)
                        {
                            sprite.PlayAnimation(rollAnimation);
                            isRolling = true;
                        }
                        else
                        {
                            sprite.PlayAnimation(crouchAnimation);
                        }
                    }
                    else if (Math.Abs(Velocity.X) - 0.02f > 0)
                    {
                        if (isRunning)
                        {
                            sprite.PlayAnimation(fastRunAnimation);
                        }
                        else
                        {
                            sprite.PlayAnimation(runAnimation);
                        }
                    }
                    else
                    {
                        if (weapon.lasergun.IsShooting)
                        {
                            weapon.lasergun.update(lastFrameElapsedTime);
                            sprite.PlayAnimation(shootAnimation);
                        }
                        else
                        {
                            sprite.PlayAnimation(idleAnimation);
                            weapon.minigun.update(lastFrameElapsedTime, this);
                        }
                    }
                }
                else
                {
                    if (rope_state.using_rope)
                    {
                        sprite.PlayAnimation(onTheRopeAnimation);
                    }
                }

                // Очищаем ввод
                movement = 0.0f;
                isJumping = false;


                // Падение убивает игрока.
                if (BoundingRectangle.Top >= Level.Height * Tile.Height)
                    OnKilled(null);

                // Игрок достиг выхода если он стоит на земле
                // и его ограничивающий прямоугольник содержит точку выхода.
                if (IsAlive &&
                    IsOnGround &&
                    BoundingRectangle.Contains(Level.exit))
                {
                    Level.OnExitReached();
                }
                UpdateItems(gameTime);
            }
        }
        /// <summary>
        /// Анимирует предметы и проверяет сбор игроком.
        /// </summary>
        internal void UpdateItems(GameTime gameTime)
        {
            for (int i = 0; i < Level.items.Count; ++i)
            {
                Item item = Level.items[i];

                item.Update(gameTime);

                if (item.BoundingCircle.Intersects(this.BoundingRectangle))
                {
                    if (Level.OnItemCollected(item, this))
                        Level.items.RemoveAt(i--);
                }
            }
        }
        /// <summary>
        /// Проверяет цепляние за верёвки игроком.
        /// </summary>
        internal void UpdateRopes(GameTime gameTime)
        {
            for (int i = 0; i < Level.ropes.Count; ++i)
            {
                Rope rope = Level.ropes[i];

                if (rope.using_player == null)
                {
                    if (rope.intersection_rect.Intersects(this.BoundingRectangle))
                    {
                        if (rope_state.using_rope != true)
                        {
                            rope_state.enabled = true;
                            //rope_state.using_rope = true;
                            rope_state.button_pressed_check(true, IsOnGround, velocity, position, rope.start_position);
                            rope.OnCollision(this);
                        }
                    }
                }
                else if (rope.using_player.rope_state.using_rope == false)
                {
                    rope.OnCollision(null);
                }
            }
        }

        #region Input
        #region InputData
        abstract class BaseInputData
        {
            public float movement { get; protected set; }
            public bool rope_button_pressed { get; protected set; }
            public bool minigun_shooting_button_pressed { get; protected set; }
            public bool lasergun_shooting_button_pressed { get; protected set; }
            public bool up_button_pressed { get; protected set; }
            public bool down_button_pressed { get; protected set; }
            public bool left_button_pressed { get; protected set; }
            public bool right_button_pressed { get; protected set; }
            public bool jump_button_pressed { get; protected set; }
            public bool crouch_button_pressed { get; protected set; }
            public bool run_button_pressed { get; protected set; }
            public Vector2 mouse_position { get; protected set; }
            public Vector2 right_thumbstick { get; protected set; }
            public abstract void retrieve();
            public abstract void set_vibration(float m1, float m2);
        }

        class FirstPlayerInputData : BaseInputData
        {
            public override void set_vibration(float m1, float m2) { }
            public override void retrieve()
            {
                // Получаем состояние.
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
                KeyboardState keyboardState = Keyboard.GetState();
                MouseState mouseState = Mouse.GetState();

                mouse_position = new Vector2(mouseState.X, mouseState.Y);

                // По горизонтали.
                movement = gamePadState.ThumbSticks.Left.X * MoveStickScale;

                // Повисание на верёвке.
                rope_button_pressed = keyboardState.IsKeyDown(Keys.Space) || gamePadState.IsButtonDown(Buttons.Y);

                // Стрельба.
                minigun_shooting_button_pressed = keyboardState.IsKeyDown(Keys.Q) || gamePadState.IsButtonDown(Buttons.X);
                lasergun_shooting_button_pressed = keyboardState.IsKeyDown(Keys.E) || gamePadState.IsButtonDown(Buttons.Y);

                // Проверяем движение.
                up_button_pressed = keyboardState.IsKeyDown(Keys.W) || (gamePadState.ThumbSticks.Left.Y > 0) || gamePadState.IsButtonDown(Buttons.DPadUp);
                down_button_pressed = keyboardState.IsKeyDown(Keys.S) || (gamePadState.ThumbSticks.Left.Y < 0) || gamePadState.IsButtonDown(Buttons.DPadDown);
                left_button_pressed = gamePadState.IsButtonDown(Buttons.DPadLeft) || keyboardState.IsKeyDown(Keys.A) || (gamePadState.ThumbSticks.Left.X < 0);
                right_button_pressed = gamePadState.IsButtonDown(Buttons.DPadRight) || keyboardState.IsKeyDown(Keys.D) || (gamePadState.ThumbSticks.Left.X > 0);

               


                // Проверяем прыжок.
                jump_button_pressed =
                    gamePadState.IsButtonDown(JumpButton) ||
                    keyboardState.IsKeyDown(Keys.Space);

                // Проверяем приседание.
                crouch_button_pressed =
                        gamePadState.IsButtonDown(Buttons.DPadDown) ||
                        keyboardState.IsKeyDown(Keys.S) ||
                        (gamePadState.ThumbSticks.Left.Y < 0);

                // Проверяем бег.
                run_button_pressed = keyboardState.IsKeyDown(Keys.LeftShift) || gamePadState.IsButtonDown(Buttons.RightTrigger);

                // Проверяем правый миниджойстик (для огнемёта).
                right_thumbstick = gamePadState.ThumbSticks.Right;
            }
        }

        class SecondPlayerInputData : BaseInputData
        {
            public override void set_vibration(float m1, float m2)
            {
                Xbox_360_Vibration.AddForce(m1, m2);
            }
            public override void retrieve()
            {
                // Получаем состояние.
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.Two);
                KeyboardState keyboardState = Keyboard.GetState();
                MouseState mouseState = Mouse.GetState();

                mouse_position = new Vector2(mouseState.X, mouseState.Y);

                // По горизонтали.
                movement = gamePadState.ThumbSticks.Left.X * MoveStickScale;

                // Повисание на верёвке.
                rope_button_pressed = keyboardState.IsKeyDown(Keys.NumPad1) || mouseState.LeftButton == ButtonState.Pressed;

                // Стрельба.
                minigun_shooting_button_pressed = mouseState.LeftButton == ButtonState.Pressed;
                lasergun_shooting_button_pressed = keyboardState.IsKeyDown(Keys.NumPad1);

                // Проверяем движение.
                up_button_pressed = keyboardState.IsKeyDown(Keys.Up);
                down_button_pressed = keyboardState.IsKeyDown(Keys.Down);
                left_button_pressed = keyboardState.IsKeyDown(Keys.Left);
                right_button_pressed = keyboardState.IsKeyDown(Keys.Right);

                // Проверяем прыжок.
                jump_button_pressed =
                    keyboardState.IsKeyDown(Keys.RightControl);

                // Проверяем приседание.
                crouch_button_pressed =
                        keyboardState.IsKeyDown(Keys.Down);

                // Проверяем бег.
                run_button_pressed = keyboardState.IsKeyDown(Keys.RightShift);

                // Проверяем правый миниджойстик (для огнемёта).
                right_thumbstick = gamePadState.ThumbSticks.Right;
            }
        }
        BaseInputData input_data = null;
        
        #endregion

        /// <summary>
        /// Получаем ввод.
        /// </summary>
        private void GetInput()
        {
            // Получаем состояние.
            input_data.retrieve();
            /*GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();*/
            weapon.flamethrower.Shoot(input_data.right_thumbstick, this);

            // По горизонтали.
            movement = input_data.movement;

            // Игнорируем медленное движение, чтобы не бегать на месте.
            if (Math.Abs(movement) < 0.5f)
                movement = 0.0f;

            // Повисание на верёвке.
            if (!IsOnGround)
            {
                Vector2 cursor_position_world_coords = CoordConvert.screen_into_world(input_data.mouse_position);
                rope_state.button_pressed_check(input_data.rope_button_pressed, IsOnGround, velocity, position, cursor_position_world_coords);
            }
            else if (!IsCrouching)
            {
                if (input_data.minigun_shooting_button_pressed)
                {
                    weapon.minigun.start_shooting(this);
                }
                else
                {
                    weapon.minigun.stop_shooting();
                }
            }
            // Стрельба.
            if (weapon.lasergun.enabled)
            {
                if (/*keyboardState.IsKeyDown(Keys.E)*/input_data.lasergun_shooting_button_pressed)
                {
                    if (IsOnGround && !IsCrouching && energy.energy > energy_t.shoot_energy)
                    {
                        weapon.lasergun.Shoot(Position + new Vector2(0,-idleAnimation.FrameHeight*0.25f), Direction, flip, idleAnimation.FrameWidth, this);
                        energy.OnShoot();
                    }
                }
                if (weapon.lasergun.IsShooting)
                {
                    return;
                }
            }
            if (rope_state.using_rope)
            {
                if (input_data.up_button_pressed)
                {
                    rope_state.set_rope_length(rope_state_t.set_ropelen_dir.up);
                }
                if (input_data.down_button_pressed)
                {
                    rope_state.set_rope_length(rope_state_t.set_ropelen_dir.down);
                }
                if (input_data.left_button_pressed)
                {
                    rope_state.set_speed(-3);
                }
                if (input_data.right_button_pressed)
                {
                    rope_state.set_speed(3);
                }
                return;
            }
            else
            {
                if (isUsingStair)
                {
                    if (input_data.jump_button_pressed)
                    {
                        wasUsingStair = isUsingStair;
                        isUsingStair = false;
                    }
                    else if (input_data.up_button_pressed)
                    {
                        position.Y -= climbSpeed;
                    }
                    else if (input_data.down_button_pressed)
                    {
                        position.Y += climbSpeed;
                    }
                }
                else
                {
                    wasUsingStair = false;
                    if(isIntersectingStair) {
                        if (input_data.up_button_pressed || input_data.down_button_pressed)
                        {
                            wasUsingStair = isUsingStair;
                            isUsingStair = true;
                        }
                    }
                }
            }

            // Проверяем движение.
            if (input_data.left_button_pressed)
            {
                if (!isByTheWall || dodgeDirection == 0)
                {
                    movement = -1.0f;
                }
                else
                {
                    movement = dodgeDirection;
                }
            }
            else if (input_data.right_button_pressed)
            {
                if (!isByTheWall || dodgeDirection == 0)
                {
                    movement = 1.0f;
                }
                else
                {
                    movement = dodgeDirection;
                }
            }

            if (isRunning)
            {
                movement *= 2;
            }
            else if (isCrouching)
                movement = 0;


            // Проверяем прыжок.
            isJumping = input_data.jump_button_pressed;

            if (isJumping)
            {
                if (IsCrouching)
                    wasCrouching = true;

                if (movement != 0.0 && isWallAhead)
                {
                    if (isByTheWall && jumpFromTheWallTime > jumpFromTheWallReleaseMinTime && movement != dodgeDirection)
                    {
                        isRunning = false;
                        velocity.Y += JumpLaunchVelocity;
                        jumpFromTheWallTime = 0.0f;
                        jumpTime = 0.0f;
                        movement = -movement;
                        dodgeDirection = movement;
                        ContinueJump();
                    }
                    else
                    {
                        isByTheWall = true;
                    }
                }
            }
            else //if (!isJumping)
            {
                isCrouching = input_data.crouch_button_pressed;
				if (wasCrouching && !isCrouching)
                    wasCrouching = false;

            	// Проверяем бег.
            	isRunning = input_data.run_button_pressed;
            }
            energy.on_frame(isRunning && (movement != 0.0f));
            if (energy.energy < 2.0f)
            {
                isRunning = false;
            }
        }
        #endregion
        /// <summary>
        /// Обновляем скорость и положение игрока,
        /// в зависимости от ввода, гравитации и т.д.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (rope_state.using_rope)
            {
                rope_state.velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                rope_state.update(elapsed);
                position = rope_state.position;
                velocity = rope_state.velocity;
                HandleCollisions();
            }
            else
            {
                Vector2 previousPosition = Position;

                // Скорость зависит от ввода движения по горизонтали и гравитации при падении.
                //velocity.X += movement * MoveAcceleration * elapsed;
				// Если игрок летит движение на него влияет меньше.
                if (!IsOnGround)
                {
                    velocity.X += movement * MoveAcceleration * AirMoveAccelerationMultiplier * elapsed;
                }
                else
                {
                    velocity.X += movement * MoveAcceleration * elapsed;
                }
                // Если игрок карабкается по лестнице, гравитация на него не влияет.
                if (!isUsingStair)
                {
                    velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                }
                velocity.Y = DoJump(velocity.Y, gameTime);

                // Применяем трение.
                if (IsOnGround)
                    velocity.X *= GroundDragFactor;
                else
                    velocity.X *= AirDragFactor;

                // Не даём игроку двигаться быстрее его максимума.
                velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

                // Применяем скорость.
                Position += velocity * elapsed;
                Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

                // Если игрок пересекается с уровнем отделяем.
                HandleCollisions();

                // Если столкновение нас остановило обнуляем скорость.
                if (Position.X == previousPosition.X)
                    velocity.X = 0;

                if (Position.Y == previousPosition.Y)
                    velocity.Y = 0;
            }
        }

        private void ContinueJump()
        {
            if (jumpTime == 0.0f)
                jumpSound.Play();

            //jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            jumpTime += lastFrameElapsedTime;

            if(isWallAhead)
                sprite.PlayAnimation(byTheWallAnimation);
            else
                sprite.PlayAnimation(jumpAnimation);
            if (IsByTheWall) {
                jumpFromTheWallTime += lastFrameElapsedTime;
            }
        }
        /// <summary>
        /// Рассчитывает скорость падения по Y во время прыжка,
        /// вызывает соответствующюю анимацию.
        /// </summary>
        /// <param name="velocityY">
        /// Текущая скорость игрока по оси Y.
        /// </param>
        /// <param name="gameTime">
        /// Данные о текущем времени.
        /// </param>
        /// <returns>
        /// Новая скорость по оси Y.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // Если игрок собрался прыгать
            if (isJumping)
            {
                // Начинаем или продолжаем прыжок
                if ((!wasJumping && (IsOnGround || IsUsingStair || wasUsingStair)) || jumpTime > 0.0f)
                {
                    ContinueJump();
                }

                // Прыгаем
                if (0.0f < jumpTime && jumpTime <= MaxJumpTime)
                {
                    // Меняем скорость
                    if (wasCrouching)
                        velocityY = ExtraCrouchJumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                    else
                        velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Допрыгались
                    jumpTime = 0.0f;
                    isByTheWall = false;
                    dodgeDirection = 0.0f;
                }
            }
            else
            {
                // Допрыгались
                jumpTime = 0.0f;
                isByTheWall = false;
                dodgeDirection = 0.0f;
            }
            wasJumping = isJumping;

            return velocityY;
        }

        /// <summary>
        /// Определяет и обрабатывает столкновения,
        /// если обнаружено таковое игрок возвращается обратно.
        /// Платформы обрабатываются специальным образом.
        /// </summary>
        private void HandleCollisions()
        {
            // Получаем прямоугольник окружающий игрока и соответствующие тайлы.
            Rectangle bounds = BoundingRectangle;
            if (IsRolling)
            {
                int h = bounds.Height / 2;
                bounds.Y += h;
                bounds.Height -= h;
            }

            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;
            int hMidTile = (topTile+bottomTile)/2;

            // Обнуляем для проверки столкновения с землёй.
            isOnGround = false;
            // Обнуляем для проверки столкновения со стеной.
            isWallAhead = false;

            // Обнуляем для проверки пересечения с лестницей.
            isIntersectingStair = false;
            bool wasUsingStair = isUsingStair;
            isUsingStair = false;

            // Для каждого потенциално пересекающего тайла
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // Смотрим проходимость тайла.
                    TileCollision collision = Level.GetCollision(x, y);

                    if (collision != TileCollision.Passable && collision != TileCollision.Stair)
                    {
                        // Определяем глубину проникновения.
                        Rectangle tileBounds = Level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Разрешаем столкновение.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // Если пересекли верх тайла - мы не на земле.
                                if (previousBottom <= tileBounds.Top)
                                {
                                    isOnGround = true;
                                    rope_state.using_rope = false;
                                }

                                // Игнорируем платформу пока не окажется, что мы на земле.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    rope_state.using_rope = false;
                                    // Разрешаем по Y.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Продолжаем.
                                    bounds = BoundingRectangle;
                                }
                                // Если убивающая платформа - отнимем жизни.
                                if (collision == TileCollision.DeadEnd)
                                {
                                    if (!health.dead)
                                    {
                                        health.Damage(5);
                                        if (health.dead)
                                            OnKilled(null);
                                    }
                                }
                            }
                            else if (collision == TileCollision.Impassable)
                            {
                                if (rope_state.under_the_rope())
                                    rope_state.collide();
                                else
                                    rope_state.using_rope = false;
                                if ((movement > 0 && (x == rightTile)) || (movement < 0 && (x == leftTile)))
                                    if (y == hMidTile)
                                        isWallAhead = true;

                                // Разрешаем по X.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Продолжаем.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                    else if (collision == TileCollision.Stair)
                    {
                        isIntersectingStair = true;
                        isUsingStair = wasUsingStair;
                        if (isUsingStair)
                            velocity = Vector2.Zero;
                    }
                }
            }

            // Сохраняем новый низ.
            previousBottom = bounds.Bottom;
        }

        /// <summary>
        /// Вызываем когда игрок убит.
        /// </summary>
        /// <param name="killedBy">
        /// Враг убивший игрока. Равно null если игрока не убивали,
        /// например он свалился в дыру.
        /// </param>
        public void OnKilled(Enemy killedBy)
        {
            rope_state.using_rope = false;
            isAlive = false;

            if (killedBy != null)
                killedSound.Play();
            else
                fallSound.Play();

            sprite.PlayAnimation(dieAnimation);
        }

        /// <summary>
        /// Вызываем если игрок достиг выхода.
        /// </summary>
        public void OnReachedExit()
        {
            sprite.PlayAnimation(celebrateAnimation);
        }

        /// <summary>
        /// Отрисовываем игрока.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            // Поворачиваем в нужном направлении.
            if (Velocity.X > 0)
                flip = SpriteEffects.FlipHorizontally;
            else if (Velocity.X < 0)
                flip = SpriteEffects.None;

            // Отрисовываем верёвку.
            rope_state.draw_rope(gameTime, spriteBatch);
            // Отрисовываем спрайт персонажа.
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }
        internal void OnDamaged(Enemy enemy, float attack)
        {
            health.Damage(attack);
            if (HealthPoints <= 0 && isAlive)
                OnKilled(enemy);
        }
    }
}
