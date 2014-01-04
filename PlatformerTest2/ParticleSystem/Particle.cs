using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XyBorg.ParticleSystem
{
    class ParticleType
    {
        internal AnimatedEffect particle;
        internal string particle_data_path;
        //internal ParticleType children;
        //internal float friction = 0.9987f;
    }
    class Particle : ParticleType
    {
        internal Vector2 position;/*
        {
            set { type.particle.position = value; }
            get { return type.particle.position; }
        }*/
        internal Vector2 velocity;
        //internal ParticleType type;
        internal Particle(string particle_name, Vector2 _position, Vector2 _velocity, float _friction)
        {
            //type = _type;
            particle_data_path = particle_name;

            velocity = _velocity;
            position = _position;

            friction = _friction;

            particle = new AnimatedEffect(particle_name, position, SpriteEffects.None);
            AnimatedEffect.AddEffect(particle);
        }
        private float life_time = 0;

        internal float friction;

        internal void update(float time_since_last_frame)
        {
            life_time += time_since_last_frame;

            velocity += Global.wind * time_since_last_frame;
            velocity *= (float)Math.Pow(friction, time_since_last_frame);
            position += velocity * time_since_last_frame;
            particle.position = position;
        }
    }
}