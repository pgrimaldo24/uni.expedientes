using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetConfiguracionExpedienteUniversidadByIdExpediente
{
    public class GetConfiguracionExpedienteUniversidadByIdExpedienteQuery : IRequest<ConfiguracionExpedienteUniversidadDto>
    {
        public int IdExpedienteAlumno { get; set; }
        public GetConfiguracionExpedienteUniversidadByIdExpedienteQuery(int idExpedienteAlumno)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
        }
    }
}
