using MediatR;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.RelacionarExpedienteAlumno
{
    public class RelacionarExpedienteAlumnoCommand : IRequest
    {
        public ExpedienteAlumno ExpedienteAlumno { get; set; }
        public TipoRelacionExpediente TipoRelacionExpediente { get; set; }
        public RelacionarExpedienteAlumnoCommand(
            ExpedienteAlumno expedienteAlumno, 
            TipoRelacionExpediente tipoRelacionExpediente)
        {
            ExpedienteAlumno = expedienteAlumno;
            TipoRelacionExpediente = tipoRelacionExpediente;
        }
    }
}
