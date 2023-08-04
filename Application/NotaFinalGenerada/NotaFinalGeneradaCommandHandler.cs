using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Unir.Expedientes.Application.AsignaturasCalificacion.Commands.CreateAsignaturaCalificacion;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos;

namespace Unir.Expedientes.Application.NotaFinalGenerada
{
    public class NotaFinalGeneradaCommandHandler : IRequestHandler<NotaFinalGeneradaCommand>
    {
        private readonly IMediator _mediator;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly IEvaluacionesServiceClient _evaluacionesServiceClient;

        public NotaFinalGeneradaCommandHandler(IMediator mediator, IErpAcademicoServiceClient erpAcademicoServiceClient, IEvaluacionesServiceClient evaluacionesServiceClient)
        {
            _mediator = mediator;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
            _evaluacionesServiceClient = evaluacionesServiceClient;
        }

        public async Task<Unit> Handle(NotaFinalGeneradaCommand request,
            CancellationToken cancellationToken)
        {
            var idsRefsAlumnos = request.Notas.Select(n => n.IdAlumno.ToString()).ToList();

            var asignaturasMatriculadas =
                await _erpAcademicoServiceClient.GetAsignaturasMatriculadasParaNotaFinal(request.IdCurso,
                    idsRefsAlumnos);

            if (asignaturasMatriculadas is null  || !asignaturasMatriculadas.Any())
            {
                return Unit.Value;
            }

            var expedientes = await _mediator.Send(new GetAllExpedientesAlumnosQuery
                { FilterIdsRefIntegracionAlumnos = idsRefsAlumnos });

            if (!expedientes.Any())
            {
                return Unit.Value;
            }

            var idsRefsAsignaturasPlan = expedientes
                .SelectMany(e => e.AsignaturasExpedientes.Select(ae => ae.IdRefAsignaturaPlan)).ToList();

            var asignaturasMatriculadasExistentesEnExpedientes = asignaturasMatriculadas
                .Where(ae => idsRefsAsignaturasPlan.Contains(ae.AsignaturaOfertada.AsignaturaPlan.Id.ToString())).ToList();

            if (!asignaturasMatriculadasExistentesEnExpedientes.Any())
            {
                return Unit.Value;
            }

            foreach (var asignaturaMatriculada in asignaturasMatriculadasExistentesEnExpedientes)
            {
                var configuracionVersionEscala = await
                    _evaluacionesServiceClient.GetConfiguracionEscalaFromNivelesAsociadosEscalas(asignaturaMatriculada
                        .AsignaturaOfertada.Id);

                if (configuracionVersionEscala != null)
                {
                    asignaturaMatriculada.ConfiguracionVersionEscala = configuracionVersionEscala;
                }
            }

            await _mediator.Send(new CreateAsignaturaCalificacionCommand
            {
                Expedientes = expedientes.ToList(),
                asignaturasMatriculadas = asignaturasMatriculadasExistentesEnExpedientes,
                notaFinal = request
            });
            return Unit.Value;
        }
    }
}