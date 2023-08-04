using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaDesestimadaCommon
{
    public class
        CreateMatriculaDesestimadaCommonCommandHandler : IRequestHandler<CreateMatriculaDesestimadaCommonCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;

        public CreateMatriculaDesestimadaCommonCommandHandler(IExpedientesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(CreateMatriculaDesestimadaCommonCommand request,
            CancellationToken cancellationToken)
        {
            var alumno = await _mediator.Send(new GetAlumnoByIdIntegracionQuery(request.AlumnoIdIntegracion,
                request.MatriculaIdIntegracion), cancellationToken);
            var expedienteAlumno = alumno.ExpedienteAlumno;
            var matriculaActual = alumno.MatriculaAcademicoModel;

            var tipoSituacionEstadoDesestimada = await _context.TiposSituacionEstado.FirstAsync(ts => ts.Id == TipoSituacionEstado.VariacionMatriculaDesestimada, cancellationToken);
            var tipoSeguimientoExpediente = TipoSeguimientoExpediente.ExpedienteActualizado;
            var hitoDesestimado = await _context.TiposHitoConseguidos.FirstAsync(hc => hc.Id == TipoHitoConseguido.Desestimado);
            var descripcionSeguimiento =
                $"Matrícula de variación desestimada por el motivo: {request.Motivo},{matriculaActual.Id}";

            if (request.EsAmpliacion)
            {
                tipoSituacionEstadoDesestimada = await _context.TiposSituacionEstado.FirstAsync(ts => ts.Id == TipoSituacionEstado.AmpliacionMatriculaDesestimada, cancellationToken);
                hitoDesestimado = await _context.TiposHitoConseguidos.FirstAsync(hc => hc.Id == TipoHitoConseguido.Desestimado);
                descripcionSeguimiento = $"Matrícula de ampliación desestimada por el motivo: {request.Motivo},{matriculaActual.Id}";
            }

            var existenOtrosActivos = alumno.AlumnoAcademicoModel.Matriculas.Any(x => x.Estado.EsActiva && x.Id != matriculaActual.Id);
            if (!existenOtrosActivos)
            {
                tipoSeguimientoExpediente = TipoSeguimientoExpediente.ExpedienteCerrado;
                expedienteAlumno.EstadoId = EstadoExpediente.Cerrado;
                expedienteAlumno.AddTipoSituacionEstadoExpediente(tipoSituacionEstadoDesestimada, request.FechaHora);
                expedienteAlumno.AddHitosConseguidos(hitoDesestimado, request.FechaHora);
            }
            else
                await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expedienteAlumno), cancellationToken);

            expedienteAlumno.AddSeguimientoNoUser(tipoSeguimientoExpediente, descripcionSeguimiento
                , request.FechaHora, request.Origen, request.Mensaje);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
