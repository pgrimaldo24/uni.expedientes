using MediatR;
using System.Collections.Generic;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetPagedComportamientosExpedientes
{
    public class GetPagedComportamientosExpedientesQuery : QueryParameters, IRequest<ResultListDto<ComportamientosExpedientesListItemDto>>
    {
        public string Nombre { get; set; }
        public bool? EstaVigente { get; set; }
        public string NivelUso { get; set; }
        public ICollection<int> IdsCondiciones { get; set; }
    }
}