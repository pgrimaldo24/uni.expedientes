using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaReiniciadaCommon;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionReiniciada.Commands
{
    public class CreateMatriculaAmpliacionReiniciadaCommandHandler : IRequestHandler<CreateMatriculaAmpliacionReiniciadaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        public CreateMatriculaAmpliacionReiniciadaCommandHandler(
            IExpedientesContext context,
            IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(CreateMatriculaAmpliacionReiniciadaCommand request, CancellationToken cancellationToken)
        {
            var matriculaAmpliacionReiniciada = new CreateMatriculaReiniciadaCommonCommand
            {
                AlumnoIdIntegracion = request.AlumnoIdIntegracion,
                MatriculaIdIntegracion = request.MatriculaIdIntegracion,
                FechaHora = request.FechaHora,
                MensajeSeguimiento = "Desestimar ampliación matrícula cancelado,{0}",
                TipoSituacionId = TipoSituacionEstado.CanceladaDesestimacionAmpliacionMatricula,
                Mensaje = request.Mensaje,
                Origen = request.Origen
            };
            await _mediator.Send(matriculaAmpliacionReiniciada, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
