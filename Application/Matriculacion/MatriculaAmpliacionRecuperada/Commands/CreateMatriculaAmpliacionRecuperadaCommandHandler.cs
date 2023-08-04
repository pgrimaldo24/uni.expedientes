using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaRecuperadaCommon;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionRecuperada.Commands
{
    public class CreateMatriculaAmpliacionRecuperadaCommandHandler : IRequestHandler<CreateMatriculaAmpliacionRecuperadaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        public CreateMatriculaAmpliacionRecuperadaCommandHandler(IExpedientesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(CreateMatriculaAmpliacionRecuperadaCommand request, CancellationToken cancellationToken)
        {
            var matriculaRecuperada = new CreateMatriculaRecuperadaCommonCommand
            {
                MatriculaIdIntegracion = request.MatriculaIdIntegracion,
                AlumnoIdIntegracion = request.AlumnoIdIntegracion,
                IdsAsignaturasOfertadas = request.IdsAsignaturasOfertadasAdicionadas.ToList(),
                FechaHora = request.FechaHora,
                IsAmpliacion = true,
                Mensaje = request.Mensaje,
                Origen = request.Origen
            };
            await _mediator.Send(matriculaRecuperada, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
