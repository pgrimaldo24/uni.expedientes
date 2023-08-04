using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using System.Linq;
using Unir.Expedientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaVariacionAnulada.Commands
{
    public class CreateMatriculaVariacionAnuladaCommandHandler : IRequestHandler<CreateMatriculaVariacionAnuladaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        public CreateMatriculaVariacionAnuladaCommandHandler(IExpedientesContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }
        public async Task<Unit> Handle(CreateMatriculaVariacionAnuladaCommand request, CancellationToken cancellationToken)
        {
            var alumno = await _mediator.Send(new GetAlumnoByIdIntegracionQuery(request.AlumnoIdIntegracion,
                    request.MatriculaIdIntegracion), cancellationToken);
            var alumnoAcademico = alumno.AlumnoAcademicoModel;
            var expedienteAlumno = alumno.ExpedienteAlumno;
            var matricula = alumno.MatriculaAcademicoModel;
            if (matricula.AsignaturaMatriculadas.Any())
            {
                var idsAsignaturasPlanAdicionadas = matricula.AsignaturaMatriculadas
                    .Where(am => request.IdsAsignaturasOfertadasAdicionadas.Contains(am.AsignaturaOfertada.Id))
                    .Select(a => a.AsignaturaOfertada.AsignaturaPlan.Id).ToList();

                var idsAsignaturasPlanExcluidos = matricula.AsignaturaMatriculadas
                    .Where(am => request.IdsAsignaturasOfertadasExcluidas.Contains(am.AsignaturaOfertada.Id))
                    .Select(a => a.AsignaturaOfertada.AsignaturaPlan.Id).ToList();

                expedienteAlumno.CambiarSituacionAsignaturas(idsAsignaturasPlanAdicionadas, SituacionAsignatura.Anulada);
                expedienteAlumno.CambiarSituacionAsignaturas(idsAsignaturasPlanExcluidos, SituacionAsignatura.Matriculada);
            }

            var tipoSeguimientoExpediente = TipoSeguimientoExpediente.ExpedienteActualizado;
            var existenOtrosMatriculasActivos = alumnoAcademico.Matriculas.Any(x => x.Estado.EsActiva && x.Id != matricula.Id);
            if (!existenOtrosMatriculasActivos)
            {
                var hitoAnulado = await _context.TiposHitoConseguidos.FirstAsync(hc => hc.Id == TipoHitoConseguido.Anulado);
                tipoSeguimientoExpediente = TipoSeguimientoExpediente.ExpedienteCerrado;
                expedienteAlumno.EstadoId = EstadoExpediente.Cerrado;
                expedienteAlumno.AddHitosConseguidos(hitoAnulado, request.FechaHora);
            }
            else
            {
                expedienteAlumno.EstadoId = EstadoExpediente.Abierto;
                await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expedienteAlumno), cancellationToken);
            }

            var tipoSituacion = await _context.TiposSituacionEstado.FirstAsync(ts => ts.Id == TipoSituacionEstado.VariacionMatriculaAnulada, cancellationToken);
            expedienteAlumno.AddTipoSituacionEstadoExpediente(tipoSituacion, request.FechaHora);
            expedienteAlumno.AddSeguimientoNoUser(tipoSeguimientoExpediente, $"Varia'ción de matrícula anulada: {matricula.Id}",
                request.FechaHora, request.Origen, request.Mensaje);

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
