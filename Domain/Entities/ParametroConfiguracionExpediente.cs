using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Unir.Framework.DomainSuperTypes;

namespace Unir.Expedientes.Domain.Entities
{
    public class ParametroConfiguracionExpediente : Entity<int>
    {
        [StringLength(250)]
        public string Nombre { get; set; }

        public virtual ICollection<ParametroConfiguracionExpedienteFileType> ParametrosConfiguracionesExpedientesFilesTypes { get; set; }
    }
}
