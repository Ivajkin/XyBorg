using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace XyBorg
{
    /// <summary>
    /// Основной класс игры.
    /// </summary>
    public class PlatformerGame : Microsoft.Xna.Framework.Game
    {
        XyBorg.Utility.Profiler input_update_profiler = new XyBorg.Utility.Profiler("Game.Update.Input");
        XyBorg.Utility.Profiler level_update_profiler = new XyBorg.Utility.Profiler("Game.Update.Level");
        XyBorg.Utility.Profiler draw_profiler = new XyBorg.Utility.Profiler("Game.Draw");
        XyBorg.Utility.Profiler normal_distortion_profiler = new XyBorg.Utility.Profiler("Normals+Distortion");
        XyBorg.Utility.Profiler hdr_bloom_profiler = new XyBorg.Utility.Profiler("HDRPostFX");
        XyBorg.Utility.Profiler draw_hud_profiler = new XyBorg.Utility.Profiler("Hud");
        XyBorg.Utility.Profiler draw_level_profiler = new XyBorg.Utility.Profiler("Level");
        XyBorg.Utility.Profiler draw_animated_effects_profiler = new XyBorg.Utility.Profiler("AnimatedEffects");

        PostProcessor pp = new PostProcessor();
        NormalDistortionPostProcessor distortion_pp = new NormalDistortionPostProcessor();
        HDRBloomFX hdr_bloom_pp = null;

        enum GameMode
        {
            menu,
            worldMap,
            game
        }
        GameMode currentGameMode = GameMode.menu;
        MainMenu menu;
        WorldMap worldMap;

        class game_state_t
        {
            //private bool paused = false;
            public bool paused { private set; get; }
            private bool button_was_released = true;
            internal void pause_key_check(bool pause_key_pressed)
            {
                if (pause_key_pressed)
                {
                    if (!button_was_released)
                        return;
                    else
                    {
                        paused = !paused;

                        if (pause_sound == null)
                            throw new Exception("Звук не инициализирован для паузы.");
                        pause_sound.Play();
                        if (paused)
                        {
                            XyBorg.GUI.Add("Pause Game GUI Element Set", new XyBorg.GUIElements.Canvas(new Vector2(0, 0), new Vector2(0, 0), "Overlays/game_paused"));
                            XyBorg.GUI.Add("Pause Game GUI Element Set", new XyBorg.GUIElements.Text("Режим Паузы", new Vector2(-245, -160),XyBorg.GUIElements.Text.font_big));
                            XyBorg.GUI.Add("Pause Game GUI Element Set", new XyBorg.GUIElements.Button(new Vector2(100, 100), new Vector2(0, 0), "Overlays/OK_button_normal", "Overlays/OK_button_hover", "Overlays/OK_button_pressed", delegate() { pause_key_check(true); }));

                            XyBorg.GUI.Add("Pause Game GUI Element Set", new XyBorg.GUIElements.Text(@"Чтобы продолжить нажмите 'P'.", new Vector2(-240, -100), XyBorg.GUIElements.Text.font_normal));
                        }
                        else
                        {
                            GUI.Destroy("Pause Game GUI Element Set");
                        }
                    }
                    button_was_released = false;
                }
                else
                {
                    button_was_released = true;
                }
            }
            private Microsoft.Xna.Framework.Audio.SoundEffect pause_sound = null;
            public void LoadContent()
            {
                pause_sound = /*Level.current_level.*/global::XyBorg.Content.Load<Microsoft.Xna.Framework.Audio.SoundEffect>("Sounds/Interface/PauseGame");
            }
        }; private game_state_t game_state = new game_state_t();
        /*class editor_mode_t
        {
            private bool enabled = false;
            private StreamWriter sw = null;
            private bool mouse_was_pressed = false;
            public void enable()
            {
                if (!enabled)
                {
                    enabled = true;
                    sw = new StreamWriter("EditorPositions.list.txt");
                }
            }

            internal void check_mouse_pressed()
            {
                if (enabled)
                {
                    MouseState mouseState = Mouse.GetState();
                    if (mouseState.MiddleButton == ButtonState.Pressed)
                    {
                        if (!mouse_was_pressed)
                        {
                            mouse_was_pressed = true;
                            sw.Write("{" + (mouseState.X + Level.current_level.CameraPosition.X) + "," + (mouseState.Y - Level.current_level.CameraPosition.Y) + "}");
                            sw.WriteLine();
                            sw.Flush();
                            AnimatedEffect.AddEffect(new AnimatedEffect("Effects/FireflyFlower.afx.txt", new Vector2(mouseState.X + Level.current_level.CameraPosition.X, mouseState.Y - Level.current_level.CameraPosition.Y), SpriteEffects.None));
                        }
                    }
                    else
                    {
                        mouse_was_pressed = false;
                    }
                }
            }
        } editor_mode_t editor_mode = new editor_mode_t();
        enum game_mode
        {
            menu,
            game,
            shop
        };*/
        // Конфигурационный файл
        ConfigParser config = new ConfigParser("config.ini");

        // Ресурсы отрисовки.
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Основное содержимое.
        private SpriteFont hudFont;

        private Texture2D winOverlay;
        private Texture2D loseOverlay;
        private Texture2D diedOverlay;

        private Texture2D energyBar;
        private Texture2D armorHudBar;
        private Texture2D clockHudBar;
        private Texture2D goldBar;
        private Texture2D skullBar;
        private Texture2D lifeBar;

        private Texture2D cursorTexture;

        // Доуровневое игровое состояние.
        private int levelIndex = 0;
        private Level level;
        private bool wasContinuePressed;

        // Время - наибольший порог, начиная с которого счётчик начинает предупреждающе мерцать.
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

#if ZUNE
        private const int TargetFrameRate = 30;        
        private const int BackBufferWidth = 240;
        private const int BackBufferHeight = 320;
        private const Buttons ContinueButton = Buttons.B;        
#else
        private const int TargetFrameRate = 60;
        //!private const int BackBufferWidth = 1280;
        //!private const int BackBufferHeight = 720;
        private const Buttons ContinueButton = Buttons.A;
#endif

        /// Конструктор
        public PlatformerGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = config.GetInt("screen_x");
            graphics.PreferredBackBufferHeight = config.GetInt("screen_y");
            GUI.window_half_size = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            graphics.IsFullScreen = config.GetBool("fullscreen");
            /*if (config.GetBool("use_hdr"))
            {
                //RenderTarget renderTarget = new RenderTarget2D(graphics, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 1, SurfaceFormat.HalfVector4, MultiSampleType.None, 0, RenderTargetUsage.DiscardContents);
                graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            }
            else
            {
                graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            }*/
            if (config.GetBool("use_hdr"))
            {
                hdr_bloom_pp = new HDRBloomFX();
            }
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;

            Content.RootDirectory = "Content";
            XyBorg.Content.set(Content);

            TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / TargetFrameRate);
        }

        /// <summary>
        /// Деструктор - выключает вибрацию геймпада.
        /// </summary>
        ~PlatformerGame()
        {
            Xbox_360_Vibration.Reset();
        }
        /// <summary>
        /// Загружает весь основной контент, используется раз за игру.
        /// </summary>
        protected override void LoadContent()
        {
            // Инициализируем постэффекты.
            pp.LoadContent(graphics, Content);
            distortion_pp.LoadContent(graphics, Content);
            if (hdr_bloom_pp != null)
            {
                hdr_bloom_pp.LoadContent(graphics);
            }
            // Создаём новый SpriteBatch, который в дальнейшем используется для отрисовки текстур.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Загрузка шрифтов.
            hudFont = Content.Load<SpriteFont>("Fonts/Hud");
            XyBorg.GUIElements.Text.font_normal = Content.Load<SpriteFont>("Fonts/Russian");
            XyBorg.GUIElements.Text.font_big = Content.Load<SpriteFont>("Fonts/RussianBig");

            // Загрузка оверлеев.
            winOverlay = Content.Load<Texture2D>("Overlays/you_win");
            loseOverlay = Content.Load<Texture2D>("Overlays/you_lose");
            diedOverlay = Content.Load<Texture2D>("Overlays/you_died");

            energyBar = Content.Load<Texture2D>("Sprites/Hud/EnergyBarFull");
            armorHudBar = Content.Load<Texture2D>("Sprites/Hud/ArmorHudBar");
            clockHudBar = Content.Load<Texture2D>("Sprites/Hud/ClockHudBar");
            goldBar = Content.Load<Texture2D>("Sprites/Hud/GoldBar");
            skullBar = Content.Load<Texture2D>("Sprites/Hud/SkullBar");
            lifeBar = Content.Load<Texture2D>("Sprites/Hud/LifeBar");

            cursorTexture = Content.Load<Texture2D>("Sprites/Cursor");

            menu = new MainMenu(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            StartMainMenu();
            //LoadNextLevel();

            //game_state.LoadContent();
        }
        /// <summary>
        /// Редактор карт, декораций.
        /// </summary>
        LevelEditor editor = null;
        /// <summary>
        /// Входим в режим главного меню.
        /// </summary>
        public void StartMainMenu()
        {
            currentGameMode = GameMode.menu;
            menu.Reset(StartWorldMap, delegate() { Exit(); });
        }
        /// <summary>
        /// Входим в режим мировой карты.
        /// </summary>
        public void StartWorldMap()
        {
            currentGameMode = GameMode.worldMap;
            if (worldMap == null)
            {
                worldMap = new WorldMap();
                worldMap.LoadContent(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight));
            }
            worldMap.Reset(new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), StartGame, StartMainMenu);
        }
        /// <summary>
        /// Запуск собственно игры.
        /// </summary>
        /// <param name="level">Уровень с которого начинаем.</param>
        /// <param name="editor_enabled">Активирован ли редактор с самого начала.</param>
        public void StartGame(int level, bool editor_enabled)
        {
            currentGameMode = GameMode.game;
            LoadNextLevel();
            game_state.LoadContent();
            if (editor_enabled)
            {
                editor = new LevelEditor();
            }
        }

        /// <summary>
        /// Позволяет запустить игровую логику, такую как обновление игрового мира,
        /// проверка столкновений, ввода, проигрывание музыки и т.д..
        /// </summary>
        /// <param name="gameTime">Текущее время.</param>
        protected override void Update(GameTime gameTime)
        {
            switch (currentGameMode)
            {
                case GameMode.menu:
                    if (!menu.isEnabled)
                        throw new Exception("Текущий игровой режим - главное меню, но меню не активно.");
                    break;
                case GameMode.worldMap:
                    worldMap.Update(gameTime);
                    break;
                case GameMode.game:
                    {
                        input_update_profiler.start_task();
                        HandleInput();
                        input_update_profiler.end_task();

                        if (!game_state.paused)
                        {
                            level_update_profiler.start_task();
                            level.Update(gameTime);
                            level_update_profiler.end_task();
                        }

                        AreaEffect.current_time = (float)gameTime.TotalGameTime.TotalSeconds;

                        SpellEffect<Player>.Update(gameTime);

                        Xbox_360_Vibration.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

                        if (editor != null && editor.isEnabled)
                            editor.Update();
                    }
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Обработка ввода.
        /// </summary>
        private void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            
            // Выход из игры при нажатии возврата.
            if (gamepadState.Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            /*if (keyboardState.IsKeyDown(Keys.L))
                editor_mode.enable();
            editor_mode.check_mouse_pressed();*/

            if (keyboardState.IsKeyDown(Keys.D1))
                level.SelectCameraPlayer(PlayerIndex.One);
            if (keyboardState.IsKeyDown(Keys.D2))
                level.SelectCameraPlayer(PlayerIndex.Two);

            bool pause_pressed = (keyboardState.IsKeyDown(Keys.P) || gamepadState.Buttons.Start == ButtonState.Pressed);
            game_state.pause_key_check(pause_pressed);

            if (keyboardState.IsKeyDown(Keys.H))
                Message.HelpMessage(1);
            if (keyboardState.IsKeyDown(Keys.O))
                Message.ShowProfileSetName("Это ваше имя?");

            bool continuePressed =
                keyboardState.IsKeyDown(Keys.Space) ||
                keyboardState.IsKeyDown(Keys.W) ||
                keyboardState.IsKeyDown(Keys.Up) ||
                gamepadState.IsButtonDown(ContinueButton);

            // Обновляем игру при особых условиях (смерть, прохождение уровня, окончание времени).
            if (!wasContinuePressed && continuePressed)
            {
                if (!level.Players.IsAlive)
                {
                    level.StartNewLife();
                }
                else if (level.TimeRemaining == TimeSpan.Zero)
                {
                    if (level.ReachedExit)
                        LoadNextLevel();
                    else
                        ReloadCurrentLevel();
                }
            }

            wasContinuePressed = continuePressed;

            GUI.HandleInput();
        }

        /// <summary>
        /// Загрузка следующего уровня.
        /// </summary>
        public void LoadNextLevel()
        {
            // Выгружаем содержимое текущего уровня, прежде чем загружать следующий.
            if (level != null)
                level.Dispose();

            // Загружаем уровень.
            level = new Level(Services, ++levelIndex);
            //ПОДПОРКА!
            if (levelIndex == 3)
            {
                MediaPlayer.Stop();
                MediaPlayer.Play(Content.Load<Song>("Sounds/Music02"));
            }
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }

        bool main_menu_just_closed = true;
        /// <summary>
        /// Отрисовываем начиная с самого заднего плана.
        /// </summary>
        /// <param name="gameTime">Текущее время.</param>
        protected override void Draw(GameTime gameTime)
        {
            switch (currentGameMode)
            {
                case GameMode.menu:
                    if (!menu.isEnabled)
                        throw new Exception("Текущий игровой режим - главное меню, но меню не активно.");
                    graphics.GraphicsDevice.Clear(Color.Goldenrod);
                    menu.Draw(spriteBatch);
                    break;
                case GameMode.worldMap:
                    graphics.GraphicsDevice.Clear(Color.Goldenrod);
                    worldMap.Draw(spriteBatch);
                    break;
                case GameMode.game:
                    {
                        if (main_menu_just_closed)
                        {
                            main_menu_just_closed = false;
                        }
                        else
                        {
                            draw_profiler.start_task();
                            {

                                pp.Prerender(GraphicsDevice);
                                {
                                    //if (!game_state.paused)
                                    //{
                                    graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

                                    draw_level_profiler.start_task();
                                    level.Draw(gameTime, spriteBatch, (float)gameTime.ElapsedGameTime.TotalSeconds);
                                    draw_level_profiler.end_task();

                                    draw_animated_effects_profiler.start_task();
                                    AnimatedEffect.Draw(gameTime, spriteBatch, false);
                                    draw_animated_effects_profiler.end_task();

                                    //}

                                } pp.PostrenderDoNotDraw(GraphicsDevice, spriteBatch);

                                normal_distortion_profiler.start_task();
                                {
                                    distortion_pp.Prerender(GraphicsDevice);
                                    {
                                        graphics.GraphicsDevice.Clear(Color.Gray);
                                        AnimatedEffect.Draw(gameTime, spriteBatch, true);
                                    }
                                    if (hdr_bloom_pp == null)
                                    {
                                        distortion_pp.Postrender(GraphicsDevice, spriteBatch, pp);
                                    }
                                    else
                                    {
                                        distortion_pp.Postrender(GraphicsDevice, spriteBatch, pp, hdr_bloom_pp.diffuse_texture);
                                    }

                                } normal_distortion_profiler.end_task();

                                if (hdr_bloom_pp != null)
                                {
                                    hdr_bloom_profiler.start_task();
                                    {
                                        hdr_bloom_pp.Draw(graphics.GraphicsDevice, spriteBatch, null);
                                    }
                                    hdr_bloom_profiler.end_task();
                                }
                                draw_hud_profiler.start_task();
                                DrawHud();
                                draw_hud_profiler.end_task();

                            } draw_profiler.end_task();
                        }
                    }
                    break;
            }

            base.Draw(gameTime);            
        }

        /// <summary>
        /// Отрисовка интерфейса.
        /// </summary>
        private void DrawHud()
        {
            MouseState mouseState = Mouse.GetState();

            spriteBatch.Begin();
            {

                Rectangle titleSafeArea = GraphicsDevice.Viewport.TitleSafeArea;
                Vector2 hudLocation = new Vector2(titleSafeArea.X + 12, titleSafeArea.Y + 12);
                Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                             titleSafeArea.Y + titleSafeArea.Height / 2.0f);

                // Отрисовка счётчика оставшегося времени.
                // Мигает когда осталось меньше критического уровня
                string timeString = /*"TIME: " + */level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00") + ":" + ((level.TimeRemaining.Milliseconds / 100) % 10).ToString();
                Color timeColor;
                if (level.TimeRemaining > WarningTime ||
                    level.ReachedExit ||
                    (int)level.TimeRemaining.TotalSeconds % 2 == 0)
                {
                    timeColor = Color.Yellow;
                }
                else
                {
                    timeColor = Color.Red;
                }
                spriteBatch.Draw(clockHudBar, hudLocation, Color.White);
                DrawShadowedString(hudFont, timeString, hudLocation + new Vector2(50,0), timeColor);

                // Отрисовка счётчика очков.
                float timeHeight = hudFont.MeasureString(timeString).Y+15;
                spriteBatch.Draw(goldBar, hudLocation + new Vector2(0.0f, timeHeight * 1.2f), Color.White);
                DrawShadowedString(hudFont, /*"SCORE: " + */level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f) + new Vector2(50, 0), Color.Yellow);

                // Индикатор смертей.
                spriteBatch.Draw(skullBar, hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 2), Color.White);
                DrawShadowedString(hudFont, /*"DIED: " + */level.PlayerDiedTimes.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 2) + new Vector2(50, 0), Color.Yellow);

                // Индикатор жизней.
                spriteBatch.Draw(lifeBar, hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 3), Color.White);
                if (level.Player1.HealthPoints > 10)
                    DrawShadowedString(hudFont, /*"Health Points: " + */level.Player1.HealthPoints.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 3) + new Vector2(50, 0), Color.GreenYellow);
                else
                    DrawShadowedString(hudFont, /*"Health Points: " + */level.Player1.HealthPoints.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 3) + new Vector2(50, 0), Color.Red);
                // Индикатор брони.
                spriteBatch.Draw(armorHudBar, hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 4), Color.White);
                DrawShadowedString(hudFont, /*"Armor Points: " + */level.Player1.ArmorPoints.ToString(), hudLocation + new Vector2(50.0f, timeHeight * 1.2f * 4), Color.GreenYellow);

                DrawShadowedString(hudFont, "All images, sounds, etc are properties of their owners.", hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 20), Color.Orange);
                DrawShadowedString(hudFont, "Copyright Ivajkin Timofej 2009-2010 (c).", hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 21), Color.Orange);

                if (level.Players.IsAlive)
                    level.Players.energy.DrawEnergyBar(spriteBatch, energyBar, hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 7));
                //spriteBatch.Draw(energyBar, hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 7), Color.Green);
                else
                    spriteBatch.Draw(energyBar, hudLocation + new Vector2(0.0f, timeHeight * 1.2f * 7), Color.Red);
                // Определяем статус отображаемого оверлея.
                Texture2D status = null;
                if (level.TimeRemaining == TimeSpan.Zero)
                {
                    if (level.ReachedExit)
                    {
                        status = winOverlay;
                    }
                    else
                    {
                        status = loseOverlay;
                    }
                }
                else if (!level.Players.IsAlive)
                {
                    status = diedOverlay;
                }

                if (status != null)
                {
                    // Отображаем сообшение состояния.
                    Vector2 statusSize = new Vector2(status.Width, status.Height);
                    spriteBatch.Draw(status, center - statusSize / 2, Color.White);
                }

                GUI.Draw(spriteBatch);

                // Напоследок рисуем курсор.
                spriteBatch.Draw(cursorTexture, new Vector2(mouseState.X, mouseState.Y), Color.Purple);

            } spriteBatch.End();
        }
        /// <summary>
        /// Отрисовываем текст с эффектом под тень.
        /// </summary>
        /// <param name="font">Шрифт отрисовки.</param>
        /// <param name="value">Выводимый текст.</param>
        /// <param name="position">Размещение выводимого текста.</param>
        /// <param name="color">Цвет выводимого текста.</param>
        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}
