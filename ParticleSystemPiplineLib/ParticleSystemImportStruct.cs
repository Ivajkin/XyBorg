using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace ParticleSystemPiplineLib
{
    public struct ParticleSystemImportStruct
    {
        public string particle;
        public Vector2 start_particle_velocity;
        public float start_praticle_velocity_angle_random;
        public float start_particle_velocity_random_k;
        public Vector2 start_position;
        public Vector2 start_size;
        public float particles_per_second;
        public float particle_friction;
    }
}
