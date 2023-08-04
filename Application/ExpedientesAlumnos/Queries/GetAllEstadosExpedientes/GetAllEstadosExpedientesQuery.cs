using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllEstadosExpedientes
{
    public class GetAllEstadosExpedientesQuery : QueryParameters, IRequest<EstadoExpedienteListItemDto[]>
    {
        public string FilterNombre { get; set; }
    }
}
