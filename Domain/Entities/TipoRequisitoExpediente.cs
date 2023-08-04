using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class TipoRequisitoExpediente : Entity<int>
    {
        /// <summary>
        /// Nombre del tipo de requisito
        /// </summary>
        [StringLength(50)]
        public string Nombre { get; set; }

        // Propiedades de navegacion
        public virtual ICollection<RequisitoComportamientoExpediente> RequisitosComportamientosExpedientes { get; set; }
        public virtual ICollection<ConsolidacionRequisitoExpediente> ConsolidacionesRequisitosExpedientes { get; set; }
    }
}
