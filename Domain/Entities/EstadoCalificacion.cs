
using System.Collections.Generic;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class EstadoCalificacion : Entity<int>
    {
        public const int EstadoProvisional = 1;
        public const int EstadoDefinitiva = 2;
        public const int EstadoCerrada = 3;
        /// <summary>
        /// Nombre del estado de la calificación
        /// </summary>
        public string Nombre { get; set; }
        public virtual ICollection<AsignaturaCalificacion> AsignaturasCalificaciones { get; set; }
    }
}
