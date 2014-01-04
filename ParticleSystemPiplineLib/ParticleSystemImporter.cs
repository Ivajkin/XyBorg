using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

// TODO: replace this with the type you want to import.
using TImport = System.String;

namespace ParticleSystemPiplineLib
{
    /// <summary>
    /// Чтение с диска.
    /// </summary>
    [ContentImporter(".psys", DisplayName = "Particle System Importer", DefaultProcessor = "ParticleSystemProcessor")]
    public class ParticleSystemImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            // TODO: read the specified file into an instance of the imported type.
            throw new NotImplementedException();
        }
    }
}
