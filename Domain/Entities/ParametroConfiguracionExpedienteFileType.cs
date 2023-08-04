using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class ParametroConfiguracionExpedienteFileType : Entity<int>
    {
        [StringLength(50)]
        public string IdRefFileType { get; set; }

        public int ParametroConfiguracionExpedienteId { get; set; }
        public virtual ParametroConfiguracionExpediente ParametroConfiguracionExpediente { get; set; }
    }
}
