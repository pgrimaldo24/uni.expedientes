using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.RelacionarExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesAlumnosARelacionar;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ProcesarRelacionExpedienteAlumnoById
{
    public class ProcesarRelacionExpedienteAlumnoByIdCommandHandler : IRequestHandler<ProcesarRelacionExpedienteAlumnoByIdCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;

        public ProcesarRelacionExpedienteAlumnoByIdCommandHandler(
            IExpedientesContext context, 
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ProcesarRelacionExpedienteAlumnoByIdCommand request, CancellationToken cancellationToken)
        {
            var expedienteAlumno = (await _mediator.Send(new GetExpedientesAlumnosARelacionarQuery
            {
                IdExpedienteAlumno = request.IdExpedienteAlumno
            }, cancellationToken)).FirstOrDefault();

            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            var tipoRelacionExpediente = await _context.TiposRelacionesExpediente.FirstAsync(e =>
                                            e.Id == TipoRelacionExpediente.CambioPlan, cancellationToken);

            await _mediator.Send(new RelacionarExpedienteAlumnoCommand(expedienteAlumno, tipoRelacionExpediente), cancellationToken);
            return Unit.Value;
        }
    }
}
