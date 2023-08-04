using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class RolRequisitoExpediente : Entity<int>
    {
        /// <summary>
        /// Código del rol
        /// </summary>
        [StringLength(150)]
        public string Rol { get; set; }

        // Propiedades de Navegación
        public virtual RequisitoExpediente RequisitoExpediente { get; set; }
    }
}
