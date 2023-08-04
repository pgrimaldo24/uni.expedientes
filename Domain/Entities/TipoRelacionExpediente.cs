using System.Collections.Generic;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class TipoRelacionExpediente : Entity<int>
    {
        public const int CambioPlan = 2;
        public const int Seminario = 3;

        // Propiedades Primitivas
        /// <summary>
        /// Nombre del Tipo de Relación
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Indica si el expediente asociado forma parte de un logro del expediente; por defecto es false
        /// </summary>
        public bool EsLogro { get; set; }

        // Propiedades de Navegación
        public virtual ICollection<RelacionExpediente> RelacionesExpedientes { get; set; }
        public virtual ICollection<RequisitoExpedienteRequerimientoTitulo> RequisitosExpedientesRequerimientosTitulos { get; set; }
    }
}
