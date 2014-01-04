using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

/*
*   +------------------------------+
*   +- For XyBorg -----------------+
*   +------------------------------+
*
*   *Emitter:
*   gravity : value
*   particle type : ParticleType
*   starter : Starter{Template}
*   particles : array of Particle
*
*   *Starter{default}
*   velocity : vector
*   emmiter : rectangle
*   amount : value //particles per second
*   [creates new particles on update]
*
*   *ParticleType:
*   particle : animated effect
*   children : ParticleType
*   friction : value
*
*   *Particle:
*   position : vector
*
*   *Global:
*   Wind : vector
*
*   +------------------------------+
*/
namespace XyBorg.ParticleSystem
{
    class GenericEmitter
    {
        internal GenericEmitter(Vector2 _start_velocity, float _start_velocity_angle_random, float _start_velocity_random_k, Rectangle _position, float _particles_per_second, float _friction)
        {
            start_velocity = _start_velocity.Length();
            /*_start_velocity.Normalize();
            start_velocity_angle = (float)Math.Acos(_start_velocity.X);
            if(_start_velocity.Y<0)
                start_velocity_angle += (float)Math.Acos(0);*/
            start_velocity_angle = CoordConvert.scalar_into_polar(_start_velocity);

            start_velocity_angle_random = _start_velocity_angle_random*0.5f;
            start_velocity_random_k = _start_velocity_random_k;
            position = _position;
            particles_per_second = _particles_per_second;
            friction = _friction;
        }
        internal virtual List<Particle> Create(float time_since_last_frame, string particle_data_path)
        {
            List<Particle> ret = new List<Particle>();
            if (!(Level.current_level.IsPointWithinScreen(new Vector2(position.X, position.Y)) || Level.current_level.IsPointWithinScreen(new Vector2(position.X + position.Width, position.Y + position.Height))))
                return ret;

            full_time += time_since_last_frame;

            if (particles_per_second <= 0)
            {
                particles_per_second = 0;
            }
            else
            {

                while (full_time > 1.0f / particles_per_second)
                {
                    full_time -= 1.0f / particles_per_second;
                    if (particle_count >= max_particles_num)
                        return ret;

                    float c_Angle = start_velocity_angle + RandomValue.get_float(-start_velocity_angle_random, start_velocity_angle_random);
                    float cf_Velosity = start_velocity * RandomValue.get_float(1, start_velocity_random_k);
                    Vector2 c_Velosity = new Vector2(cf_Velosity * (float)Math.Cos(c_Angle), cf_Velosity * (float)Math.Sin(c_Angle));//new Vector2(start_velocity.X * RandomValue.get_float(1, start_velocity_random_k), start_velocity.Y * RandomValue.get_float(1, start_velocity_random_k));
                    Particle p = new Particle(particle_data_path, new Vector2(position.X + RandomValue.get_float(0, position.Width), position.Y + RandomValue.get_float(0, position.Height)), c_Velosity, friction);

                    //p.velocity = start_velocity;
                    ret.Add(p);
                    particle_count++;
                }
            }
            return ret;
        }
        internal void RemoveCheck(int number_of_removed)
        {
            particle_count -= number_of_removed;
        }
        /// <summary>
        /// Начальная скорость частиц.
        /// </summary>
        public float start_velocity;
        /// <summary>
        /// Начальное направление частиц.
        /// </summary>
        public float start_velocity_angle;
        /// <summary>
        /// Разлёт в градусах начального направления частиц +/-.
        /// </summary>
        float start_velocity_angle_random;
        /// <summary>
        /// Коэффициент максимального увеличения (от 1х до Kх).
        /// </summary>
        float start_velocity_random_k;
        /// <summary>
        /// Квадрат внутри которого возникают частицы.
        /// </summary>
        public Rectangle position;
        /// <summary>
        /// Количество частиц возникающих за секунду игрового времени.
        /// </summary>
        public float particles_per_second;
        /// <summary>
        /// Время которое ещё не использованно на создание частиц.
        /// </summary>
        private float full_time = 0;
        /// <summary>
        /// Текущий общий счётчик количества частиц.
        /// </summary>
        private static int particle_count = 0;
        /// <summary>
        /// Коэффициент трения частиц, как медленно они останавливаются.
        /// </summary>
        private float friction;
        /// <summary>
        /// Максисальное общее количество частиц.
        /// </summary>
        private const int max_particles_num = 10000;
        public const int max_particles_per_ps_num = 2000;

    }
    class DefaultEmitter : GenericEmitter
    {
        internal DefaultEmitter()
            : base(new Vector2(-95, 110), (float)Math.PI/5, 1.6f, new Rectangle(32646, 2600, 30, 30), 200.0f, 0.9987f)
        {
        }
    }
    class RainEmitter : GenericEmitter
    {
        internal RainEmitter()
            : base(new Vector2(100, -100), (float)Math.PI / 8, 6, new Rectangle(30046, 1800, 1500, 200), 1000.0f, 0.9987f)
        {
        }
    }
    class FlamethrowerEmitter : GenericEmitter
    {
        internal FlamethrowerEmitter()
            : base(new Vector2(120, 120), (float)Math.PI / 9, 2.75f, new Rectangle(31646, 2500, 8, 5), 220.0f, 0.464f)
        {
        }
    }
    class FlamethrowerDistortionEmitter : GenericEmitter
    {
        internal FlamethrowerDistortionEmitter()
            : base(new Vector2(120, 120), (float)Math.PI / 9, 2.75f, new Rectangle(31646, 2500, 8, 5), 60.0f, 0.464f)
        {
        }
    }
    class FirePitEmitter1 : GenericEmitter
    {
        internal FirePitEmitter1()
            : base(new Vector2(0, 100), (float)Math.PI / 5, 1.4f, new Rectangle(0, 0, 0, 0), 15.0f, 0.9987f)
        {
        }
    }
    class FirePitEmitter2Distortion : GenericEmitter
    {
        internal FirePitEmitter2Distortion()
            : base(new Vector2(0, 100), (float)Math.PI / 5, 1.4f, new Rectangle(0, 0, 0, 0), 5.0f, 0.9987f)
        {
        }
    }
    class JumperEmitter1 : GenericEmitter
    {
        internal JumperEmitter1()
            : base(new Vector2(0, 147), (float)Math.PI / 8.6f, 1.6f, new Rectangle(0, 0, 0, 0), 20.0f, 0.9987f)
        {
        }
    }
    class JumperEmitter2Distortion : GenericEmitter
    {
        internal JumperEmitter2Distortion()
            : base(new Vector2(0, 147), (float)Math.PI / 8.6f, 1.6f, new Rectangle(0, 0, 0, 0), 7.0f, 0.9987f)
        {
        }
    }
}
