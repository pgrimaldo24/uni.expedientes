using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditTitulacionAccesoExpedienteAlumno
{
    public class EditTitulacionAccesoExpedienteAlumnoCommandHandler : IRequestHandler<EditTitulacionAccesoExpedienteAlumnoCommand>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler> _localizer;
        private readonly IMediator _mediator;

        public EditTitulacionAccesoExpedienteAlumnoCommandHandler(IExpedientesContext context,
            IStringLocalizer<EditTitulacionAccesoExpedienteAlumnoCommandHandler> localizer,
            IMediator mediator)
        {
            _context = context;
            _localizer = localizer;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(EditTitulacionAccesoExpedienteAlumnoCommand request, CancellationToken cancellationToken)
        {
            if (!IsRequestValidaParaProcederConEdicion(request))
                return Unit.Value;

            ValidateDatos(request);

            var expedienteAlumno =
                await _context.ExpedientesAlumno
                    .Include(ea => ea.TitulacionAcceso)
                    .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            var resultAddSeguimientoTitulacionAcceso = 
                await AddSeguimientoTitulacionAcceso(expedienteAlumno, request, cancellationToken);
            if (!resultAddSeguimientoTitulacionAcceso) return Unit.Value;

            AssignTitulacionAcceso(request, expedienteAlumno);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual bool IsRequestValidaParaProcederConEdicion(EditTitulacionAccesoExpedienteAlumnoCommand request)
        {
            return !string.IsNullOrWhiteSpace(request.InstitucionDocente) || !string.IsNullOrWhiteSpace(request.Titulo);
        }

        protected internal virtual void ValidateDatos(EditTitulacionAccesoExpedienteAlumnoCommand request)
        {
            const string codeOtros = "-1";
            if (string.IsNullOrWhiteSpace(request.Titulo))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.Titulo)} es requerido para editar la Titulación de Acceso."]);
            if (string.IsNullOrWhiteSpace(request.InstitucionDocente) || string.IsNullOrWhiteSpace(request.IdRefInstitucionDocente))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.InstitucionDocente)} es requerido para editar la Titulación de Acceso."]);
            if (!string.IsNullOrEmpty(request.IdRefTerritorioInstitucionDocente) && request.IdRefInstitucionDocente != codeOtros &&
                request.IdRefInstitucionDocente.Split("-")[0] != request.IdRefTerritorioInstitucionDocente.Split("-")[0])
            {
                throw new BadRequestException(_localizer[$"El País de la Ubicación tiene que ser el mismo que el País de la Institución Docente."]);
            }

            if (request.FechaInicioTitulo.HasValue &&
                request.FechafinTitulo.HasValue &&
                request.FechaInicioTitulo.Value >
                request.FechafinTitulo.Value)
            {
                throw new BadRequestException(_localizer[
                    $"El campo {nameof(request.FechaInicioTitulo)} no puede ser mayor al campo {nameof(request.FechafinTitulo)}."]);
            }
        }

        protected internal virtual async Task<bool> AddSeguimientoTitulacionAcceso(ExpedienteAlumno expedienteAlumno,
            EditTitulacionAccesoExpedienteAlumnoCommand request, CancellationToken cancellationToken)
        {
            var assignTitulacionAcceso = await _mediator.Send(new AddSeguimientoTitulacionAccesoUncommitCommand(
                expedienteAlumno, true,
                new TitulacionAccesoParametersDto
                {
                    Titulo = request.Titulo,
                    TipoEstudio = request.TipoEstudio,
                    InstitucionDocente = request.InstitucionDocente,
                    IdRefTerritorioInstitucionDocente = request.IdRefTerritorioInstitucionDocente,
                    FechaInicioTitulo = request.FechaInicioTitulo,
                    FechafinTitulo = request.FechafinTitulo,
                    CodigoColegiadoProfesional = request.CodigoColegiadoProfesional,
                    NroSemestreRealizados = request.NroSemestreRealizados,
                    IdRefInstitucionDocente = request.IdRefInstitucionDocente
                }), cancellationToken);
            return assignTitulacionAcceso;
        }

        protected internal virtual void AssignTitulacionAcceso(EditTitulacionAccesoExpedienteAlumnoCommand requestTitulacionAcceso, 
            ExpedienteAlumno expediente)
        {
            expediente.TitulacionAcceso ??= new TitulacionAcceso();
            expediente.TitulacionAcceso.Titulo = requestTitulacionAcceso.Titulo;
            expediente.TitulacionAcceso.InstitucionDocente = requestTitulacionAcceso.InstitucionDocente;
            expediente.TitulacionAcceso.NroSemestreRealizados = requestTitulacionAcceso.NroSemestreRealizados;
            expediente.TitulacionAcceso.TipoEstudio = requestTitulacionAcceso.TipoEstudio;
            expediente.TitulacionAcceso.IdRefTerritorioInstitucionDocente = requestTitulacionAcceso.IdRefTerritorioInstitucionDocente;
            expediente.TitulacionAcceso.FechaInicioTitulo = requestTitulacionAcceso.FechaInicioTitulo;
            expediente.TitulacionAcceso.FechafinTitulo = requestTitulacionAcceso.FechafinTitulo;
            expediente.TitulacionAcceso.CodigoColegiadoProfesional = requestTitulacionAcceso.CodigoColegiadoProfesional;
            expediente.TitulacionAcceso.IdRefInstitucionDocente = requestTitulacionAcceso.IdRefInstitucionDocente;
        }
    }
}
