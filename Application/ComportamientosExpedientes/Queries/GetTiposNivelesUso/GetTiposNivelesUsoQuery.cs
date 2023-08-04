using MediatR;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetTiposNivelesUso
{
    public class GetTiposNivelesUsoQuery : QueryParameters, IRequest<TipoNivelUsoListItemDto[]>
    {
        public string FilterNombre { get; set; }
    }
}
