using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace XyBorg
{
    class PostProcessor
    {
        protected bool initialised = false;

        internal void LoadContent(GraphicsDeviceManager graphics, ContentManager Content)
        {
            backbuffer = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            initialised = true;
        }

        public RenderTarget2D backbuffer { private set; get; }
        protected EffectParameter distortion_texture;

        protected RenderTarget2D temp = null;

        /// <summary>
        /// Используется ли постпроцессор в данный момент.
        /// Prerender() меняет его на true, PostrenderDoNotDraw() и Postrender() на false.
        /// </summary>
        protected bool in_use_now = false;
        internal void Prerender(GraphicsDevice GraphicsDevice)
        {
            if (!initialised)
                throw new Exception("Постпроцессор не инициализирован, но используется.");

            if (in_use_now)
                throw new Exception("Этот постпроцессор уже используется!");
            in_use_now = true;

            temp =
                (RenderTarget2D)GraphicsDevice.GetRenderTarget(0);
            GraphicsDevice.SetRenderTarget(0, backbuffer);
        }

        internal void PostrenderDoNotDraw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
            if (!initialised)
                throw new Exception("Постпроцессор не инициализирован, но используется.");

            GraphicsDevice.SetRenderTarget(0, temp);
            temp = null;

            // Обязательно.
            in_use_now = false;
        }
        /*internal void Postrender(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch)
        {
            if (!initialised)
                throw new Exception("Постпроцессор не инициализирован, но используется.");

            GraphicsDevice.SetRenderTarget(0, temp);
            temp = null;

            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            ripple.Begin();
            ripple.CurrentTechnique.Passes[0].Begin();
            spriteBatch.Draw(backbuffer.GetTexture(), Vector2.Zero, Color.White);
            ripple.CurrentTechnique.Passes[0].End();
            ripple.End();
            spriteBatch.End();
        }*/
    }
    class NormalDistortionPostProcessor : PostProcessor
    {
        new internal void LoadContent(GraphicsDeviceManager graphics, ContentManager Content)
        {
            GraphicsDeviceCapabilities caps = graphics.GraphicsDevice.GraphicsDeviceCapabilities;
            // Проверяем, поддерживает ли видеокарта пиксльные шейдеры версии 2.0
            if (caps.MaxPixelShaderProfile < ShaderProfile.PS_2_0)
            {
                normal_distortion_fx = null;
            }
            else
            {
                normal_distortion_fx = Content.Load<Effect>("Shaders/normal_distortion_ps_2_0");
                distortion_texture = normal_distortion_fx.Parameters["DistortionTexture"];
            }
            base.LoadContent(graphics, Content);
        }
        protected Effect normal_distortion_fx;
        //private new void Postrender(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch) { }
        /// <summary>
        /// Используется для обработки с нормал картой.
        /// </summary>
        /// <param name="GraphicsDevice"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="diffuse">Построцессор, обработавший цвета (всю сцену).</param>
        internal void Postrender(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, PostProcessor diffuse)
        {
            if (!initialised)
                throw new Exception("Постпроцессор не инициализирован, но используется.");

            GraphicsDevice.SetRenderTarget(0, temp);
            temp = null;

            // Проверяем, существует ли шейдер.
            if (normal_distortion_fx == null)
            {
                spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    spriteBatch.Draw(diffuse.backbuffer.GetTexture(), Vector2.Zero, Color.White);
                }
                spriteBatch.End();
            }
            else
            {
                distortion_texture.SetValue(backbuffer.GetTexture());

                spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    normal_distortion_fx.Begin();
                    {
                        normal_distortion_fx.CurrentTechnique.Passes[0].Begin();
                        {
                            spriteBatch.Draw(diffuse.backbuffer.GetTexture(), Vector2.Zero, Color.White);
                        }
                        normal_distortion_fx.CurrentTechnique.Passes[0].End();
                    }
                    normal_distortion_fx.End();
                }
                spriteBatch.End();
            }

            // Обязательно.
            in_use_now = false;
        }
        /// <summary>
        /// Используется для обработки с нормал картой.
        /// </summary>
        /// <param name="GraphicsDevice"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="diffuse">Построцессор, обработавший цвета (всю сцену).</param>
        /// <param name="texture_to_render">Куда будет отрисовываться финальный результат.</param>
        internal void Postrender(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, PostProcessor diffuse, RenderTarget2D texture_to_render)
        {
            if (!initialised)
                throw new Exception("Постпроцессор не инициализирован, но используется.");

            GraphicsDevice.SetRenderTarget(0, texture_to_render);

            // Проверяем, существует ли шейдер.
            if (normal_distortion_fx == null)
            {
                spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    spriteBatch.Draw(diffuse.backbuffer.GetTexture(), Vector2.Zero, Color.White);
                }
                spriteBatch.End();
            }
            else
            {
                distortion_texture.SetValue(backbuffer.GetTexture());

                spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
                {
                    normal_distortion_fx.Begin();
                    {
                        normal_distortion_fx.CurrentTechnique.Passes[0].Begin();
                        {
                            spriteBatch.Draw(diffuse.backbuffer.GetTexture(), Vector2.Zero, Color.White);
                        }
                        normal_distortion_fx.CurrentTechnique.Passes[0].End();
                    }
                    normal_distortion_fx.End();
                }
                spriteBatch.End();
            }

            GraphicsDevice.SetRenderTarget(0, temp);
            temp = null;

            // Обязательно.
            in_use_now = false;
        }
    }
    class HDRBloomFX
    {
        bool initialised = false;
        private Effect bright_pass;
        private Effect blur_3px_x;
        private Effect blur_3px_y;
        /// <summary>
        /// Складывает изображения и настраивает яркость.
        /// </summary>
        private Effect bloom_final;

        public RenderTarget2D diffuse_texture;
        private RenderTarget2D half_tmp1;
        private RenderTarget2D half_tmp4;
        private RenderTarget2D full_tmp2;
        private RenderTarget2D full_tmp3;
        /// <summary>
        /// Содержит прошлый RenderTarget2D.
        /// </summary>
        private RenderTarget2D tmp;

        private EffectParameter glow_texture;

        public HDRBloomFX()
        {
        }
        public void LoadContent(GraphicsDeviceManager graphics)
        {
            bright_pass = Content.Load<Effect>("Shaders/bright_pass_ps_2_0");
            blur_3px_x = Content.Load<Effect>("Shaders/blur_3px_x_ps_2_0");
            blur_3px_y = Content.Load<Effect>("Shaders/blur_3px_y_ps_2_0");
            bloom_final = Content.Load<Effect>("Shaders/bloom_final_ps_2_0");

            screen_width = graphics.PreferredBackBufferWidth;
            screen_height = graphics.PreferredBackBufferHeight;
            diffuse_texture = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            half_tmp1 = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth / 3, graphics.PreferredBackBufferHeight / 3, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            //half_tmp2 = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            full_tmp2 = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            full_tmp3 = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            half_tmp4 = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth / 3, graphics.PreferredBackBufferHeight / 3, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            /*quater_tmp2 = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth / 4, graphics.PreferredBackBufferHeight / 4, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            quater_tmp3 = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth / 4, graphics.PreferredBackBufferHeight / 4, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);*/

            glow_texture = bloom_final.Parameters["GlowTex"];
            
            initialised = true;
        }
        int screen_width = 1280;
        int screen_height = 720;
        private void Shade(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, RenderTarget2D target, Effect shader, RenderTarget2D base_texture)
        {
            GraphicsDevice.SetRenderTarget(0, target);

            Rectangle target_size;

            if (target != null)
            {
                target_size = new Rectangle(0, 0, target.Width, target.Height);
            }
            else
            {
                target_size = new Rectangle(0, 0, screen_width, screen_height);
            }
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                shader.Begin();
                {
                    shader.Techniques[0].Passes[0].Begin();
                    {
                        spriteBatch.Draw(base_texture.GetTexture(), target_size, Color.White);
                    }
                    shader.Techniques[0].Passes[0].End();
                }
                shader.End();
            }
            spriteBatch.End();
        }
        public void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch/*, Texture2D diffuse_color*/, RenderTarget2D render_target)
        {
            if (!initialised)
                throw new Exception("HDR постпроцессор не инициализирован, но используется.");

            tmp = (RenderTarget2D)GraphicsDevice.GetRenderTarget(0);
            if (render_target == null)
                render_target = tmp;

            // Первый пасс - по яркости.
            Shade(GraphicsDevice, spriteBatch, full_tmp2, bright_pass, diffuse_texture);

            // Второй пасс - карта меньшая в два раза прозодит blur на три пикселя в каждую сторону по x.
            Shade(GraphicsDevice, spriteBatch, half_tmp1, blur_3px_x, full_tmp2);

            // Третий пасс - карта меньшая в два раза прозодит blur на три пикселя в каждую сторону по y.
            Shade(GraphicsDevice, spriteBatch, half_tmp4, blur_3px_y, half_tmp1);
            Shade(GraphicsDevice, spriteBatch, half_tmp1, blur_3px_x, half_tmp4);
            Shade(GraphicsDevice, spriteBatch, half_tmp4, blur_3px_y, half_tmp1);
            // Последний пасс.
            //Shade(GraphicsDevice, spriteBatch, tmp, bloom_final, full_tmp3);
            GraphicsDevice.SetRenderTarget(0, render_target);
            glow_texture.SetValue(half_tmp4.GetTexture());
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                bloom_final.Begin();
                {
                    bloom_final.Techniques[0].Passes[0].Begin();
                    {
                        spriteBatch.Draw(diffuse_texture.GetTexture(), new Rectangle(0, 0, screen_width, screen_height), Color.White);
                    }
                    bloom_final.Techniques[0].Passes[0].End();
                }
                bloom_final.End();
            }
            spriteBatch.End();


            GraphicsDevice.SetRenderTarget(0, tmp);
        }
    }
    class MotionBlurFX
    {
        private bool initialised = false;
        /// <summary>
        /// Эффект для записи скорости в RenderTarget2D velocity.
        /// </summary>
        private Effect motion_capture;
        private EffectParameter current_velocity_3d;
        /// <summary>
        /// Эффект использует две текстуры - диффузию и тектуру скоростей.
        /// </summary>
        private Effect motion_blur;
        /// <summary>
        /// Здесь хранится цвет.
        /// </summary>
        internal RenderTarget2D diffuse_texture;
        /// <summary>
        /// Здесь хранятся направления.
        /// </summary>
        private RenderTarget2D velocity;
        private EffectParameter velocity_texture;
        internal RenderTarget2D tmp;
        /// <summary>
        /// Загрузка содержимого.
        /// </summary>
        internal void LoadContent(GraphicsDeviceManager graphics)
        {
            motion_blur = Content.Load<Effect>("Shaders/motion_blur_ps_2_0");
            velocity_texture = motion_blur.Parameters["VelocityTexture"];
            motion_capture = Content.Load<Effect>("Shaders/motion_capture_ps_2_0");
            current_velocity_3d = motion_capture.Parameters["current_velocity"];
            diffuse_texture = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            velocity = new RenderTarget2D(graphics.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, 1, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            initialised = true;
        }
        /// <summary>
        /// Здесь начинаем отрисовывать скорости.
        /// </summary>
        internal void Begin(GraphicsDevice device)
        {
            if (!initialised)
                throw new Exception("Motion Blur постпроцессор не инициализирован, но используется.");


            tmp = (RenderTarget2D)device.GetRenderTarget(0);

            device.SetRenderTarget(0, velocity);
            motion_capture.Begin();
            motion_capture.CurrentTechnique.Passes[0].Begin();
            SetVelocity(new Vector3(0,0,0));
            device.Clear(Color.Gray);
        }
        /// <summary>
        /// Скорость задаётся для координаты от -1 до +1.
        /// </summary>
        /// <param name="current_velocity"></param>
        internal void SetVelocity(Vector3 current_velocity)
        {
            current_velocity += new Vector3(1, 1, 1);
            current_velocity *= 0.5f;
            current_velocity_3d.SetValue(current_velocity);
        }
        /// <summary>
        /// Постобработка - после отрисовки скоростей.
        /// </summary>
        /// <param name="device"></param>
        /// <param name="spriteBatch"></param>
        /// <param name="render_target">Куда кидаем результат.</param>
        internal void End(GraphicsDevice device, SpriteBatch spriteBatch, RenderTarget2D render_target)
        {
            if (!initialised)
                throw new Exception("Motion Blur постпроцессор не инициализирован, но используется.");

            motion_capture.CurrentTechnique.Passes[0].End();
            motion_capture.End();

            if (render_target == null)
                render_target = tmp;


            device.SetRenderTarget(0, render_target);
            velocity_texture.SetValue(velocity.GetTexture());
            spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            {
                motion_blur.Begin();
                {
                    motion_blur.CurrentTechnique.Passes[0].Begin();
                    {
                        spriteBatch.Draw(diffuse_texture.GetTexture(), Vector2.Zero, Color.White);
                    }
                    motion_blur.CurrentTechnique.Passes[0].End();
                }
                motion_blur.End();
            }
            spriteBatch.End();
        }
    }
    /*interface IPostFX
    {
        void Begin();
        void End(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, RenderTarget2D render_target);
        public RenderTarget2D surface { get; private set; }
    }
    class PostFXChain
    {
        private List<IPostFX> post_effects = new List<IPostFX>();
        public void Push(IPostFX effect) {
            post_effects.Add(effect);
        }
        public void Clear()
        {
            post_effects.Clear();
        }
        public void Draw(GraphicsDevice GraphicsDevice, SpriteBatch spriteBatch, RenderTarget2D render_target)
        {
        }
    }*/
}
