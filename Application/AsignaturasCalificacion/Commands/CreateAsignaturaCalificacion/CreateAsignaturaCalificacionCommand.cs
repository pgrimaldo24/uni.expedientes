using System.Collections.Generic;
using MediatR;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos;
using Unir.Expedientes.Application.NotaFinalGenerada;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Commands.CreateAsignaturaCalificacion
{
    public class CreateAsignaturaCalificacionCommand : IRequest
    {
        public NotaFinalGeneradaCommand notaFinal { get; set; }
        public List<AsignaturaMatriculadaModel> asignaturasMatriculadas { get; set; }
        public List<ExpedienteAlumnoListItemDto> Expedientes { get; set; }
    }
}