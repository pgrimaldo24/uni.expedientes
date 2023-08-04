using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class ConfiguracionExpedienteUniversidad : Entity<int>
    {
        /// <summary>
        /// Identificador de la universidad
        /// </summary>
        public string IdRefUniversidad { get; set; }
        /// <summary>
        /// Acrónimo de la universidad
        /// </summary>
        public string AcronimoUniversidad { get; set; }
        /// <summary>
        /// Nombre de la universidad
        /// </summary>
        public string NombreUniversidad { get; set; }
        /// <summary>
        /// Identificador de integración de la universidad
        /// </summary>
        public string IdIntegracionUniversidad { get; set; }
        /// <summary>
        /// Código de la clasificación documental correspondiente a una universidad
        /// </summary>
        public string CodigoDocumental { get; set; }
        /// <summary>
        /// Tamaño máximo en Mb de un fichero asociado al expediente; por defecto, es 10 Mb
        /// </summary>
        public int TamanyoMaximoFichero { get; set; }
        /// <summary>
        /// Tiempo máximo en días que puede estar inactivo un expediente; por defecto, es 730 días
        /// </summary>
        public int TiempoMaximoInactividad { get; set; }
    }
}
