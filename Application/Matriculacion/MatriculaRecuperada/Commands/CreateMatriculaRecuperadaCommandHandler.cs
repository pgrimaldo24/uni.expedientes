using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaRecuperadaCommon;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaRecuperada.Commands
{
    public class CreateMatriculaRecuperadaCommandHandler : IRequestHandler<CreateMatriculaRecuperadaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        public CreateMatriculaRecuperadaCommandHandler(IExpedientesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(CreateMatriculaRecuperadaCommand request, CancellationToken cancellationToken)
        {
            var matriculaRecuperada = new CreateMatriculaRecuperadaCommonCommand
            {
                MatriculaIdIntegracion = request.MatriculaIdIntegracion,
                AlumnoIdIntegracion = request.AlumnoIdIntegracion,
                IdsAsignaturasOfertadas = request.IdsAsignaturasOfertadas,
                FechaHora = request.FechaHora,
                IsAmpliacion = false,
                Mensaje = request.Mensaje,
                Origen = request.Origen
            };
            await _mediator.Send(matriculaRecuperada, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
