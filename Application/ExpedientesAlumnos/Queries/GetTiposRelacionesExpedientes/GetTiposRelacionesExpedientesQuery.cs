using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetTiposRelacionesExpedientes
{
    public class GetTiposRelacionesExpedientesQuery : QueryParameters, IRequest<TipoRelacionExpedienteListItemDto[]>
    {
        public string FilterNombre { get; set; }
    }
}
