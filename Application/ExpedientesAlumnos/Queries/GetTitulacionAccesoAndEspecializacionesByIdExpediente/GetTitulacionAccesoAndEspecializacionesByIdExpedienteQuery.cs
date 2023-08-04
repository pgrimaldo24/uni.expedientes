using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetTitulacionAccesoAndEspecializacionesByIdExpediente;

public class GetTitulacionAccesoAndEspecializacionesByIdExpedienteQuery : IRequest<ExpedienteTitulacionAccesoEspecializacionesItemDto>
{
    public int IdExpedienteAlumno { get; set; }

    public GetTitulacionAccesoAndEspecializacionesByIdExpedienteQuery(int idExpedienteAlumno)
    {
        IdExpedienteAlumno = idExpedienteAlumno;
    }
}