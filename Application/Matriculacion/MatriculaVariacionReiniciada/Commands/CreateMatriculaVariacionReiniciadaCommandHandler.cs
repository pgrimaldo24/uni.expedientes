using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaReiniciadaCommon;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaVariacionReiniciada.Commands
{
    public class CreateMatriculaVariacionReiniciadaCommandHandler : IRequestHandler<CreateMatriculaVariacionReiniciadaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        public CreateMatriculaVariacionReiniciadaCommandHandler(IExpedientesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(CreateMatriculaVariacionReiniciadaCommand request, CancellationToken cancellationToken)
        {
            var matriculaReiniciada = new CreateMatriculaReiniciadaCommonCommand
            {
                AlumnoIdIntegracion = request.AlumnoIdIntegracion,
                MatriculaIdIntegracion = request.MatriculaIdIntegracion,
                FechaHora = request.FechaHora,
                MensajeSeguimiento = "Desestimar variación matrícula cancelado,{0}",
                TipoSituacionId = TipoSituacionEstado.CanceladaDesestimacionVariacionMatricula,
                Mensaje = request.Mensaje,
                Origen = request.Origen
            };
            await _mediator.Send(matriculaReiniciada, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
