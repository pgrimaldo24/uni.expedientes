using System.Collections.Generic;
using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.Common.Models.Results
{
    public class ResultFileContentDto : Result
    {
        /// <summary>Nombre del Archivo (Debe contener la extensión)</summary>
        public string FileName { get; set; }

        /// <summary>Tipos MIME del Archivo.</summary>
        public IEnumerable<string> MimeTypes { get; set; }

        /// <summary>Contenido binario del Archivo.</summary>
        public byte[] Content { get; set; }
    }
}
