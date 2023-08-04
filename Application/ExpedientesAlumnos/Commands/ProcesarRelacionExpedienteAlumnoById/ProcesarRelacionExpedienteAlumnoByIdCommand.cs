using MediatR;
using System;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ProcesarRelacionExpedienteAlumnoById
{
    public class ProcesarRelacionExpedienteAlumnoByIdCommand : IRequest
    {
        public int? IdExpedienteAlumno { get; set; }
    }
}
