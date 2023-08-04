using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class TipoNivelUso : Entity<int>
    {
        public const int Universidad = 1;
        public const int TipoEstudio = 2;
        public const int Estudio = 3;
        public const int PlanEstudio = 4;
        public const int TipoAsignatura = 5;
        public const int AsignaturaPlan = 6;

        /// <summary>
        /// Nombre del tipo
        /// </summary>
        [StringLength(50)]
        public string Nombre { get; set; }

        /// <summary>
        /// Número de orden del tipo
        /// </summary>
        public int Orden { get; set; }

        // Propiedade de navegación
        public virtual ICollection<NivelUsoComportamientoExpediente> NivelesUsoComportamientosExpedientes { get; set; }
    }
}
