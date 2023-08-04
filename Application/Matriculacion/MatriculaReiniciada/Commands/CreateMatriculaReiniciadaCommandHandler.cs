using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaReiniciadaCommon;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaReiniciada.Commands
{
    public class CreateMatriculaReiniciadaCommandHandler : IRequestHandler<CreateMatriculaReiniciadaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        public CreateMatriculaReiniciadaCommandHandler(IExpedientesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateMatriculaReiniciadaCommand request, CancellationToken cancellationToken)
        {
            var matriculaReiniciada = new CreateMatriculaReiniciadaCommonCommand
            {
                AlumnoIdIntegracion = request.AlumnoIdIntegracion,
                MatriculaIdIntegracion = request.MatriculaIdIntegracion,
                FechaHora = request.FechaHora,
                MensajeSeguimiento = "Desestimar matrícula cancelado,{0}",
                TipoSituacionId = TipoSituacionEstado.CanceladaDesestimacionMatricula,
                Mensaje = request.Mensaje,
                Origen = request.Origen
            };
            await _mediator.Send(matriculaReiniciada, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
