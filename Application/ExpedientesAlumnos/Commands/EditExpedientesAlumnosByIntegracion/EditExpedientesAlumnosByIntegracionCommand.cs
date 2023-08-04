using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedientesAlumnosByIntegracion
{
    public class EditExpedientesAlumnosByIntegracionCommand : IRequest
    {
        public List<EditExpedienteAlumnoByIdIntegracionParametersDto> ExpedientesAlumnos { get; set; }

        public EditExpedientesAlumnosByIntegracionCommand(
            List<EditExpedienteAlumnoByIdIntegracionParametersDto> expedientesAlumnos)
        {
            ExpedientesAlumnos = expedientesAlumnos;
        }
    }
}
