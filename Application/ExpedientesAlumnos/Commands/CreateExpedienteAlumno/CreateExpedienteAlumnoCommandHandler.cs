using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidateNodoExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.HasPrimeraMatriculaExpediente;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.CreateExpedienteAlumno
{
    public class CreateExpedienteAlumnoCommandHandler : IRequestHandler<CreateExpedienteAlumnoCommand, int>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<CreateExpedienteAlumnoCommandHandler> _localizer;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateExpedienteAlumnoCommandHandler> _logger;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public CreateExpedienteAlumnoCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<CreateExpedienteAlumnoCommandHandler> localizer, 
            IMediator mediator,
            ILogger<CreateExpedienteAlumnoCommandHandler> logger,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _localizer = localizer;
            _mediator = mediator;
            _logger = logger;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<int> Handle(CreateExpedienteAlumnoCommand request, CancellationToken cancellationToken)
        {
            ValidatePropiedadesRequeridas(request);
            var expedienteExistente = await _context.ExpedientesAlumno
                .Include(ea => ea.TitulacionAcceso)
                .Include(ea => ea.ExpedientesEspecializaciones)
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .FirstOrDefaultAsync(ea => ea.IdRefIntegracionAlumno == request.IdRefIntegracionAlumno 
                    && ea.IdRefPlan == request.IdRefPlan, cancellationToken);

            var expediente = expedienteExistente ?? new ExpedienteAlumno();
            AssignExpedienteAlumno(request, expediente);
            AssignTitulacionAcceso(request.TitulacionAcceso, expediente);
            await _mediator.Send(new ValidateNodoExpedienteAlumnoCommand(expediente), cancellationToken);

            var idTipoSeguimiento = expedienteExistente != null
                ? TipoSeguimientoExpediente.ExpedienteModificadoVersionPlan
                : TipoSeguimientoExpediente.ExpedienteCreado;
            await AddNewExpediente(expediente, idTipoSeguimiento, cancellationToken);
            await AddSeguimientoExpediente(expediente, request, idTipoSeguimiento, cancellationToken);
            AddEspecializaciones(expediente, request.Especializaciones);
            await AssignExpedienteAlumnoRelacionado(request, expediente, cancellationToken);
            await AddHitosConseguidos(request.FechaHoraMensaje, expediente, cancellationToken);
            await AddTipoSituacionEstadoExpediente(request.FechaHoraMensaje, expediente, cancellationToken);
            await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expediente), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogWarning("DateTimeNowUtc {0}, App {1}, Command {2}, IdExpedienteAlumno {3}", DateTime.UtcNow,
                "expedienteserp", nameof(CreateExpedienteAlumnoCommandHandler), expediente.Id);
            return expediente.Id;
        }

        protected internal virtual async Task AssignExpedienteAlumnoRelacionado(CreateExpedienteAlumnoCommand request,
            ExpedienteAlumno expedienteAlumno, CancellationToken cancellationToken)
        {
            if (request.IdsPlanes == null) return;
            var tipoRelacionExpediente = await _context.TiposRelacionesExpediente.FirstAsync(e => 
                                                e.Id == TipoRelacionExpediente.CambioPlan, cancellationToken);

            foreach (var idPlan in request.IdsPlanes)
            {
                var expedienteRelacionado = await _context.ExpedientesAlumno.FirstOrDefaultAsync(e => 
                        e.IdRefIntegracionAlumno == request.IdRefIntegracionAlumno && e.IdRefPlan == idPlan.ToString(), 
                        cancellationToken);
                if (expedienteRelacionado == null)
                    continue;

                await _context.RelacionesExpedientes.AddAsync(new RelacionExpediente
                {
                    ExpedienteAlumnoRelacionado = expedienteRelacionado,
                    TipoRelacion = tipoRelacionExpediente,
                    ExpedienteAlumno = expedienteAlumno
                }, cancellationToken);
                return;
            }
        }

        protected internal virtual void ValidatePropiedadesRequeridas(CreateExpedienteAlumnoCommand request)
        {
            if (string.IsNullOrWhiteSpace(request.IdRefIntegracionAlumno))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefIntegracionAlumno)} es requerido para Crear el Expediente."]);
            if (!int.TryParse(request.IdRefIntegracionAlumno, out var idIntegracionAlumno))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefIntegracionAlumno)} tiene un valor inválido."]);
            if (idIntegracionAlumno <= 0)
                throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefIntegracionAlumno)} debe ser mayor a cero."]);

            if (string.IsNullOrWhiteSpace(request.IdRefPlan))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefPlan)} es requerido para Crear el Expediente."]);
            if (!int.TryParse(request.IdRefPlan, out var idPlan))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefPlan)} tiene un valor inválido."]);
            if (idPlan <= 0)
                throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefPlan)} debe ser mayor a cero."]);

            if (!string.IsNullOrWhiteSpace(request.IdRefVersionPlan))
            {
                if (!int.TryParse(request.IdRefVersionPlan, out var idVersionPlan))
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefVersionPlan)} tiene un valor inválido."]);
                if (idVersionPlan <= 0)
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefVersionPlan)} debe ser mayor a cero."]);
                if (!request.NroVersion.HasValue)
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.NroVersion)} es requerido para Crear el Expediente."]);
            }

            if (!string.IsNullOrWhiteSpace(request.IdRefNodo))
            {
                if (!int.TryParse(request.IdRefNodo, out var idNodo))
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefNodo)} tiene un valor inválido."]);
                if (idNodo <= 0)
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefNodo)} debe ser mayor a cero."]);
            }

            if (string.IsNullOrWhiteSpace(request.AlumnoNombre))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.AlumnoNombre)} es requerido para Crear el Expediente."]);

            if (string.IsNullOrWhiteSpace(request.AlumnoApellido1))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.AlumnoApellido1)} es requerido para Crear el Expediente."]);

            if (string.IsNullOrWhiteSpace(request.IdRefTipoDocumentoIdentificacionPais))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefTipoDocumentoIdentificacionPais)} es requerido para Crear el Expediente."]);

            if (string.IsNullOrWhiteSpace(request.AlumnoNroDocIdentificacion))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.AlumnoNroDocIdentificacion)} es requerido para Crear el Expediente."]);

            if (string.IsNullOrWhiteSpace(request.AlumnoEmail))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.AlumnoEmail)} es requerido para Crear el Expediente."]);
        }

        protected internal virtual void AssignExpedienteAlumno(CreateExpedienteAlumnoCommand request,
            ExpedienteAlumno expedienteAlumno)
        {
            expedienteAlumno.IdRefIntegracionAlumno = request.IdRefIntegracionAlumno;
            expedienteAlumno.IdRefPlan = request.IdRefPlan;
            expedienteAlumno.IdRefVersionPlan =
                string.IsNullOrWhiteSpace(request.IdRefVersionPlan) ? null : request.IdRefVersionPlan;
            expedienteAlumno.IdRefNodo = string.IsNullOrWhiteSpace(request.IdRefNodo) ? null : request.IdRefNodo;
            expedienteAlumno.AlumnoNombre = request.AlumnoNombre;
            expedienteAlumno.AlumnoApellido1 = request.AlumnoApellido1;
            expedienteAlumno.AlumnoApellido2 = request.AlumnoApellido2;
            expedienteAlumno.IdRefTipoDocumentoIdentificacionPais = request.IdRefTipoDocumentoIdentificacionPais;
            expedienteAlumno.AlumnoNroDocIdentificacion = request.AlumnoNroDocIdentificacion;
            expedienteAlumno.AlumnoEmail = request.AlumnoEmail;
            expedienteAlumno.IdRefViaAccesoPlan = request.IdRefViaAccesoPlan;
            expedienteAlumno.DocAcreditativoViaAcceso = request.DocAcreditativoViaAcceso;
            expedienteAlumno.IdRefIntegracionDocViaAcceso = request.IdRefIntegracionDocViaAcceso;
            expedienteAlumno.FechaSubidaDocViaAcceso = request.FechaSubidaDocViaAcceso;
            expedienteAlumno.NombrePlan = request.NombrePlan;
            expedienteAlumno.IdRefUniversidad = request.IdRefUniversidad;
            expedienteAlumno.AcronimoUniversidad = request.AcronimoUniversidad;
            expedienteAlumno.IdRefCentro = request.IdRefCentro;
            expedienteAlumno.IdRefAreaAcademica = request.IdRefAreaAcademica;
            expedienteAlumno.IdRefTipoEstudio = request.IdRefTipoEstudio;
            expedienteAlumno.IdRefEstudio = request.IdRefEstudio;
            expedienteAlumno.NombreEstudio = request.NombreEstudio;
            expedienteAlumno.IdRefTitulo = request.IdRefTitulo;
            expedienteAlumno.FechaApertura = request.FechaApertura;
            expedienteAlumno.FechaFinalizacion = request.FechaFinalizacion;
            expedienteAlumno.FechaTrabajoFinEstudio = request.FechaTrabajoFinEstudio;
            expedienteAlumno.TituloTrabajoFinEstudio = request.TituloTrabajoFinEstudio;
            expedienteAlumno.FechaExpedicion = request.FechaExpedicion;
            expedienteAlumno.NotaMedia = request.NotaMedia;
            expedienteAlumno.FechaPago = request.FechaPago;
            expedienteAlumno.EstadoId = EstadoExpediente.Inicial;
        }

        protected internal virtual async Task AddSeguimientoExpediente(ExpedienteAlumno expedienteAlumno,
            CreateExpedienteAlumnoCommand request, int idTipoSeguimiento, CancellationToken cancellationToken)
        {
            var nroVersion = request.NroVersion.HasValue ? request.NroVersion.Value.ToString() : "-";
            var descripcion = $"Versión del Plan: {nroVersion}.";
            expedienteAlumno.AddSeguimientoNoUser(idTipoSeguimiento, descripcion);

            var esPrimeraMatricula = await _mediator.Send(new 
                HasPrimeraMatriculaExpedienteQuery(request.IdIntegracionMatricula), cancellationToken);
            if (request.IdEstadoMatricula == EstadoMatriculaAcademicoModel.PREMATRICULA && esPrimeraMatricula)
            {
                expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteCreado, 
                    $"Prematrícula primera matrícula: {request.IdIntegracionMatricula} (integración)");
            }
        }

        protected internal virtual async Task AddNewExpediente(ExpedienteAlumno expedienteAlumno, int idTipoSeguimiento,
            CancellationToken cancellationToken)
        {
            if (idTipoSeguimiento == TipoSeguimientoExpediente.ExpedienteCreado)
            {
                await _context.ExpedientesAlumno.AddAsync(expedienteAlumno, cancellationToken);
            }
        }

        protected internal virtual void AssignTitulacionAcceso(TitulacionAccesoDto requestTitulacionAcceso, ExpedienteAlumno expediente)
        {
            if (requestTitulacionAcceso == null) return;

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

        protected internal virtual void AddEspecializaciones(ExpedienteAlumno expediente, List<ExpedienteEspecializacionDto> especializaciones)
        {
            if (especializaciones == null) return;

            foreach (var especializacion in especializaciones.Where(especializacion =>
                !expediente.HasEspecializacion(especializacion.IdRefEspecializacion)))
            {
                expediente.AddEspecializacion(especializacion.IdRefEspecializacion);
            }
        }

        protected internal virtual async Task AddHitosConseguidos(DateTime? fechaInicio, 
            ExpedienteAlumno expediente, CancellationToken cancellationToken)
        {
            if (fechaInicio == null) return;
            var tipoHitoPrematriculado = await _context.TiposHitoConseguidos
                .FirstAsync(t => t.Id == TipoHitoConseguido.PreMatriculado, cancellationToken);
            expediente.AddHitosConseguidos(tipoHitoPrematriculado, fechaInicio.Value);
        }

        protected internal virtual async Task AddTipoSituacionEstadoExpediente(DateTime? fechaInicio,
            ExpedienteAlumno expediente, CancellationToken cancellationToken)
        {
            if (fechaInicio == null) return;
            var tipoSituacionEstado = await _context.TiposSituacionEstado
                .FirstAsync(t => t.Id == TipoSituacionEstado.PreMatriculado, cancellationToken);
            expediente.AddTipoSituacionEstadoExpediente(tipoSituacionEstado, fechaInicio.Value);
        }
    }
}
