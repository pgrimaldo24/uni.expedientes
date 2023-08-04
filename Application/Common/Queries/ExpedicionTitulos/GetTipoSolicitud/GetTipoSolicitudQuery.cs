using MediatR;

namespace Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTipoSolicitud
{
    public class GetTipoSolicitudQuery : IRequest<TipoSolicitudDto>
    {
        public int Id { get; set; }
        public GetTipoSolicitudQuery(int id)
        {
            Id = id;
        }
    }
}
