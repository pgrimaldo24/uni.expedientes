using MediatR;

namespace Unir.Expedientes.Application.AsignaturasExpediente.Commands.EditAsignaturaExpedienteToSeguimientoByNotaFinal
{
    public class EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand : IRequest
    {
        public int IdAsignaturaExpedienteAlumno { get; set; }
        public bool EsNoPresentado { get; set; }
        public bool EsMatriculaHonor { get; set; }
        public bool EsSuperada { get; set; }
    }
}
