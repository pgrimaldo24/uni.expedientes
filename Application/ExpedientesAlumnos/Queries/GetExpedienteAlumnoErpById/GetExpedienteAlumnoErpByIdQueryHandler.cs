using System;
using MediatR;
using Microsoft.Extensions.Localization;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoById;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoErpById
{
    public class GetExpedienteAlumnoErpByIdQueryHandler : IRequestHandler<GetExpedienteAlumnoErpByIdQuery, ExpedienteAcademicoModel>
    {
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
        private readonly IStringLocalizer<GetExpedienteAlumnoErpByIdQueryHandler> _localizer;
        private readonly IMediator _mediator;

        public GetExpedienteAlumnoErpByIdQueryHandler(IErpAcademicoServiceClient erpAcademicoServiceClient,
            IStringLocalizer<GetExpedienteAlumnoErpByIdQueryHandler> localizer, IMediator mediator)
        {
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
            _localizer = localizer;
            _mediator = mediator;
        }

        public async Task<ExpedienteAcademicoModel> Handle(GetExpedienteAlumnoErpByIdQuery request, CancellationToken cancellationToken)
        {
            var expediente = await _mediator.Send(new GetExpedienteAlumnoByIdQuery(request.IdExpedienteAlumno),
                cancellationToken);

            var expedienteErp = await _erpAcademicoServiceClient.GetExpediente(expediente.Id);

            ValidateDatosExpediente(expedienteErp, expediente);

            expedienteErp.IdRefVersionPlan = expediente.IdRefVersionPlan;
            expedienteErp.IdRefPlan = expediente.IdRefPlan;
            expedienteErp.IdRefIntegracionAlumno = expediente.IdRefIntegracionAlumno;
            if (expediente.TitulacionAcceso != null)
            {
                if (expedienteErp.TitulacionAcceso == null)
                {
                    expedienteErp.TitulacionAcceso = new TitulacionAccesoAcademicoModel();
                }
                expedienteErp.TitulacionAcceso.Titulo =
                    expediente.TitulacionAcceso.Titulo;
                expedienteErp.TitulacionAcceso.InstitucionDocente =
                    expediente.TitulacionAcceso.InstitucionDocente;
                expedienteErp.TitulacionAcceso.IdRefInstitucionDocente =
                    expediente.TitulacionAcceso.IdRefInstitucionDocente;
                expedienteErp.TitulacionAcceso.CodeCountryInstitucionDocente =
                    (!string.IsNullOrEmpty(expediente.TitulacionAcceso.IdRefInstitucionDocente) && expediente.TitulacionAcceso.IdRefInstitucionDocente != "-1")
                        ? expediente.TitulacionAcceso.IdRefInstitucionDocente.Split("-")[0]
                        : !string.IsNullOrEmpty(expediente.TitulacionAcceso.IdRefTerritorioInstitucionDocente) 
                        ? expediente.TitulacionAcceso.IdRefTerritorioInstitucionDocente.Split("-")[0] 
                        : String.Empty;
                expedienteErp.TitulacionAcceso.IdRefTerritorioInstitucionDocente =
                    expediente.TitulacionAcceso.IdRefTerritorioInstitucionDocente;
                expedienteErp.TitulacionAcceso.FechaInicioTitulo =
                    expediente.TitulacionAcceso.FechaInicioTitulo;
                expedienteErp.TitulacionAcceso.FechafinTitulo =
                    expediente.TitulacionAcceso.FechafinTitulo;
                expedienteErp.TitulacionAcceso.CodigoColegiadoProfesional =
                    expediente.TitulacionAcceso.CodigoColegiadoProfesional;
                expedienteErp.TitulacionAcceso.NroSemestreRealizados =
                    expediente.TitulacionAcceso.NroSemestreRealizados;
                expedienteErp.TitulacionAcceso.TipoEstudio =
                    expediente.TitulacionAcceso.TipoEstudio;
            }
            return expedienteErp;
        }

        protected internal virtual void ValidateDatosExpediente(ExpedienteAcademicoModel expedienteErp,
            ExpedienteAlumnoItemDto expedienteAlumnoDto)
        {
            if (expedienteErp.Plan.Id.ToString() != expedienteAlumnoDto.IdRefPlan)
                throw new BadRequestException(_localizer[$"ERP Académico: El Plan con Id {expedienteAlumnoDto.IdRefPlan} no coincide"]);

            if (expedienteErp.Alumno.IdIntegracion != expedienteAlumnoDto.IdRefIntegracionAlumno)
                throw new BadRequestException(_localizer[
                    $"ERP Académico: El Alumno con Id Integración {expedienteAlumnoDto.IdRefIntegracionAlumno} no coincide"]);

            if (expedienteErp.Plan?.VersionesPlanes != null
                && !string.IsNullOrEmpty(expedienteAlumnoDto.IdRefVersionPlan)
                && !expedienteErp.Plan.VersionesPlanes.Any(x =>
                    expedienteAlumnoDto.IdRefVersionPlan.Equals(x.Id.ToString())))
                throw new WarningException(_localizer["La Versión no pertenece al Plan de Estudio"]);

            if (expedienteErp.ViaAccesoPlan != null && expedienteAlumnoDto.IdRefNodo == null ||
                expedienteErp.ViaAccesoPlan == null && expedienteAlumnoDto.IdRefNodo != null ||
                expedienteErp.ViaAccesoPlan != null && expedienteAlumnoDto.IdRefNodo != null &&
                expedienteAlumnoDto.IdRefNodo != expedienteErp.ViaAccesoPlan.Nodo.Id.ToString())
                throw new BadRequestException(_localizer[$"ERP Académico: El Nodo con Id {expedienteAlumnoDto.IdRefNodo} no coincide"]);
        }
    }
}
