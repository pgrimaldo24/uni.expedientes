using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.SetEstadoHitosEspecializacionesExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesSinEstados;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ProcesarExpedienteByIdSinEstado
{
    public class ProcesarExpedienteByIdSinEstadoCommandHandler : IRequestHandler<ProcesarExpedienteByIdSinEstadoCommand>
    {
        private readonly IMediator _mediator;

        public ProcesarExpedienteByIdSinEstadoCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ProcesarExpedienteByIdSinEstadoCommand request, CancellationToken cancellationToken)
        {
            var expedienteAlumno = (await _mediator.Send(new GetExpedientesSinEstadosQuery
                {
                    IdExpedienteAlumno = request.IdExpedienteAlumno
                },
                cancellationToken)).FirstOrDefault();

            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            await _mediator.Send(new SetEstadoHitosEspecializacionesExpedienteAlumnoCommand(expedienteAlumno), cancellationToken);

            return Unit.Value;
        }
    }
}
