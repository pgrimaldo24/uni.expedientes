using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaReiniciadaCommon
{
    public class CreateMatriculaReiniciadaCommonCommandHandler : IRequestHandler<CreateMatriculaReiniciadaCommonCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        public CreateMatriculaReiniciadaCommonCommandHandler(IExpedientesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(CreateMatriculaReiniciadaCommonCommand request, CancellationToken cancellationToken)
        {
            var alumno = await _mediator.Send(new GetAlumnoByIdIntegracionQuery(request.AlumnoIdIntegracion,
                request.MatriculaIdIntegracion), cancellationToken);
            var expedienteAlumno = alumno.ExpedienteAlumno;
            var alumnoAcademico = alumno.AlumnoAcademicoModel;
            var matriculaActual = alumno.MatriculaAcademicoModel;

            var existenOtrosActivos = alumnoAcademico.Matriculas.Any(x => x.Estado.EsActiva && x.Id != matriculaActual.Id);
            if (!existenOtrosActivos)
            {
                var tipoSituacion = await _context.TiposSituacionEstado.FirstAsync(ts => ts.Id == request.TipoSituacionId, cancellationToken);
                expedienteAlumno.EstadoId = EstadoExpediente.Inicial;
                expedienteAlumno.FechaFinalizacion = null;
                expedienteAlumno.AddTipoSituacionEstadoExpediente(tipoSituacion, matriculaActual.FechaCambioEstado);
            }

            var hitoDesestimado = await _context.HitosConseguidos.OrderByDescending(hc => hc.FechaInicio)
                .FirstOrDefaultAsync(hc => hc.TipoConseguidoId == TipoHitoConseguido.Desestimado && hc.ExpedienteAlumnoId == expedienteAlumno.Id);
            if (hitoDesestimado != null)
                _context.HitosConseguidos.Remove(hitoDesestimado);

            expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteActualizado,
                string.Format(request.MensajeSeguimiento, matriculaActual.Id.ToString()), request.FechaHora, request.Origen, request.Mensaje);

            await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expedienteAlumno), cancellationToken);
            return Unit.Value;
        }
    }
}
