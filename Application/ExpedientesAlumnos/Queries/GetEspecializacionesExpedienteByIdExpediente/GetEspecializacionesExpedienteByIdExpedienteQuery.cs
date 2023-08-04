using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetEspecializacionesExpedienteByIdExpediente
{
    public class GetEspecializacionesExpedienteByIdExpedienteQuery : IRequest<List<ExpedienteEspecializacionDto>>
    {
        public int IdExpedienteAlumno { get; set; }
        public GetEspecializacionesExpedienteByIdExpedienteQuery(int idExpedienteAlumno)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
        }
    }
}
