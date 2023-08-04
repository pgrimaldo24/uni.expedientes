using MediatR;

namespace Unir.Expedientes.Application.TiposSituacionEstadoExpedientes.Queries.GetUltimoTipoSituacionEstadoByIdExpediente
{
    public class GetUltimoTipoSituacionEstadoByIdExpedienteQuery : IRequest<TipoSituacionEstadoExpedienteDto>
    {
        public int IdExpedienteAlumno { get; set; }
        public GetUltimoTipoSituacionEstadoByIdExpedienteQuery(int idExpedienteAlumno)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
        }
    }
}
