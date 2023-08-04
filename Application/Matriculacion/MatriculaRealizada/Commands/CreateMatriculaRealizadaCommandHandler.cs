using System.Linq;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.HasPrimeraMatriculaExpediente;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAlumnoByIdIntegracion;
using Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaRealizada.Commands
{
    public class CreateMatriculaRealizadaCommandHandler : IRequestHandler<CreateMatriculaRealizadaCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly IStringLocalizer<CreateMatriculaRealizadaCommandHandler> _localizer;
        private readonly IMediator _mediator;
        public CreateMatriculaRealizadaCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<CreateMatriculaRealizadaCommandHandler> localizer,
            IMediator mediator,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _localizer = localizer;
            _mediator = mediator;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }
        public async Task<Unit> Handle(CreateMatriculaRealizadaCommand request, CancellationToken cancellationToken)
        {
            var alumno = await _mediator.Send(new GetAlumnoByIdIntegracionQuery(request.AlumnoIdIntegracion,
                request.MatriculaIdIntegracion), cancellationToken);
            var expedienteAlumno = alumno.ExpedienteAlumno;
            var matricula = alumno.MatriculaAcademicoModel;
            var asignaturasErp = await _erpAcademicoServiceClient.GetAsignaturasMatricula(matricula.Id);

            var esPrimeraMatricula = await _mediator.Send(new
                HasPrimeraMatriculaExpedienteQuery(request.MatriculaIdIntegracion), cancellationToken);

            var descripcionSeguimiento = "Nueva";
            if (esPrimeraMatricula)
            {
                var tipoHitoConseguido = new TipoHitoConseguido { Id = TipoHitoConseguido.PrimeraMatricula, Nombre = HitoConseguido.PrimeraMatricula };
                expedienteAlumno.FechaApertura = request.FechaRecepcion;
                expedienteAlumno.AddHitosConseguidos(tipoHitoConseguido, expedienteAlumno.FechaApertura.Value);
                descripcionSeguimiento = "Primera";
            }

            expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteActualizado,
            $"{descripcionSeguimiento} matrícula: {matricula.Id}", request.FechaRecepcion, request.Origen, request.Mensaje);

            var asignaturasAgregar = await _mediator.Send(new GetAsignaturasAsociadasQuery(expedienteAlumno.AsignaturasExpedientes, 
                asignaturasErp.Select(am => am.AsignaturaOfertada).ToList(), SituacionAsignatura.Matriculada), cancellationToken);

            if (asignaturasAgregar.Any())
                expedienteAlumno.AsignaturasExpedientes.AddRange(asignaturasAgregar);

            await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expedienteAlumno), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
