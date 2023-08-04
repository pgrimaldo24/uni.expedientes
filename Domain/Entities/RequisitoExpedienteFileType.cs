using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class RequisitoExpedienteFileType : Entity<int>
    {
        /// <summary>
        /// Identificador de la clasificación documental
        /// </summary>
        [StringLength(50)]
        public string IdRefFileType { get; set; }

        // Propiedades de Navegación
        public virtual RequisitoExpediente RequisitoExpediente { get; set; }
    }
}
