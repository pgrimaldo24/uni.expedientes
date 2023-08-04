using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class CausaEstadoRequisitoConsolidadaExpediente : Entity<int>
    {
        /// <summary>
        /// Nombre de la causa
        /// </summary>
        [StringLength(250)]
        public string Nombre { get; set; }

        // Propiedades de Navegación
        public virtual EstadoRequisitoExpediente EstadoRequisitoExpediente { get; set; }
        public virtual RequisitoExpediente RequisitoExpediente { get; set; }
        public virtual ICollection<ConsolidacionRequisitoExpediente> ConsolidacionesRequisitosExpedientes { get; set; }
    }
}
