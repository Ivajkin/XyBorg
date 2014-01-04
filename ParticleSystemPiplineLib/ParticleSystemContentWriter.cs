using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

// TODO: replace this with the type you want to write out.
using TWrite = ParticleSystemPiplineLib.ParticleSystemImportStruct;

namespace ParticleSystemPiplineLib
{
    /// <summary>
    /// Этот код используется для записи систем частиц в файлы формата .xnb.
    /// </summary>
    [ContentTypeWriter]
    public class ParticleSystemContentWriter : ContentTypeWriter<TWrite>
    {
        protected override void Write(ContentWriter output, TWrite value)
        {
            output.Write(value.particle);
            output.Write(value.start_particle_velocity);
            output.Write(value.start_praticle_velocity_angle_random);
            output.Write(value.start_particle_velocity_random_k);
            output.Write(value.start_position);
            output.Write(value.start_size);
            output.Write(value.particles_per_second);
            output.Write(value.particle_friction);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(ParticleSystemContentReader).AssemblyQualifiedName;//"MyNamespace.MyContentReader, MyGameAssembly";
        }
    }
}
