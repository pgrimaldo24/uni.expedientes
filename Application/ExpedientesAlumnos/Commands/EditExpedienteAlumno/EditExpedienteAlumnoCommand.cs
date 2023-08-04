using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumno
{
    public class EditExpedienteAlumnoCommand : IRequest<int>
    {
        public int IdExpedienteAlumno { get; set; }
        public string IdRefVersionPlan { get; set; }
        public int? NroVersion { get; set; }
        public string IdRefNodo { get; set; }
        public string IdRefViaAcceso { get; set; }
        public TitulacionAccesoParametersDto TitulacionAcceso { get; set; }
    }
}
