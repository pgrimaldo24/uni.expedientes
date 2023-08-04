using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditTipoVinculacionExpedienteAlumno
{
    public class EditTipoVinculacionByExpedienteAlumnoCommand : IRequest<bool>
    {
        public int IdExpedienteAlumno { get; set; }
        public string IdRefTipoVinculacion { get; set; }

        public EditTipoVinculacionByExpedienteAlumnoCommand(int idExpedienteAlumno, string idRefTipoVinculacion)
        {
            IdExpedienteAlumno = idExpedienteAlumno;
            IdRefTipoVinculacion = idRefTipoVinculacion;
        }
    }
}
