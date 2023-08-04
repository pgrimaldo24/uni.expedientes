using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaRecuperadaCommon;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaVariacionRecuperada
{
    public class CreateMatriculaVariacionRecuperadaCommandHandler : IRequestHandler<CreateMatriculaVariacionRecuperadaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        public CreateMatriculaVariacionRecuperadaCommandHandler(IExpedientesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(CreateMatriculaVariacionRecuperadaCommand request, CancellationToken cancellationToken)
        {
            var matriculaRecuperada = new CreateMatriculaRecuperadaCommonCommand
            {
                MatriculaIdIntegracion = request.MatriculaIdIntegracion,
                AlumnoIdIntegracion = request.AlumnoIdIntegracion,
                IdsAsignaturasOfertadas = request.IdsAsignaturasOfertadasAdicionadas.ToList(),
                IdsAsignaturasExcluidas = request.IdsAsignaturasOfertadasExcluidas.ToList(),
                FechaHora = request.FechaHora,
                EsVariacion = true,
                Mensaje = request.Mensaje,
                Origen = request.Origen
            };
            await _mediator.Send(matriculaRecuperada, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
