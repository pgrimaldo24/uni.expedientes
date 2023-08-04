using System;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class TipoSituacionEstadoExpediente : Entity<int>
    {
        // Propiedades Primitivas
        /// <summary>
        /// Fecha de inicio de la situación
        /// </summary>
        public DateTime FechaInicio { get; set; }

        /// <summary>
        /// Fecha de Fin de la situación
        /// </summary>
        public DateTime? FechaFin { get; set; }

        /// <summary>
        /// Descripción que amplía información sobre la situación
        /// </summary>
        public string Descripcion { get; set; }

        // Propiedades de Navegación
        public int TipoSituacionEstadoId { get; set; }
        public virtual TipoSituacionEstado TipoSituacionEstado { get; set; }
        public int ExpedienteAlumnoId { get; set; }
        public virtual ExpedienteAlumno ExpedienteAlumno { get; set; }
    }
}
