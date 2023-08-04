using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoById
{
    public class GetExpedienteAlumnoByIdQuery : IRequest<ExpedienteAlumnoItemDto>
    {
        public int IdExpedienteAlumno { get; set; }

        public GetExpedienteAlumnoByIdQuery(int idExpedienteAlumno)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
        }
    }
}
