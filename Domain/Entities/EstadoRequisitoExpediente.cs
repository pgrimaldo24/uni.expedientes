using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class EstadoRequisitoExpediente : Entity<int>
    {
        public const int NoProcesado = 1;
        public const int Pendiente = 2;
        public const int Rechazado = 3;
        public const int Validado = 4;

        /// <summary>
        /// Nombre del tipo
        /// </summary>
        [StringLength(50)]
        public string Nombre { get; set; }

        /// <summary>
        /// Número de orden del tipo
        /// </summary>
        public int Orden { get; set; }

        // Propiedades de Navegación
        public virtual ICollection<CausaEstadoRequisitoConsolidadaExpediente> CausasEstadosRequisitosConsolidadasExpediente { get; set; }
        public virtual ICollection<ConsolidacionRequisitoExpediente> ConsolidacionesRequisitosExpedientes { get; set; }
    }
}
