using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using TRead = ParticleSystemPiplineLib.ParticleSystemImportStruct;

namespace ParticleSystemPiplineLib
{
    /// <summary>
    /// Этот код используется для чтения систем частиц из файлов формата .xnb.
    /// </summary>
    class ParticleSystemContentReader : ContentTypeReader<TRead>
    {
        protected override TRead Read(ContentReader input, TRead existingInstance)
        {
            /*string particle = input.ReadString();
            Vector2 start_particle_velocity = input.ReadVector2();
            float start_praticle_velocity_angle_random = (float)input.ReadDouble();
            float start_particle_velocity_random_k = (float)input.ReadDouble();
            Vector2 start_position = input.ReadVector2();
            Vector2 start_size = input.ReadVector2();
            float particles_per_second = (float)input.ReadDouble();
            float particle_friction = (float)input.ReadDouble();

            return new ParticleSystem(new GenericEmitter(start_particle_velocity, start_praticle_velocity_angle_random, start_particle_velocity_random_k, new Rectangle((int)start_position.X, (int)start_position.Y, (int)start_size.X, (int)start_size.Y), particles_per_second, particle_friction), particle);*/
            TRead ret = new TRead();

            ret.particle = input.ReadString();
            ret.start_particle_velocity = input.ReadVector2();
            ret.start_praticle_velocity_angle_random = (float)input.ReadDouble();
            ret.start_particle_velocity_random_k = (float)input.ReadDouble();
            ret.start_position = input.ReadVector2();
            ret.start_size = input.ReadVector2();
            ret.particles_per_second = (float)input.ReadDouble();
            ret.particle_friction = (float)input.ReadDouble();

            return ret;
        }
    }
}
