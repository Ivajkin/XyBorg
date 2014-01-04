using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace XyBorg
{
    /// <summary>
    /// Однородная сетка тайлов с колекцией предметов и врагов.
    /// Игрок - часть уровня, который контролирует условия победы и поражения,
    /// а также прокрутку.
    /// </summary>
    class Level : IDisposable
    {
        public static Level current_level = null;
        // Физическая структура уровня.
        private Tile[,] tiles;
        private Layer[] layers;
        // Слой над которым отрисовываются объекты.
        private const int EntityLayer = 4;

        private XyBorg.Enemies.TurretManager turretManager;

        internal List<Player> players = new List<Player>();
        // Объекты на уровне.
        public class PlayerProxy
        {
            public bool IsAlive { get { return Level.current_level.Player1.IsAlive && Level.current_level.Player2.IsAlive; } }
            public Player.energy_t energy { get { return Level.current_level.Player1.energy; } }

            internal void ApplyPhysics(GameTime gameTime)
            {
                Level.current_level.Player1.ApplyPhysics(gameTime);
                Level.current_level.Player2.ApplyPhysics(gameTime);
            }

            internal void Update(GameTime gameTime)
            {
                Level.current_level.Player1.Update(gameTime);
                Level.current_level.Player2.Update(gameTime);

            }

            internal void UpdateItems(GameTime gameTime)
            {
                Level.current_level.Player1.UpdateItems(gameTime);
                Level.current_level.Player2.UpdateItems(gameTime);
            }

            internal void UpdateRopes(GameTime gameTime)
            {
                Level.current_level.Player1.UpdateRopes(gameTime);
                Level.current_level.Player2.UpdateRopes(gameTime);
            }

            internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
            {
                Level.current_level.Player1.Draw(gameTime, spriteBatch);
                Level.current_level.Player2.Draw(gameTime, spriteBatch);
            }

            internal void set_vibration(float mf, float d)
            {
                Xbox_360_Vibration.AddForce(mf, d);
            }
        }
        public PlayerProxy Players = null;
        /*{
            get { return player1; }
        }*/
        public Player Player1
        {
            get { return player1; }
        }
        public Player Player2
        {
            get { return player2; }
        }
        Player player1;
        Player player2;

        /// <summary>
        /// Список предметов.
        /// </summary>
        internal List<Item> items = new List<Item>();
        /// <summary>
        /// Список верёвок, за которые можно зацепиться.
        /// </summary>
        internal List<Rope> ropes = new List<Rope>();
        /// <summary>
        /// Временный список для врагов.
        /// </summary>
        private List<Enemy> enemies = null;
        /// <summary>
        /// Массив списков врагов - уровень делится на части, враги отрисовываются только в нужной части.
        /// </summary>
        private List<Enemy>[,] enemies_grid;
        private void AddEnemy(Enemy enemy)
        {
            int x = (int)Math.Max(1,enemy.Position.X / (Tile.Width * 15));
            int y = (int)Math.Max(1,enemy.Position.Y / (Tile.Height * 15));

            enemies_grid[x, y].Add(enemy);

            enemies_grid[x - 1, y].Add(enemy);
            enemies_grid[x + 1, y].Add(enemy);
            enemies_grid[x, y - 1].Add(enemy);
            enemies_grid[x, y + 1].Add(enemy);

            enemies_grid[x - 1, y - 1].Add(enemy);
            enemies_grid[x + 1, y - 1].Add(enemy);
            enemies_grid[x + 1, y - 1].Add(enemy);
            enemies_grid[x + 1, y + 1].Add(enemy);
        }
        /// <summary>
        /// Выдаёт индекс списка врагов, которых нужно отрисовывать или обновлять, зависит от положения камеры.
        /// </summary>
        private int GetEnemySetIndexX()
        {
            return (int)Math.Max(0, CameraPosition.X / (Tile.Width * 15));
        }
        /// <summary>
        /// Выдаёт индекс списка врагов, которых нужно отрисовывать или обновлять, зависит от положения камеры.
        /// </summary>
        private int GetEnemySetIndexY()
        {
            return (int)Math.Max(0, -CameraPosition.Y / (Tile.Height * 15));
        }
        // Ключевые локации на уровне - вход и выход.
        private Vector2 start;
        internal Point exit = InvalidPosition;
        private static readonly Point InvalidPosition = new Point(-1, -1);

        // Состояние игры.
        private Random random = new Random(32125); // Статическая случайность (pff, amateurs, proffesionals using the named constants))

        private Vector2 cameraPosition;
        public Vector2 CameraPosition
        {
            get { return cameraPosition; }
        }

        public int Score
        {
            get { return score; }
            set { score = value; }
        }
        int score;

        public bool ReachedExit
        {
            get { return reachedExit; }
        }
        bool reachedExit;

        public TimeSpan TimeRemaining
        {
            get { return timeRemaining; }
        }
        TimeSpan timeRemaining;


        public int PlayerDiedTimes
        {
            get { return playerDiedTimes; }
        }
        private int playerDiedTimes = 0;

        private const int PointsPerSecond = 5;

        // Содержимое уровня (ресурсы)        
        /*public ContentManager Content
        {
            get { return content; }
        }*/
        ContentManager content;

        private SoundEffect exitReachedSound;

        #region Loading

        /// <summary>
        /// Создание нового уровня.
        /// </summary>
        /// <param name="serviceProvider">
        /// IServiceProvider для конструирования нового ContentManager.
        /// </param>
        /// <param name="stage">
        /// Текущий уровень.
        /// </param>
        public Level(IServiceProvider serviceProvider, int stage)
        {

            current_level = this;

            string path = null;
            // Находим путь.
            {
                // Пробуем найти следующий уровень ( это текстовые файлы 0.txt, 1.txt, 2.txt ... )
                path = String.Format("Levels/Level{0}/level.data.txt", stage);
                path = Path.Combine(
                    Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation,
                    "Content/" + path);
                if (!File.Exists(path))
                {
                    stage = 0;
                    path = String.Format("Levels/Level{0}/level.data.txt", stage);
                    path = Path.Combine(
                        Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation,
                        "Content/" + path);
                }
            }
            if (path == null)
                throw new Exception("Не найден файл уровня.");
            
            // Создаём новый менеджер контекста для загрузки контента, необходимого только для этого уровня.
            content = new ContentManager(serviceProvider, "Content");

            turretManager = new XyBorg.Enemies.TurretManager();

            timeRemaining = TimeSpan.FromMinutes(9.0);

            LoadTiles(path);

            // Загружаем слои заднего фона.
            layers = new Layer[5];
            layers[0] = new Layer(content, stage, 0, 0.1f, this.Height);
            layers[1] = new Layer(content, stage, 1, 0.11f, this.Height); layers[1].setSelfScrollSpeed(new Vector2(27.21f, 10.1f));
            layers[2] = new Layer(content, stage, 2, 0.12f, this.Height); layers[2].setSelfScrollSpeed(new Vector2(-31.31f, 10.1f));
            layers[3] = new Layer(content, stage, 3, 0.6f, this.Height);
            layers[4] = new Layer(content, stage, 4, 0.8f, this.Height);
            //layers[5] = new Layer(Content, stage, 5, 1.001f, this.Height);

            // Загружаем звуки.
            exitReachedSound = Content.Load<SoundEffect>("Sounds/ExitReached");

            path = String.Format("Levels/Level{0}/animated_effects.txt", stage);
            path = Path.Combine(
                Microsoft.Xna.Framework.Storage.StorageContainer.TitleLocation,
                "Content/" + path);
            AnimatedEffect.LoadSceneEffects(path);

            Message.HelpMessage(0);

            XyBorg.ParticleSystem.Global.Initialise();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Content.Load<Song>("Sounds/Music01"));
            
        }

        /// <summary>
        /// Проходим по каждому тайлу в файле и загружаем его внешний вид и поведение.
        /// Также проверяется существование важных объектов - входов, выходов и т.д.
        /// </summary>
        /// <param name="path">
        /// Абсолютный путь к файлу карты.
        /// </param>
        private void LoadTiles(string path)
        {
            // Загружаем файл и проверяем все ли строки одинаковы.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("Протяжённость строки {0} отличается от протяжённости остальных строк.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Выделяем память на сетку тайлов.
            tiles = new Tile[width, lines.Count];
            // Выделяем память на сетку врагов.
            enemies_grid = new List<Enemy>[width / 14 + 2, lines.Count / 14 + 2];
            for (int i = 0; i < width / 14 + 2; i++)
            {
                for (int j = 0; j < lines.Count / 14 + 2; j++)
                {
                    enemies_grid[i, j] = new List<Enemy>();

                }
            }

            // Проходим по каждому положению,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // чтобы загрузить тайл.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                    // Тест травы и другой растительности, декораций
                    if(false)
                    {
                        if (tiles[x, y].Collision == TileCollision.Impassable && tiles[x, (y - 1 > 0 ? y - 1 : y)].Collision == TileCollision.Passable)
                        {
                            {
                                float rand = RandomValue.get_int(100, 400);
                                if (rand == 141 || rand == 157 || rand == 111)
                                    AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/StreetLight.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }

                            {
                                float rand = RandomValue.get_int(100, 300);
                                if (rand == 134 || rand == 191 || rand == 150)
                                    AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Palm1-1.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            {
                                float rand = RandomValue.get_int(100, 300);
                                if (rand == 134 || rand == 191 || rand == 150)
                                    AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Palm1-2.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            {
                                float rand = RandomValue.get_int(100, 300);
                                if (rand == 134 || rand == 191 || rand == 150)
                                    AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Palm1-3.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            {
                                float rand = RandomValue.get_int(100, 300);
                                if (rand == 134 || rand == 191 || rand == 150)
                                    AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Palm2-1.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            {
                                float rand = RandomValue.get_int(100, 300);
                                if (rand == 134 || rand == 191 || rand == 150)
                                    AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Palm2-2.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }

                            if (RandomValue.get_int(10, 30) == 25)
                            {
                                AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Brush-1.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            if (RandomValue.get_int(10, 30) == 26)
                            {
                                AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Brush-2.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            if (RandomValue.get_int(10, 30) == 27)
                            {
                                AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Brush-3.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            if (RandomValue.get_int(25, 30) == 27)
                            {
                                AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Grass-1.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            if (RandomValue.get_int(25, 30) == 27)
                            {
                                AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Grass-2.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            if (RandomValue.get_int(25, 30) == 27)
                            {
                                AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Grass-3.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            {
                                float rand = RandomValue.get_int(100, 350);
                                if (rand == 141 || rand == 157 || rand == 111)
                                    AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/GlowingArtefact.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            if (RandomValue.get_int(0, 50) == 27)
                            {
                                AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/Fireflies.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                            if (RandomValue.get_int(0, 100) == 27)
                            {
                                AnimatedEffect.AddEffect(new AnimatedEffect("Sprites/Effects/FireflyFlower.afx.txt", new Vector2(x * Tile.Width + RandomValue.get_int(0, Tile.Width), y * Tile.Height), SpriteEffects.None));
                            }
                        }
                    }
                }
            }

            // Проверяем существование входа и выхода.
            if (Players == null)
                throw new NotSupportedException("На уровне должна быть начальная позиция.");
            if (exit == InvalidPosition)
                throw new NotSupportedException("На уровне должен быть выход.");

        }

        /// <summary>
        /// Загружаем внешний вид тайла и его поведение.
        /// </summary>
        /// <param name="tileType">
        /// Символ из файла карты, обозначающий тип тайла.
        /// </param>
        /// <param name="x">
        /// Положение среди тайлов по оси X.
        /// </param>
        /// <param name="y">
        /// Положение среди тайлов по оси Y.
        /// </param>
        /// <returns>Загруженый тайл.</returns>
        private Tile LoadTile(char tileType, int x, int y)
        {
            if (tileType >= 'A' && tileType < 'F')
            {
                // Разные враги
                return LoadEnemyTile(x, y, "Enemies/Monster"+tileType);
            }

            switch (tileType)
            {
                // Пусто
                case '.':
                    return new Tile(null, TileCollision.Passable);

                // Выход
                case 'X':
                    return LoadExitTile(x, y);

                // Предмет
                case 'I':
                    return LoadItemTile(x, y,Item.item_type.item);
                // Предмет - миниган
                case 'M':
                    return LoadItemTile(x, y, Item.item_type.minigun);
                // Предмет - лазерган
                case 'L':
                    return LoadItemTile(x, y, Item.item_type.lasergun);
                // Предмет - верёвка
                //case 'R':
                //    return LoadItemTile(x, y, Item.item_type.rope);
                // Верёвка висящая.
                case 'r':
                    return LoadRopeTile(x, y);
                // Предмет - болеутоляющее
                case 'H':
                    return LoadItemTile(x, y, Item.item_type.health_pack);
                // Предмет - лёгкая броня
                case 'a':
                    return LoadItemTile(x, y, Item.item_type.armor_light);
                // Предмет - средняя броня
                case 'm':
                    return LoadItemTile(x, y, Item.item_type.armor_middle);
                // Предмет - тяжёлая броня
                case 'h':
                    return LoadItemTile(x, y, Item.item_type.armor_heavy);
                // Предмет - связка гранат
                case 'g':
                    return LoadItemTile(x, y, Item.item_type.grenades_pack);

                // Предмет - верёвка
                case 'j':
                    return LoadJumperTile(x, y);

                // Предмет - ускоряющий бонус
                case 'S':
                    return LoadItemTile(x, y, Item.item_type.speed_up_bonus);
                // Предмет - замедляющий бонус
                case 's':
                    return LoadItemTile(x, y, Item.item_type.slow_down_bonus);

                case 'f':
                    //Content.Load<ParticleSystemPiplineLib.ParticleSystemImportStruct>("Sprites/Effects/ParticleSystems/FirePit.xml");
                    ParticleSystem.GenericEmitter starter1 = new ParticleSystem.FirePitEmitter1();
                    ParticleSystem.FirePitEmitter2Distortion starter2 = new ParticleSystem.FirePitEmitter2Distortion();
                    starter1.position = GetBounds(x, y); starter1.position.Y += starter1.position.Height / 2 + starter1.position.Height / 4; starter1.position.Height /= 2;
                    starter2.position = starter1.position;
                    ParticleSystem.Global.Add(new ParticleSystem.ParticleSystem(starter1, "Sprites/Effects/Smoke.afx.txt"));
                    ParticleSystem.Global.Add(new ParticleSystem.ParticleSystem(starter2, "Sprites/Effects/SmokeNormalMap.afx.txt"));
                    return LoadVarietyTile("Pit", 2, TileCollision.DeadEnd);

                // Платформа в воздухе
                case '-':
                    return LoadVarietyTile("Platform", 5, TileCollision.Platform);

                // Лестница.
                case 'u':
                    return LoadVarietyTile("Stair", 2, TileCollision.Stair);

                // Проходимый блок
                case ':':
                    return LoadVarietyTile("BlockB", 5, TileCollision.Passable);

                // Начальная позиция для игрока 1
                case '1':
                    return LoadStartTile(x, y);

                // Непроходимый блок
                case '#':
                    return LoadVarietyTile("BlockA", 10, TileCollision.Impassable);

                // Подсказка, беги вправо
                case '>':
                    return LoadTile("HelpRunRight", TileCollision.Passable);


                // Туррель - вращается и стреляет в игрока.
                case 't':
                    return LoadTurretTile(x, y);

                // Неизвестный символ!
                default:
                    throw new NotSupportedException(String.Format("Непонятный символ '{0}'! Координаты: {1}, {2}.", tileType, x, y));
            }
        }

        /// <summary>
        /// Создаём тайл с верёвкой на позиции x,y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>Созданный тайл.</returns>
        private Tile LoadRopeTile(int x, int y)
        {
            Point pt = GetBounds(x, y).Center;
            Vector2 pos = new Vector2(pt.X, pt.Y);
            ropes.Add(new Rope(pos));

            return LoadVarietyTile("BlockA", 10, TileCollision.Impassable);
        }

        private Tile LoadTurretTile(int x, int y)
        {
            Point pt = GetBounds(x, y).Center;
            Vector2 pos = new Vector2(pt.X, pt.Y);
            turretManager.Add(new XyBorg.Enemies.Turret(pos));
            return LoadVarietyTile("BlockA", 10, TileCollision.Impassable);
        }
        private Tile LoadJumperTile(int x, int y)
        {
            // Сам объект.
            Point pt = GetBounds(x, y).Center;
            Vector2 pos = new Vector2(pt.X, pt.Y);
            Objects.Jumper j = new Objects.Jumper(pos);

            // Внешний вид.

            //Content.Load<ParticleSystemPiplineLib.ParticleSystemImportStruct>("Sprites/Effects/ParticleSystems/FirePit.xml");
            ParticleSystem.GenericEmitter starter1 = new ParticleSystem.JumperEmitter1();
            ParticleSystem.GenericEmitter starter2 = new ParticleSystem.JumperEmitter2Distortion();
            starter1.position = GetBounds(x, y); starter1.position.Y += starter1.position.Height / 2 + starter1.position.Height / 4; starter1.position.Height /= 2;
            starter2.position = starter1.position;
            ParticleSystem.Global.Add(new ParticleSystem.ParticleSystem(starter1, "Sprites/Effects/Smoke.afx.txt"));
            ParticleSystem.Global.Add(new ParticleSystem.ParticleSystem(starter2, "Sprites/Effects/HotFireDistortionNormalMap.afx.txt"));
            return LoadVarietyTile("Jumper", 2, TileCollision.Impassable);
        }

        /// <summary>
        /// Создаёт тайл. Остальные функции создания тайлов
        /// как правило ссылются на эту.
        /// </summary>
        /// <param name="name">
        /// Путь относительно Content/Tiles.
        /// </param>
        /// <param name="collision">
        /// Тип пересечения - проходимости.
        /// </param>
        /// <returns>Новый тайл.</returns>
        private Tile LoadTile(string name, TileCollision collision)
        {
            return new Tile(Content.Load<Texture2D>("Tiles/" + name), collision);
        }


        /// <summary>
        /// Загружает тайл со случайным внешним видом.
        /// </summary>
        /// <param name="baseName">
        /// Префикс для этой группы тайлов.
        /// имя0.png, имя1.png, имя2.png и т.д.
        /// </param>
        /// <param name="variationCount">
        /// Количество вариаций.
        /// </param>
        /// <param name="collision">
        /// Тип тайла по взаимодействию с игроком.
        /// </param>
        private Tile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName + index, collision);
        }


        /// <summary>
        /// Создаёт игрока и запоминает его начальное расположение.
        /// </summary>
        private Tile LoadStartTile(int x, int y)
        {
            if (Players != null)
                throw new NotSupportedException("На уровне уже есть точка входа!");

            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            player1 = new Player(this, start, Player.PlayerType.PlayerOne);
            player2 = new Player(this, start, Player.PlayerType.PlayerTwo);
            Players = new PlayerProxy();

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Забоминаем точку выхода.
        /// </summary>
        private Tile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("На уровне уже есть выход!");

            exit = GetBounds(x, y).Center;

            return LoadTile("Exit", TileCollision.Passable);
        }

        /// <summary>
        /// Создаём врага, размещаем на уровне.
        /// </summary>
        private Tile LoadEnemyTile(int x, int y, string spriteSet)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            //enemies.Add(new Enemy(this, position, spriteSet));
            AddEnemy(new Enemy(this, position, spriteSet));

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Создаём предмет и размещаем на уровне.
        /// </summary>
        private Tile LoadItemTile(int x, int y, Item.item_type type)
        {
            Point position = GetBounds(x, y).Center;
            switch (type)
            {
                case Item.item_type.item:
                    items.Add(new Item(this, new Vector2(position.X, position.Y)));
                    break;
                case Item.item_type.minigun:
                    items.Add(new XyBorg.Items.Minigun(this, new Vector2(position.X, position.Y)));
                    break;
                case Item.item_type.lasergun:
                    items.Add(new XyBorg.Items.Lasergun(this, new Vector2(position.X, position.Y)));
                    break;
                /*case Item.item_type.rope:
                    items.Add(new XyBorg.Items.Rope(this, new Vector2(position.X, position.Y)));
                    break;*/
                case Item.item_type.slow_down_bonus:
                    items.Add(new XyBorg.Items.SlowDownBonus(this, new Vector2(position.X, position.Y)));
                    break;
                case Item.item_type.speed_up_bonus:
                    items.Add(new XyBorg.Items.SpeedUpBonus(this, new Vector2(position.X, position.Y)));
                    break;
                case Item.item_type.health_pack:
                    items.Add(new XyBorg.Items.HealthPack(this, new Vector2(position.X, position.Y)));
                    break;
                case Item.item_type.armor_light:
                    items.Add(new XyBorg.Items.ArmorLight(this, new Vector2(position.X, position.Y)));
                    break;
                case Item.item_type.armor_middle:
                    items.Add(new XyBorg.Items.ArmorMid(this, new Vector2(position.X, position.Y)));
                    break;
                case Item.item_type.armor_heavy:
                    items.Add(new XyBorg.Items.ArmorHeavy(this, new Vector2(position.X, position.Y)));
                    break;
                case Item.item_type.grenades_pack:
                    items.Add(new XyBorg.Items.Grenades(this, new Vector2(position.X, position.Y)));
                    break;
                default:
                    throw new Exception("Неизветный тип предмета на уровне.");
            }

            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Выгружаем содержимое.
        /// </summary>
        public void Dispose()
        {
            AnimatedEffect.CleanEffects();
            content.Unload();
            XyBorg.ParticleSystem.Global.Destroy();
        }

        #endregion

        #region Bounds and collision

        /// <summary>
        /// Выдаёт проходимость тайла в определённой точке.
        /// Этот метод управляет тайлами за пределами уроня таким образом,
        /// что за пределы слева и справа нельзя выйти, а прыгнуть выше или свалиться
        /// запросто.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            // Не даёт сбежать
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Позволяет прыгать выше потолка и сваливаться в пропасть.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            // Нормальная обработка.
            return tiles[x, y].Collision;
        }
        /// <summary>
        /// Выдаёт проходимость тайла в определённой точке.
        /// Этот метод управляет тайлами за пределами уроня таким образом,
        /// что за пределы слева и справа нельзя выйти, а прыгнуть выше или свалиться
        /// запросто.
        /// </summary>
        public TileCollision GetCollision(Vector2 position)
        {
            int tile_x = (int)Math.Floor((float)position.X / Tile.Width);
            int tile_y = (int)Math.Floor((float)position.Y / Tile.Height);
            return GetCollision(tile_x, tile_y);
        }

        /// <summary>
        /// Выдаёт прямоугольник тайла в мировых координатах.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }

        /// <summary>
        /// Ширина уровня в тайлах.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Высота уровня в тайлах.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion

        #region Update

        /// <summary>
        /// Обновляет все объекты в мире, обрабатывает столкновения
        /// и лимит времени.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Ставим на паузу если герой умер или время истекло.
            if (!Players.IsAlive || TimeRemaining == TimeSpan.Zero)
            {
                // Всё ещё нужно обрабатывать физику.
                Players.ApplyPhysics(gameTime);
            }
            else if (ReachedExit)
            {
                // Анимируем превращение остатка времени в очки.
                int seconds = (int)Math.Round(gameTime.ElapsedGameTime.TotalSeconds * 100.0f);
                seconds = Math.Min(seconds, (int)Math.Ceiling(TimeRemaining.TotalSeconds));
                timeRemaining -= TimeSpan.FromSeconds(seconds);
                score += seconds * PointsPerSecond;
            }
            else
            {
                timeRemaining -= gameTime.ElapsedGameTime;

                Players.Update(gameTime);

                Players.UpdateItems(gameTime);
                UpdateRopes(gameTime);
                Players.UpdateRopes(gameTime);
                Objects.IBaseObject.UpdateObjects(gameTime);
                turretManager.Update(gameTime);

                /*// Падение убивает игрока.
                if (Player.BoundingRectangle.Top >= Height * Tile.Height)
                    OnPlayerKilled(null);*/

                UpdateEnemies(gameTime);

            }

            // Ограничиваем время нулём.
            if (timeRemaining < TimeSpan.Zero)
                timeRemaining = TimeSpan.Zero;


            XyBorg.ParticleSystem.Global.update(gameTime);
        }


        /// <summary>
        /// Список эффектов на уровне.
        /// </summary>
        public List<AreaEffect> AreaEffects = new List<AreaEffect>();

        /// <summary>
        /// Анимирует каждого врага и позволяет им убить игрока.
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            enemies =  enemies_grid[GetEnemySetIndexX()+1, GetEnemySetIndexY()];
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);

                Player[] players = {Player1,Player2};
                for( int i = 0; i<2; i++ )
                {
                    Player player = players[i];

                    Rectangle shoot_rec = player.BoundingRectangle;
                    shoot_rec.Width *= 4;
                    shoot_rec.X += player.Direction * shoot_rec.Width / 2;
                    // Прикосновение врага убивает.
                    if (enemy.BoundingRectangle.Intersects(player.BoundingRectangle))
                    {
                        if (player.IsRolling)
                        {
                            enemy.OnKilled(player);
                        }
                        else
                        {
                            if (enemy.IsAlive)
                            {
                                enemy.OnPlayerIntersect(player, gameTime);
                            }
                        }
                    }
                    else if (player.weapon.lasergun.IsShooting && enemy.BoundingRectangle.Intersects(shoot_rec))
                    {
                        enemy.OnKilled(player);
                    }
                }
                /*{
                    Rectangle shoot_rec = Player2.BoundingRectangle;
                    shoot_rec.Width *= 4;
                    shoot_rec.X += Player2.Direction * shoot_rec.Width / 2;
                    // Прикосновение врага убивает.
                    if (enemy.BoundingRectangle.Intersects(Player2.BoundingRectangle))
                    {
                        if (Player2.IsRolling)
                        {
                            enemy.OnKilled(Player2);
                        }
                        else
                        {
                            if (enemy.IsAlive)
                            {
                                Player2.OnKilled(enemy);
                            }
                        }
                    }
                    else if (Player2.weapon.lasergun.IsShooting && enemy.BoundingRectangle.Intersects(shoot_rec))
                    {
                        enemy.OnKilled(Player2);
                    }
                }*/

                List<AreaEffect> OnRemove_AE_List = new List<AreaEffect>();
                foreach (AreaEffect ae in AreaEffects)
                {
                    /*if (ae.type == AreaEffect.ae_type.kill)
                    {
                        if (enemy.BoundingRectangle.Intersects(ae.rect))
                        {
                            enemy.OnKilled(null);
                            OnRemove_AE_List.Add(ae);
                        }
                    }*/
                    ae.Use(enemy);
                    if (ae.on_destroy)
                        OnRemove_AE_List.Add(ae);
                }
                foreach (AreaEffect ae in OnRemove_AE_List)
                {
                    AreaEffects.Remove(ae);
                }
            }
        }
        private void UpdateRopes(GameTime gameTime)
        {
            for (int i = 0; i < ropes.Count; ++i)
            {
                Rope rope = ropes[i];
                rope.Update(gameTime);
            }
        }

        /// <summary>
        /// Вызываем когда предмет подобран.
        /// </summary>
        /// <param name="item">Предмет, который был подобран.</param>
        /// <param name="collectedBy">Игрок подобравший предмет.</param>
        internal bool OnItemCollected(Item item, Player collectedBy)
        {
            return item.OnCollected(collectedBy);
        }

        /*/// <summary>
        /// Вызываем когда игрок убит.
        /// </summary>
        /// <param name="killedBy">
        /// Враг убивший игрока. Равно null если игрока не убивали,
        /// например он свалился в дыру.
        /// </param>
        private void OnPlayerKilled(Enemy killedBy)
        {
            playerDiedTimes++;
            Player.OnKilled(killedBy);
        }*/

        /// <summary>
        /// Вызываем когда игрок достиг выхода.
        /// </summary>
        public void OnExitReached()
        {
            Player1.OnReachedExit();
            Player2.OnReachedExit();
            exitReachedSound.Play();
            reachedExit = true;
        }

        /// <summary>
        /// Возвращает игрока к началу уровня, чтобы он попробовал снова.
        /// </summary>
        public void StartNewLife()
        {
            if (!Player1.IsAlive)
            {
                if (!Player2.IsAlive)
                {
                    Player1.Reset(start);
                    Player2.Reset(start);
                }
                else
                {
                    Player1.Reset(Player2.Position);
                }
            }
            else
            {
                if (!Player2.IsAlive)
                {
                    Player2.Reset(Player1.Position);
                }
            }
        }

        #endregion

        #region Draw

        private XyBorg.Utility.Profiler draw_level_layers_profiler = new XyBorg.Utility.Profiler("layers");
        private XyBorg.Utility.Profiler draw_enemies_profiler = new XyBorg.Utility.Profiler("enemies");
        private XyBorg.Utility.Profiler draw_player_profiler = new XyBorg.Utility.Profiler("player");
        private XyBorg.Utility.Profiler draw_tiles_profiler = new XyBorg.Utility.Profiler("tiles");
        /// <summary>
        /// Отрисовываем всё на уровне от заднего плана к переднему.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, float ellapsedTime)
        {
            draw_level_layers_profiler.start_task();
            {
                spriteBatch.Begin();
                {
                    for (int i = 0; i <= EntityLayer; ++i)
                        layers[i].Draw(spriteBatch, cameraPosition, ellapsedTime);
                } spriteBatch.End();
            } draw_level_layers_profiler.end_task();

            ScrollCamera(spriteBatch.GraphicsDevice.Viewport);
            Matrix cameraTransform = Matrix.CreateTranslation(-cameraPosition.X, cameraPosition.Y, 0.0f);
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None,
                cameraTransform);

            draw_tiles_profiler.start_task();
                DrawTiles(spriteBatch);
            draw_tiles_profiler.end_task();

            // Отрисовка предметов.
            foreach (Item item in items)
                item.Draw(gameTime, spriteBatch);
            // Отрисовка верёвок.
            foreach (Rope rope in ropes)
                rope.Draw(gameTime, spriteBatch);

            draw_player_profiler.start_task();
                Players.Draw(gameTime, spriteBatch);
            draw_player_profiler.end_task();

            draw_enemies_profiler.start_task();
            {
                foreach (Enemy enemy in enemies)
                    enemy.Draw(gameTime, spriteBatch);
                turretManager.Draw(spriteBatch);
            } draw_enemies_profiler.end_task();

            spriteBatch.End();

            draw_level_layers_profiler.start_task();
            {
                spriteBatch.Begin();
                {
                    for (int i = EntityLayer + 1; i < layers.Length; ++i)
                        layers[i].Draw(spriteBatch, cameraPosition, ellapsedTime);
                } spriteBatch.End();
            } draw_level_layers_profiler.end_task();
        }

        /// <summary>
        /// Отсекаем лишних (расстояние в две камеры). Определяет находится ли точка внутри облати двух камер.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsPointWithinScreen(Vector2 point)
        {
            //float   dw = Width * Tile.Width * 0.5f,
            //        dh = Height * Tile.Height * 0.5f;
            float   dw = Tile.Width * 10.0f,
                    dh = Tile.Height * 10.0f;
            float left = cameraPosition.X - dw,
                right = cameraPosition.X + 1280 + dw,
                down = (- cameraPosition.Y) - dh,
                up = (- cameraPosition.Y) + 720 + dh;
            return (
                point.X > left &&
                point.X < right &&
                point.Y > down &&
                point.Y < up);
        }

        /// <summary>
        /// Точка за которой следит камера.
        /// </summary>
        Vector2 view_point = Vector2.Zero;
        private void ScrollCamera(Viewport viewport)
        {
#if ZUNE
            const float ViewMargin = 0.45f;
#else
            const float ViewMargin = 0.35f;//0.35f;
#endif
            //Находим края экрана.
            //float marginWidth = viewport.Width * ViewMargin; 
            Vector2 margin = new Vector2(viewport.Width, viewport.Height) * ViewMargin;
            Vector2 marginLeftBottom = cameraPosition + margin;
            Vector2 marginRightTop = cameraPosition + new Vector2(viewport.Width, viewport.Height) - margin;

            //Рассчитываем как сильно скролить когда игрок подошёл к краю экрана.
            Vector2 cameraMovement = Vector2.Zero;

            Vector2 position = Vector2.Zero;
            if (camera_player_index == PlayerIndex.One)
            {
                position = Player1.Position;
            }
            else if (camera_player_index == PlayerIndex.Two)
            {
                position = Player2.Position;
            }

            // X
            if (position.X < marginLeftBottom.X)
                cameraMovement.X = position.X - marginLeftBottom.X;
            else if (position.X > marginRightTop.X)
                cameraMovement.X = position.X - marginRightTop.X;
            // Y
            if (position.Y < marginLeftBottom.Y)
                cameraMovement.Y = position.Y + marginLeftBottom.Y;
            else if (position.Y > marginRightTop.Y)
                cameraMovement.Y = position.Y - marginRightTop.Y;

            // Обновляем положение камеры, не даём ей выйти за края экрана.
            Vector2 maxCameraPosition = new Vector2(Tile.Width * Width - viewport.Width, Tile.Height * Height - viewport.Height);
            cameraPosition.X = MathHelper.Clamp(cameraPosition.X + cameraMovement.X, 0.0f, maxCameraPosition.X);
            cameraPosition.Y = -MathHelper.Clamp(cameraPosition.Y + cameraMovement.Y, 0.0f, maxCameraPosition.Y);
        }
        private PlayerIndex camera_player_index = PlayerIndex.One;
        public void SelectCameraPlayer( PlayerIndex playerIndex)
        {
            camera_player_index = playerIndex;
        }

        /// <summary>
        /// Отрисовывает каждый тайл на уровне.
        /// </summary>
        private void DrawTiles(SpriteBatch spriteBatch)
        {
            int left = (int)Math.Floor(cameraPosition.X / Tile.Width);
            int right = left + spriteBatch.GraphicsDevice.Viewport.Width / Tile.Width + 1;
            int top = (int)Math.Floor(-cameraPosition.Y / Tile.Height - 1);
            int bottom = top + spriteBatch.GraphicsDevice.Viewport.Height / Tile.Height + 1;
            right = Math.Min(right, Width - 1);
            top = Math.Max(top, 0);
            bottom = Math.Min(bottom, Height - 1);
            // Для каждого расположения тайла
            /*for (int y = 0; y < Height; ++y)*/
            //Рисуем снизу вверх
            //for (int y = Height-1; y >= 0; --y)
            for (int y = bottom; y >= top; --y)
            {
                for (int x = left; x < right; ++x)
                {
                    // если здесь существует видимый тайл
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // отрисовываем его в экранных координатах
                        Vector2 position = new Vector2(x, y) * Tile.Size - new Vector2((texture.Width - Tile.Width), (texture.Height - Tile.Height)) * 0.5f;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
        }

        #endregion
    }
}
