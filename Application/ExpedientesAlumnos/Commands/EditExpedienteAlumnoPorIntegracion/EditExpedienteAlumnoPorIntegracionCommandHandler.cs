using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidateNodoExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.HasPrimeraMatriculaExpediente;
using Unir.Expedientes.Application.SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumnoPorIntegracion
{
    public class EditExpedienteAlumnoPorIntegracionCommandHandler : IRequestHandler<EditExpedienteAlumnoPorIntegracionCommand, EditExpedienteAlumnoPorIntegracionDto>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler> _localizer;
        private readonly IMediator _mediator;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public EditExpedienteAlumnoPorIntegracionCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<EditExpedienteAlumnoPorIntegracionCommandHandler> localizer,
            IMediator mediator,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _localizer = localizer;
            _mediator = mediator;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<EditExpedienteAlumnoPorIntegracionDto> Handle(EditExpedienteAlumnoPorIntegracionCommand request, CancellationToken cancellationToken)
        {
            ValidatePropiedadesRequeridas(request);
            var expedienteAlumno = await _context.ExpedientesAlumno
                .Include(ea => ea.TitulacionAcceso)
                .Include(ea => ea.ExpedientesEspecializaciones)
                .Include(ea => ea.ConsolidacionesRequisitosExpedientes)
                .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            AssignExpedienteAlumno(request, expedienteAlumno);

            var resultAssignVersionPlan = AssignVersionPlan(expedienteAlumno, request);
            if (resultAssignVersionPlan)
            {
                AddSeguimientoVersionPlan(expedienteAlumno, request);
                expedienteAlumno.IdRefVersionPlan = request.IdRefVersionPlan;
            }

            var resultAssignViaAcceso = AssignViaAcceso(expedienteAlumno, request);
            if (resultAssignViaAcceso)
            {
                AddSeguimientoViaAcceso(expedienteAlumno, request);
                expedienteAlumno.IdRefViaAccesoPlan = request.IdRefViaAccesoPlan;
            }
            
            var resultAddSeguimientoTitulacionAcceso = 
                await AddSeguimientoTitulacionAcceso(expedienteAlumno, request, cancellationToken);
            if (resultAddSeguimientoTitulacionAcceso)
            {
                AssignTitulacionAcceso(request.TitulacionAcceso, expedienteAlumno);
            }

            AddEspecializaciones(expedienteAlumno, request.Especializaciones);
            await AddSeguimientoExpediente(expedienteAlumno, request, cancellationToken);
            await _mediator.Send(new ValidateNodoExpedienteAlumnoCommand(expedienteAlumno), cancellationToken);
            await _mediator.Send(new AddConsolidacionesRequisitosExpedienteUncommitCommand(expedienteAlumno), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new EditExpedienteAlumnoPorIntegracionDto
            {
                Id = expedienteAlumno.Id,
                HasAddedSeguimientos = resultAssignVersionPlan || resultAssignViaAcceso || resultAddSeguimientoTitulacionAcceso
            };
        }

        protected internal virtual void AssignExpedienteAlumno(EditExpedienteAlumnoPorIntegracionCommand request, ExpedienteAlumno expedienteAlumno)
        {
            expedienteAlumno.IdRefNodo = request.IdRefNodo;
            expedienteAlumno.AlumnoNombre = request.AlumnoNombre;
            expedienteAlumno.AlumnoApellido1 = request.AlumnoApellido1;
            expedienteAlumno.AlumnoApellido2 = request.AlumnoApellido2;
            expedienteAlumno.IdRefTipoDocumentoIdentificacionPais = request.IdRefTipoDocumentoIdentificacionPais;
            expedienteAlumno.AlumnoNroDocIdentificacion = request.AlumnoNroDocIdentificacion;
            expedienteAlumno.AlumnoEmail = request.AlumnoEmail;
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
            expedienteAlumno.FechaApertura ??= request.FechaApertura;
            expedienteAlumno.FechaFinalizacion = request.FechaFinalizacion;
            expedienteAlumno.FechaTrabajoFinEstudio = request.FechaTrabajoFinEstudio;
            expedienteAlumno.TituloTrabajoFinEstudio = request.TituloTrabajoFinEstudio;
            expedienteAlumno.FechaExpedicion = request.FechaExpedicion;
            expedienteAlumno.NotaMedia = request.NotaMedia;
            expedienteAlumno.FechaPago = request.FechaPago;
        }

        protected internal virtual bool AssignVersionPlan(ExpedienteAlumno expedienteAlumno, 
            EditExpedienteAlumnoPorIntegracionCommand request)
        {
            var hasChangedVersionPlan = !string.IsNullOrWhiteSpace(request.IdRefVersionPlan) &&
                                        expedienteAlumno.IdRefVersionPlan != request.IdRefVersionPlan;
            return hasChangedVersionPlan;
        }

        protected internal virtual void ValidatePropiedadesRequeridas(EditExpedienteAlumnoPorIntegracionCommand request)
        {
            if (!string.IsNullOrWhiteSpace(request.IdRefVersionPlan))
            {
                if (!int.TryParse(request.IdRefVersionPlan, out var idVersionPlan))
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefVersionPlan)} tiene un valor inválido."]);
                if (idVersionPlan <= 0)
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefVersionPlan)} debe ser mayor a cero."]);
                if (!request.NroVersion.HasValue)
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.NroVersion)} es requerido para Editar el Expediente."]);
            }

            if (!string.IsNullOrWhiteSpace(request.IdRefNodo))
            {
                if (!int.TryParse(request.IdRefNodo, out var idNodo))
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefNodo)} tiene un valor inválido."]);
                if (idNodo <= 0)
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefNodo)} debe ser mayor a cero."]);
            }

            if (string.IsNullOrWhiteSpace(request.AlumnoNombre))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.AlumnoNombre)} es requerido para Editar el Expediente."]);

            if (string.IsNullOrWhiteSpace(request.AlumnoApellido1))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.AlumnoApellido1)} es requerido para Editar el Expediente."]);

            if (string.IsNullOrWhiteSpace(request.IdRefTipoDocumentoIdentificacionPais))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefTipoDocumentoIdentificacionPais)} es requerido para Editar el Expediente."]);

            if (string.IsNullOrWhiteSpace(request.AlumnoNroDocIdentificacion))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.AlumnoNroDocIdentificacion)} es requerido para Editar el Expediente."]);

            if (string.IsNullOrWhiteSpace(request.AlumnoEmail))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.AlumnoEmail)} es requerido para Editar el Expediente."]);
        }

        protected internal virtual void AssignTitulacionAcceso(TitulacionAccesoParametersDto requestTitulacionAcceso, ExpedienteAlumno expediente)
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

        protected internal virtual void AddEspecializaciones(ExpedienteAlumno expediente, List<ExpedienteEspecializacionDto> especializaciones)
        {
            expediente.DeleteEspecializacionesNoIncluidos(especializaciones?.Select(e => e.IdRefEspecializacion).ToArray());
            if (especializaciones == null) return;

            foreach (var especializacion in especializaciones.Where(especializacion =>
                         !expediente.HasEspecializacion(especializacion.IdRefEspecializacion)))
            {
                expediente.AddEspecializacion(especializacion.IdRefEspecializacion);
            }
        }

        protected internal virtual void AddSeguimientoVersionPlan(ExpedienteAlumno expedienteAlumno,
            EditExpedienteAlumnoPorIntegracionCommand request)
        {
            var descripcionSeguimiento = $"Versión de plan anterior con id.: {expedienteAlumno.IdRefVersionPlan}. Por Integración.";
            expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteModificadoVersionPlan,
            descripcionSeguimiento);
        }

        protected internal virtual bool AssignViaAcceso(ExpedienteAlumno expedienteAlumno,
            EditExpedienteAlumnoPorIntegracionCommand request)
        {
            var hasChangedViaAcceso = !string.IsNullOrEmpty(request.IdRefViaAccesoPlan) &&
                                      expedienteAlumno.IdRefViaAccesoPlan != request.IdRefViaAccesoPlan;
            return hasChangedViaAcceso;
        }

        protected internal virtual void AddSeguimientoViaAcceso(ExpedienteAlumno expedienteAlumno, EditExpedienteAlumnoPorIntegracionCommand request)
        {
            var descripcionSeguimiento = $"Vía de Acceso Plan anterior con id.: {expedienteAlumno.IdRefViaAccesoPlan}. Por Integración.";
            expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteModificadoViaAcceso,
            descripcionSeguimiento);
        }

        protected internal virtual async Task<bool> AddSeguimientoTitulacionAcceso(ExpedienteAlumno expedienteAlumno, 
            EditExpedienteAlumnoPorIntegracionCommand request, CancellationToken cancellationToken)
        {
            var requestTitulacionAcceso = request.TitulacionAcceso;
            if (requestTitulacionAcceso == null) return false;

            var assignTitulacionAcceso = await _mediator.Send(new AddSeguimientoTitulacionAccesoUncommitCommand(
                expedienteAlumno, true,
                new SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit.TitulacionAccesoParametersDto
                {
                    Titulo = requestTitulacionAcceso.Titulo,
                    TipoEstudio = requestTitulacionAcceso.TipoEstudio,
                    InstitucionDocente = requestTitulacionAcceso.InstitucionDocente,
                    IdRefTerritorioInstitucionDocente = requestTitulacionAcceso.IdRefTerritorioInstitucionDocente,
                    FechaInicioTitulo = requestTitulacionAcceso.FechaInicioTitulo,
                    FechafinTitulo = requestTitulacionAcceso.FechafinTitulo,
                    CodigoColegiadoProfesional = requestTitulacionAcceso.CodigoColegiadoProfesional,
                    NroSemestreRealizados = requestTitulacionAcceso.NroSemestreRealizados,
                    IdRefInstitucionDocente = requestTitulacionAcceso.IdRefInstitucionDocente
                }), cancellationToken);
            return assignTitulacionAcceso;
        }

        protected internal virtual async Task AddSeguimientoExpediente(ExpedienteAlumno expedienteAlumno,
            EditExpedienteAlumnoPorIntegracionCommand request, CancellationToken cancellationToken)
        {
            var esPrimeraMatricula = await _mediator.Send(new
                HasPrimeraMatriculaExpedienteQuery(request.IdIntegracionMatricula), cancellationToken);
            if (request.IdEstadoMatricula == EstadoMatriculaAcademicoModel.PREMATRICULA && esPrimeraMatricula)
            {
                expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteCreado,
                    $"Prematrícula primera matrícula: {request.IdIntegracionMatricula} (integración)");
                return;
            }

            expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteActualizado,
                $"Prematrícula nueva matrícula: {request.IdIntegracionMatricula} (integración)");
        }
    }
}
