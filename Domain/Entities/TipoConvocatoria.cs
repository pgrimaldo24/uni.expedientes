using System.Collections.Generic;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class TipoConvocatoria : Entity<int>
    {
        public const int Ordinaria = 1;
        public const int Extraordinaria = 2;
        /// <summary>
        /// Código identificativo de la convocatoria
        /// </summary>
        public string Codigo { get; set; }

        /// <summary>
        /// Nombre de la convocatoria
        /// </summary>
        public string Nombre { get; set; }
        public virtual ICollection<AsignaturaCalificacion> AsignaturasCalificaciones { get; set; }
    }
}
