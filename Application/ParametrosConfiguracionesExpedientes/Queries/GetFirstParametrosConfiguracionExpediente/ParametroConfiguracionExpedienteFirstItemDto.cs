using System.Collections.Generic;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.ParametrosConfiguracionesExpedientes.Queries.GetFirstParametrosConfiguracionExpediente
{
    public class ParametroConfiguracionExpedienteFirstItemDto : IMapFrom<ParametroConfiguracionExpediente>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public virtual List<ParametroConfiguracionExpedienteFileTypeListItemDto> ParametrosConfiguracionesExpedientesFilesTypes { get; set; }

    }

    public class ParametroConfiguracionExpedienteFileTypeListItemDto : IMapFrom<ParametroConfiguracionExpedienteFileType>
    {
        public int Id { get; set; }
        public string IdRefFileType { get; set; }
    }
}
