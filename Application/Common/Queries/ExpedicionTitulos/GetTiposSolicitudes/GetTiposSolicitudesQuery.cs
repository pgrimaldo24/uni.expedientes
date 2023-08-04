using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTiposSolicitudes
{
    public class GetTiposSolicitudesQuery : IRequest<List<TiposSolicitudesDto>>
    {
        public string Nombre { get; set; }
        public List<string> IdsRefUniversidad { get; set; }
    }
}
