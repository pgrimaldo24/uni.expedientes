using MediatR;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Commands.MigrarCalificacionesExpedientesBackground
{
    public class MigrarCalificacionesExpedientesBackgroundCommand : IRequest<string>
    {
        public string IdRefUniversidad { get; set; }
        public string IdRefEstudio { get; set; }
    }
}
