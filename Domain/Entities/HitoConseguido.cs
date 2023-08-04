using System;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class HitoConseguido : Entity<int>
    {
        public const string PrimeraMatricula = "Acceso a la Titulación";

        /// <summary>
        /// Nombre del hito
        /// </summary>
        public string Nombre { get; set; }
        /// <summary>
        /// Fecha de inicio del hito
        /// </summary>
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Fecha de fin del hito
        /// </summary>
        public DateTime? FechaFin { get; set; }

        // Propiedades de Navegación
        public int TipoConseguidoId { get; set; }
        public virtual TipoHitoConseguido TipoConseguido { get; set; }
        public int ExpedienteAlumnoId { get; set; }
        public virtual ExpedienteAlumno ExpedienteAlumno { get; set; }
        public int? ExpedienteEspecializacionId { get; set; }
        public virtual ExpedienteEspecializacion ExpedienteEspecializacion { get; set; }
    }
}
