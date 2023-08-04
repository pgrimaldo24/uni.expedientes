using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaAnuladaCommon;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionAnulada.Commands
{
    public class CreateMatriculaAmpliacionAnuladaCommandHandler : IRequestHandler<CreateMatriculaAmpliacionAnuladaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        public CreateMatriculaAmpliacionAnuladaCommandHandler(IExpedientesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateMatriculaAmpliacionAnuladaCommand request, CancellationToken cancellationToken)
        {
            var matriculaAnulada = new CreateMatriculaAnuladaCommonCommand
            {
                AlumnoIdIntegracion = request.AlumnoIdIntegracion,
                MatriculaIdIntegracion = request.MatriculaIdIntegracion,
                IdsAsignaturasOfertadas = request.IdsAsignaturasOfertadasAdicionadas.ToList(),
                IsAmpliacion = true,
                Mensaje = request.Mensaje,
                Origen = request.Origen
            };
            await _mediator.Send(matriculaAnulada, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
