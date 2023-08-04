using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ParametrosConfiguracionesExpedientes.Queries.GetFirstParametrosConfiguracionExpediente
{
    public class GetFirstParametrosConfiguracionExpedienteQuery : QueryParameters, IRequest<ParametroConfiguracionExpedienteFirstItemDto>
    {
    }
}
