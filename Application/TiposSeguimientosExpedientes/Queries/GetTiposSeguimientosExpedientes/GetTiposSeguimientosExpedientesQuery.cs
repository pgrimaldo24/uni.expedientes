using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.TiposSeguimientosExpedientes.Queries.GetTiposSeguimientosExpedientes
{
    public class GetTiposSeguimientosExpedientesQuery : QueryParameters, IRequest<TipoSeguimientoExpedienteListItemDto[]>
    {
        public string Nombre { get; set; }
    }
}
