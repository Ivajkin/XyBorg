using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

// TODO: replace these with the processor input and output types.
using TInput = ParticleSystemPiplineLib.ParticleSystemImportStruct;
using TOutput = ParticleSystemPiplineLib.ParticleSystemImportStruct;

namespace ParticleSystemPiplineLib
{
    /// <summary>
    /// Запись на диск.
    /// </summary>
    [ContentProcessor(DisplayName = "Particle System Processor")]
    public class ParticleSystemProcessor : ContentProcessor<TInput, TOutput>
    {
        public override TOutput Process(TInput input, ContentProcessorContext context)
        {
            // TODO: process the input object, and return the modified data.
            return input;
        }
    }
}