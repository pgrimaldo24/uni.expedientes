using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaAnuladaCommon;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaAnulada.Commands
{
    public class CreateMatriculaAnuladaCommandHandler : IRequestHandler<CreateMatriculaAnuladaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;

        public CreateMatriculaAnuladaCommandHandler(IExpedientesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateMatriculaAnuladaCommand request, CancellationToken cancellationToken)
        {
            var matriculaAnulada = new CreateMatriculaAnuladaCommonCommand
            {
                AlumnoIdIntegracion = request.AlumnoIdIntegracion,
                MatriculaIdIntegracion = request.MatriculaIdIntegracion,
                IdCausaBaja = request.IdCausaBaja,
                IdTipoBaja = request.IdTipoBaja,
                IdsAsignaturasOfertadas = request.IdsAsignaturasOfertadas,
                IsAmpliacion = false,
                Mensaje = request.Mensaje,
                Origen = request.Origen
            };
            await _mediator.Send(matriculaAnulada, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
