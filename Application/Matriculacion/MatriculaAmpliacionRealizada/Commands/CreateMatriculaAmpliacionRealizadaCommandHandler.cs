using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionRealizada.Commands
{
    public class CreateMatriculaAmpliacionRealizadaCommandHandler : IRequestHandler<CreateMatriculaAmpliacionRealizadaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IMediator _mediator;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        public CreateMatriculaAmpliacionRealizadaCommandHandler(IExpedientesContext context,
            IMediator mediator,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _mediator = mediator;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }
        public async Task<Unit> Handle(CreateMatriculaAmpliacionRealizadaCommand request, CancellationToken cancellationToken)
        {
            var alumno = await _mediator.Send(new GetAlumnoByIdIntegracionQuery(request.AlumnoIdIntegracion,
                request.MatriculaIdIntegracion), cancellationToken);
            var expedienteAlumno = alumno.ExpedienteAlumno;
            var alumnoAcademico = alumno.AlumnoAcademicoModel;
            var matricula = alumno.MatriculaAcademicoModel;
            var asignaturasErp = await _erpAcademicoServiceClient.GetAsignaturasMatricula(matricula.Id);

            var tipoSituacionAmpliacionMatricula = await _context.TiposSituacionEstado
                .FirstAsync(ts => ts.Id == TipoSituacionEstado.AmpliacionMatricula, cancellationToken);
            var existenOtrosAlta = alumnoAcademico.Matriculas.Any(x => x.Estado.EsAlta && x.Id != matricula.Id);
            if (!existenOtrosAlta)
            {
                expedienteAlumno.EstadoId = EstadoExpediente.Abierto;
                expedienteAlumno.AddTipoSituacionEstadoExpediente(tipoSituacionAmpliacionMatricula, request.FechaHoraAlta);
            }

            var asignaturasAsociar = asignaturasErp
                .Where(a => request.IdsAsignaturasOfertadasAdicionadas.Contains(a.AsignaturaOfertada.Id))
                .Select(ao => ao.AsignaturaOfertada)
                .ToList();

            var asignaturasAgregar = await _mediator.Send(new GetAsignaturasAsociadasQuery(expedienteAlumno.AsignaturasExpedientes, 
                asignaturasAsociar, SituacionAsignatura.Matriculada), cancellationToken);
            if (asignaturasAgregar.Any())
                expedienteAlumno.AsignaturasExpedientes.AddRange(asignaturasAgregar);

            expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteActualizado,
                    $"Ampliación de matrícula:{matricula.Id}", request.FechaHoraAlta, request.Origen, request.Mensaje);
            await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expedienteAlumno), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
