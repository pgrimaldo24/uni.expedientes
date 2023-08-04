using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ValidateNodoExpedienteAlumno;
using Unir.Expedientes.Application.SeguimientosExpedientes.Commands.AddSeguimientoTitulacionAccesoUncommit;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Security;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumno
{
    public class EditExpedienteAlumnoCommandHandler : IRequestHandler<EditExpedienteAlumnoCommand, int>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<EditExpedienteAlumnoCommandHandler> _localizer;
        private readonly IMediator _mediator;
        private readonly IIdentityService _identityService;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public EditExpedienteAlumnoCommandHandler(IExpedientesContext context,
            IStringLocalizer<EditExpedienteAlumnoCommandHandler> localizer, IMediator mediator,
            IIdentityService identityService, IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _localizer = localizer;
            _mediator = mediator;
            _identityService = identityService;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<int> Handle(EditExpedienteAlumnoCommand request, CancellationToken cancellationToken)
        {
            ValidatePropiedadesRequeridas(request);
            var expedienteAlumno =
                await _context.ExpedientesAlumno
                    .Include(ea => ea.TitulacionAcceso)
                    .Include(ea => ea.ExpedientesEspecializaciones)
                    .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);
            await AssignExpedienteAlumno(request, expedienteAlumno, cancellationToken);
            await _mediator.Send(new ValidateNodoExpedienteAlumnoCommand(expedienteAlumno), cancellationToken);
            await ModificarExpedienteAlumnoEnErp(request);
            await _context.SaveChangesAsync(cancellationToken);
            return expedienteAlumno.Id;
        }

        protected internal virtual async Task AssignExpedienteAlumno(EditExpedienteAlumnoCommand request, 
            ExpedienteAlumno expedienteAlumno, CancellationToken cancellationToken)
        {
            var assignVersionPlan = AddSeguimientoVersionPlan(expedienteAlumno, request);
            if (assignVersionPlan)
            {
                expedienteAlumno.IdRefVersionPlan = request.IdRefVersionPlan;
            }
            var idViaAccesoPlan = await AddSeguimientoViaAcceso(expedienteAlumno, request);
            if (!string.IsNullOrEmpty(idViaAccesoPlan))
            {
                expedienteAlumno.IdRefViaAccesoPlan = idViaAccesoPlan;
            }
            var assignTitulacionAcceso = await AddSeguimientoTitulacionAcceso(expedienteAlumno, request, cancellationToken);
            if (assignTitulacionAcceso)
            {
                AssignTitulacionAcceso(request.TitulacionAcceso, expedienteAlumno);
            }
        }

        protected internal virtual void ValidatePropiedadesRequeridas(EditExpedienteAlumnoCommand request)
        {
            const string codeOtros = "-1";
            if (!string.IsNullOrWhiteSpace(request.IdRefVersionPlan))
            {
                if (!int.TryParse(request.IdRefVersionPlan, out _))
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefVersionPlan)} tiene un valor inválido."]);
                if (!request.NroVersion.HasValue)
                    throw new BadRequestException(_localizer[$"El campo {nameof(request.NroVersion)} es requerido para Editar el Expediente."]);
            }

            if (!string.IsNullOrWhiteSpace(request.IdRefNodo) && !int.TryParse(request.IdRefNodo, out _))
                throw new BadRequestException(_localizer[$"El campo {nameof(request.IdRefNodo)} tiene un valor inválido."]);

            if (request.TitulacionAcceso == null) return;
            if (string.IsNullOrWhiteSpace(request.TitulacionAcceso.Titulo))
                throw new BadRequestException(_localizer[$"El campo Título es requerido para Editar el Expediente."]);
            if (string.IsNullOrWhiteSpace(request.TitulacionAcceso.InstitucionDocente) || string.IsNullOrWhiteSpace(request.TitulacionAcceso.IdRefInstitucionDocente))
                throw new BadRequestException(_localizer[$"El campo Institución Docente es requerido para Editar el Expediente."]);
            if (!string.IsNullOrEmpty(request.TitulacionAcceso.IdRefTerritorioInstitucionDocente) && request.TitulacionAcceso.IdRefInstitucionDocente != codeOtros &&
                request.TitulacionAcceso.IdRefInstitucionDocente.Split("-")[0] != request.TitulacionAcceso.IdRefTerritorioInstitucionDocente.Split("-")[0])
            {
                throw new BadRequestException(_localizer[$"El País de la Ubicación tiene que ser el mismo que el País de la Institución Docente."]);
            }

            if (request.TitulacionAcceso.FechaInicioTitulo.HasValue &&
                request.TitulacionAcceso.FechafinTitulo.HasValue &&
                request.TitulacionAcceso.FechaInicioTitulo.Value >
                request.TitulacionAcceso.FechafinTitulo.Value)
            {
                throw new BadRequestException(_localizer[
                    $"El campo Fecha Inicio del Título no puede ser mayor al campo de Fecha Fin del Título."]);
            }
        }

        protected internal virtual async Task ModificarExpedienteAlumnoEnErp(EditExpedienteAlumnoCommand request)
        {
            var parameters = new ExpedienteEditParameters
            {
                ViaAccesoIntegracion = !string.IsNullOrEmpty(request.IdRefViaAcceso)
                    ? new ViaAccesoParameters
                    {
                        Id = Convert.ToInt32(request.IdRefViaAcceso)
                    }
                    : null
            };
            await _erpAcademicoServiceClient.ModifyExpedienteByIdIntegracion(
                request.IdExpedienteAlumno.ToString(), parameters);
        }

        protected internal virtual void AssignTitulacionAcceso(TitulacionAccesoParametersDto requestTitulacionAcceso, 
            ExpedienteAlumno expediente)
        {
            expediente.TitulacionAcceso ??= new TitulacionAcceso();
            expediente.TitulacionAcceso.Titulo = requestTitulacionAcceso.Titulo;
            expediente.TitulacionAcceso.TipoEstudio = requestTitulacionAcceso.TipoEstudio;
            expediente.TitulacionAcceso.IdRefInstitucionDocente = requestTitulacionAcceso.IdRefInstitucionDocente;
            expediente.TitulacionAcceso.InstitucionDocente = requestTitulacionAcceso.InstitucionDocente;
            expediente.TitulacionAcceso.IdRefTerritorioInstitucionDocente = requestTitulacionAcceso.IdRefTerritorioInstitucionDocente;
            expediente.TitulacionAcceso.FechaInicioTitulo = requestTitulacionAcceso.FechaInicioTitulo;
            expediente.TitulacionAcceso.FechafinTitulo = requestTitulacionAcceso.FechafinTitulo;
            expediente.TitulacionAcceso.NroSemestreRealizados = requestTitulacionAcceso.NroSemestreRealizados;
            expediente.TitulacionAcceso.CodigoColegiadoProfesional = requestTitulacionAcceso.CodigoColegiadoProfesional;
        }

        protected internal virtual bool AddSeguimientoVersionPlan(ExpedienteAlumno expedienteAlumno,
            EditExpedienteAlumnoCommand request)
        {
            if (expedienteAlumno.IdRefVersionPlan == request.IdRefVersionPlan) return false;
            var descripcionSeguimiento = $"Versión de plan anterior con id.: {expedienteAlumno.IdRefVersionPlan}";
            expedienteAlumno.AddSeguimiento(TipoSeguimientoExpediente.ExpedienteModificadoVersionPlan,
                    _identityService.GetUserIdentityInfo().Id, descripcionSeguimiento);
            return true;
        }

        protected internal virtual async Task<string> AddSeguimientoViaAcceso(ExpedienteAlumno expedienteAlumno, 
            EditExpedienteAlumnoCommand request)
        {
            if (string.IsNullOrEmpty(request.IdRefViaAcceso)) return null;

            var parameters = new ViaAccesoPlanListParameters
            {
                FilterIdViaAcceso = request.IdRefViaAcceso,
                FilterIdNodo = !string.IsNullOrEmpty(request.IdRefNodo) ? Convert.ToInt32(request.IdRefNodo) : null,
                NoPaged = true,
                Count = 1,
                Index = 1
            };

            var viasAccesosPlanes = await _erpAcademicoServiceClient.GetViasAccesosPlanes(parameters);
            if (viasAccesosPlanes == null || !viasAccesosPlanes.Any()) return null;

            var viaAccesoPlan = viasAccesosPlanes.First();
            if (expedienteAlumno.IdRefViaAccesoPlan == viaAccesoPlan.Id.ToString()) return null;

            var descripcionSeguimiento = $"Vía de Acceso Plan anterior con id.: {expedienteAlumno.IdRefViaAccesoPlan}";
            expedienteAlumno.AddSeguimiento(TipoSeguimientoExpediente.ExpedienteModificadoViaAcceso,
                    _identityService.GetUserIdentityInfo().Id, descripcionSeguimiento);
            return viaAccesoPlan.Id.ToString();
        }

        protected internal virtual async Task<bool> AddSeguimientoTitulacionAcceso(ExpedienteAlumno expedienteAlumno,
            EditExpedienteAlumnoCommand request, CancellationToken cancellationToken)
        {
            var requestTitulacionAcceso = request.TitulacionAcceso;
            if (requestTitulacionAcceso == null) return false;

            var assignTitulacionAcceso = await _mediator.Send(new AddSeguimientoTitulacionAccesoUncommitCommand(
                expedienteAlumno, false,
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
    }
}
