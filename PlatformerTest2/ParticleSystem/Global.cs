using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace XyBorg.ParticleSystem
{
    /// <summary>
    /// Управляется классом Level.
    /// Класс управляет системами частиц.
    /// </summary>
    static class Global
    {
        static bool initialised = false;
        public static void Initialise()
        {
            if (initialised)
                throw new Exception("Уже инициализирован класс Global для систем частиц.");

            //defPT.particle = new AnimatedEffect("Sprites/Effects/ExplosionNormalMap.afx.txt", new Vector2(100, 100), SpriteEffects.None);
            //AnimatedEffect.AddEffect(defPT.particle);

            // Создаём для проверки.
            {
                RainEmitter rain = new RainEmitter();
                //particle_systems.Add(new ParticleSystem(rain, "Sprites/Effects/RainDrops.afx.txt"));
                RainPS = new ParticleSystem(rain, "Sprites/Effects/RainDropsNormalMap.afx.txt");
                particle_systems.Add(RainPS);

                DefaultEmitter starter = new DefaultEmitter();
                particle_systems.Add(new ParticleSystem(starter, "Sprites/Effects/Smoke.afx.txt"));
                particle_systems.Add(new ParticleSystem(starter, "Sprites/Effects/SmokeNormalMap.afx.txt"));

            }
            LoadContent();

            initialised = true;
        }
        static private void LoadContent()
        {
            SoundEffect rainFX1 = Content.Load<SoundEffect>("Sounds/Ambient/Rain1");
            SoundEffect rainFX2 = Content.Load<SoundEffect>("Sounds/Ambient/Rain2");
            rainSound = new SoundEffectInstance[2];
            {
                rainSound[0] = rainFX1.CreateInstance();
                rainSound[0].Volume = 0;
                rainSound[0].IsLooped = true;
                rainSound[0].Play();
            }
            {
                rainSound[1] = rainFX2.CreateInstance();
                rainSound[1].Volume = 0;
                rainSound[1].IsLooped = true;
                rainSound[1].Play();
            }
        }
        /// <summary>
        /// Добавляет систему частиц.
        /// </summary>
        /// <param name="ps">Система частиц для добавления.</param>
        public static void Add(ParticleSystem ps)
        {
            particle_systems.Add(ps);
        }
        public static void Destroy()
        {
            if (!initialised)
                throw new Exception("Не инициализирован класс Global для систем частиц.");
            initialised = false;
            particle_systems.Clear();
        }
        public static ParticleType defPT = new ParticleType();
        public static List<ParticleSystem> particle_systems = new List<ParticleSystem>();
        public static Vector2 wind = new Vector2(10.01f, 110.01f);
        private static ParticleSystem RainPS = null;
        private static ParticleSystem[] FlameGunPS = new ParticleSystem[2];
        static SoundEffectInstance[] rainSound;
        public static void update(GameTime gameTime)
        {
            if (!initialised)
                throw new Exception("Не инициализирован класс Global для систем частиц.");

            float time_since_last_frame = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Обновим ветер.
            wind += new Vector2(RandomValue.get_float(-time_since_last_frame * 10, time_since_last_frame * 10), RandomValue.get_float(-time_since_last_frame * 10, time_since_last_frame * 10));
            // Обновим дождь.
            float rainy_k = (float)Math.Max(0, Math.Sin(-(float)gameTime.TotalGameTime.TotalSeconds / 40));
            rainSound[0].Volume = rainy_k;
            rainSound[1].Volume = rainy_k;

            RainPS.starter.particles_per_second = 900 * rainy_k;//RandomValue.get_float(-time_since_last_frame * 100.5f, time_since_last_frame * 100.5f);
            RainPS.starter.position = new Rectangle((int)Level.current_level.CameraPosition.X-400, -(int)Level.current_level.CameraPosition.Y, RainPS.starter.position.Width, RainPS.starter.position.Height);
           
            // Обновим все частицы.
            List<ParticleSystem> RemoveList = new List<ParticleSystem>();
            foreach (ParticleSystem particle_system in particle_systems)
            {
                particle_system.update(time_since_last_frame);
                if (particle_system.isDead)
                {
                    RemoveList.Add(particle_system);
                }
            }
            foreach (ParticleSystem particle_system in RemoveList)
            {
                particle_systems.Remove(particle_system);
            }
        }

        public static void PSExplode(float power, Vector2 position)
        {
            XyBorg.ParticleSystem.GenericEmitter starter1 = new XyBorg.ParticleSystem.GenericEmitter(new Vector2(1, 1) * 30.0f * power, (float)Math.PI * 2, 5.6f, new Rectangle((int)position.X, (int)position.Y, 5, 5), 100.0f * power, 0.0005987f);
            XyBorg.ParticleSystem.ParticleSystem ps = new XyBorg.ParticleSystem.ParticleSystem(starter1, "Sprites/Effects/Smoke.afx.txt");
            ps.gravity = new Vector2(100, -150);
            ps.time_to_live = 0.12f;
            XyBorg.ParticleSystem.Global.Add(ps);

#if !XYBORG
            ps = new XyBorg.ParticleSystem.ParticleSystem(starter1, "Sprites/Effects/SmokeBig.afx.txt");
            ps.gravity = new Vector2(100, -150);
            ps.time_to_live = 0.12f;
            XyBorg.ParticleSystem.Global.Add(ps);
#endif


            XyBorg.ParticleSystem.GenericEmitter starter2 = new XyBorg.ParticleSystem.GenericEmitter(new Vector2(1, 1) * 29.0f * power, (float)Math.PI * 2, 5.6f, new Rectangle((int)position.X, (int)position.Y, 5, 5), 20.0f * power, 0.000001f);
            ps = new XyBorg.ParticleSystem.ParticleSystem(starter1, "Sprites/Effects/SmokeNormalMap.afx.txt");
            ps.gravity = new Vector2(100, -150);
            ps.time_to_live = 0.12f;
            XyBorg.ParticleSystem.Global.Add(ps);

        }
    }
}
