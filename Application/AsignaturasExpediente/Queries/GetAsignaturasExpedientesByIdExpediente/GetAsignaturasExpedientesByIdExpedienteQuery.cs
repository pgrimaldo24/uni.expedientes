using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetAsignaturasExpedientesByIdExpediente
{
    public class GetAsignaturasExpedientesByIdExpedienteQuery : IRequest<List<AsignaturaExpedienteListItemDto>>
    {
        public int IdExpedienteAlumno { get; set; }
        public GetAsignaturasExpedientesByIdExpedienteQuery(int idExpedienteAlumno)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
        }
    }
}
