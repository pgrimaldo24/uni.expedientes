using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class ComportamientoExpediente : Entity<int>
    {
        /// <summary>
        /// Nombre del comportamiento
        /// </summary>
        [StringLength(250)]
        public string Nombre { get; set; }

        /// <summary>
        /// Descripción del comportamiento
        /// </summary>
        public string Descripcion { get; set; }

        /// <summary>
        /// Indica si el comportamiento está o no vigente; valor true, por defecto
        /// </summary>
        public bool EstaVigente { get; set; }

        /// <summary>
        /// Indica si el comportamiento está bloqueado y no se puede gestionar; valor false, por defecto.
        /// Los requisitos asociados al comportamiento se pueden añadir libremente al expediente.
        /// Este comportamiento tiene como objetivo aglutinar requisitos que no se añadirían inicialmente al expediente, pero que, posteriormente, el usuario podría añadirlos manualmente.
        /// Esto es útil para añadir sanciones, logros como premios, documentación adicional, certificado de idioma, etc.
        /// Además, esos requisitos asociados podrán consolidarse varias veces, verificando que su información asociada es diferente
        /// </summary>
        public bool Bloqueado { get; set; }

        // Propiedades de navegación
        public virtual ICollection<RequisitoComportamientoExpediente> RequisitosComportamientosExpedientes { get; set; }
        public virtual ICollection<NivelUsoComportamientoExpediente> NivelesUsoComportamientosExpedientes { get; set; }
    }
}
