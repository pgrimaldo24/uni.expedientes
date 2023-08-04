using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.CreateExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumnoPorIntegracion;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumnoToAbierto;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedientesAlumnosByIntegracion;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditTipoVinculacionExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditTitulacionAccesoExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditViaAccesoPlanExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.HasBloqueoExpedienteAlumno;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ProcesarExpedienteByIdSinEstado;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ProcesarRelacionExpedienteAlumnoById;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.RelacionarExpedientesAlumnosBackground;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.SetEstadosHitosEspecializacionesExpedientesAlumnos;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.CanQualifyAlumnoInPlanEstudio;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperados;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetDatosSolicitudTitulacionSupercap;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoById;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedienteAlumnoErpById;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetInfoAlumnoByIdAlumnoIntegracion;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetPagedExpedientesAlumnos;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetTitulacionAccesoAndEspecializacionesByIdExpediente;
using Unir.Expedientes.WebUi.Model.RequestParameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;
using Unir.Framework.WebApiSuperTypes.Models.Cal;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllEstadosExpedientes;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetTiposRelacionesExpedientes;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetConfiguracionExpedienteUniversidadByIdExpediente;
using Unir.Expedientes.Application.ExpedientesAlumnos.Commands.ConsolidarRequisitosComportamientosExpediente;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetEspecializacionesExpedienteByIdExpediente;

namespace Unir.Expedientes.WebUi.Controllers;

[Route("api/v1/expedientes-alumnos")]
[Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                   ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
public class ExpedientesAlumnosController : ControllerSuperType
{
    [Route("query")]
    [HttpPost]
    public async Task<ActionResult<PagedListResponseMessage<ExpedienteAlumnoPagedListItemDto>>>
        GetPagedExpedientesAlumnosAsync(ExpedienteAlumnoPagedCriteriaRequest criteria)
    {
        var result = await Mediator.Send(criteria.GetQueryParameters());
        var response = new PagedListResponseMessage<ExpedienteAlumnoPagedListItemDto>
        {
            Data = result.Elements,
            Meta = new MetaResponse
            {
                TotalCount = result.TotalElements
            }
        };

        return Ok(response);
    }

    [Route("query-no-paged")]
    [HttpPost]
    public async Task<ActionResult<ExpedienteAlumnoListItemDto[]>>
        GetAllExpedientesAlumnosAsync(ExpedienteAlumnoCriteriaRequest criteria)
    {
        var result = await Mediator.Send(criteria.GetQueryParameters());
        return Ok(result);
    }

    [Route("{id:int}/datos-erp")]
    [HttpGet]
    public async Task<ActionResult<ExpedienteAcademicoModel>> GetExpedienteAlumnoErp(int id)
    {
        var result = await Mediator.Send(new GetExpedienteAlumnoErpByIdQuery(id));
        return Ok(result);
    }

    [Route("{id:int}")]
    [HttpGet]
    public async Task<ActionResult<ExpedienteAlumnoItemDto>> GetExpedienteAlumno(int id)
    {
        var result = await Mediator.Send(new GetExpedienteAlumnoByIdQuery(id));
        return Ok(result);
    }

    [Route("")]
    [HttpPost]
    public async Task<ActionResult> CreateExpedienteAlumno(CreateExpedienteAlumnoCommand command)
    {
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [Route("{id:int}/por-integracion")]
    [HttpPut]
    public async Task<ActionResult> EditExpedienteAlumnoPorIntegracion(int id, EditExpedienteAlumnoPorIntegracionCommand command)
    {
        command.IdExpedienteAlumno = id;
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [Route("{id:int}")]
    [HttpPut]
    public async Task<ActionResult> EditExpedienteAlumno(int id, EditExpedienteAlumnoCommand command)
    {
        command.IdExpedienteAlumno = id;
        var result = await Mediator.Send(command);
        return Ok(result);
    }

    [Route("{id:int}/tiene-expediente-bloqueado")]
    [HttpGet]
    public async Task<ActionResult<bool>> TieneExpedienteBloqueado(int id)
    {
        var result = await Mediator.Send(new HasBloqueoExpedienteAlumnoCommand(id));
        return Ok(result);
    }

    [Route("update-version-alumnos-antiguos-por-integracion")]
    [HttpPut]
    public async Task<ActionResult> EditExpedientesAlumnosVersionAntiguaPorIntegracion(
        List<EditExpedienteAlumnoByIdIntegracionParametersDto> parameters)
    {
        return Ok(await Mediator.Send(new EditExpedientesAlumnosByIntegracionCommand(parameters)));
    }

    [Route("has-any")]
    [HttpPost]
    public async Task<ActionResult<bool>> HasExpedientesAlumnos(HasExpedientesAlumnosCriteriaRequest criteria)
    {
        var result = await Mediator.Send(criteria.GetQueryParameters());
        return Ok(result);
    }

    [Route("alumno-puede-titularse-en-plan")]
    [HttpPost]
    public async Task<ActionResult<ExpedienteAlumnoTitulacionPlanDto>> AlumnoPuedeTitularseEnPlan(CanQualifyAlumnoInPlanEstudioCriteriaRequest criteria)
    {
        var result = await Mediator.Send(criteria.GetQueryParameters());
        return Ok(result);
    }

    [Route("expediente-alumno")]
    [HttpPost]
    public async Task<ActionResult<AlumnoPuedeTitularseDto>> ExpedienteAlumno(
        GetAlumnoPuedeTitularseEnPlanConElementosSuperadosYNoSuperadosCriteriaRequest criteria)
    {
        var result = await Mediator.Send(criteria.GetQueryParameters());
        return Ok(result);
    }

    [Route("{id:int}/tipo-vinculacion")]
    [HttpPut]
    public async Task<ActionResult> EditTipoVinculacionAsync(int id, EditTipoVinculacionByExpedienteAlumnoCommand request)
    {
        request.IdExpedienteAlumno = id;
        return Ok(await Mediator.Send(request));
    }

    [Route("{id:int}/via-acceso-plan")]
    [HttpPut]
    public async Task<ActionResult> EditViaAccesoPlanAsync(int id, EditViaAccesoPlanExpedienteAlumnoCommand request)
    {
        request.IdExpedienteAlumno = id;
        return Ok(await Mediator.Send(request));
    }

    [Route("{id:int}/titulacion-acceso")]
    [HttpPut]
    public async Task<ActionResult> EditTitulacionAccesoAsync(int id, EditTitulacionAccesoExpedienteAlumnoCommand request)
    {
        request.IdExpedienteAlumno = id;
        return Ok(await Mediator.Send(request));
    }

    [Route("solicitud-titulacion-supercap")]
    [HttpGet]
    public async Task<ActionResult<ExpedienteAlumnoSolicitudTitulacionSupercapDto>>
        GetDatosSolicitudTitulacionSupercapQueryAsync(string idRefPlan, string idRefIntegracionAlumno)
    {
        var result =
            await Mediator.Send(new GetDatosSolicitudTitulacionSupercapQuery(idRefPlan, idRefIntegracionAlumno));
        return Ok(result);
    }

    [Route("{id:int}/abierto")]
    [HttpPut]
    public async Task<ActionResult> EditExpedienteAlumnoToAbiertoAsync(int id, EditExpedienteAlumnoToAbiertoCommand request)
    {
        request.Id = id;
        return Ok(await Mediator.Send(request));
    }

    [Route("{id:int}/set-estado-hitos-especializaciones")]
    [HttpPut]
    public async Task<ActionResult> CambiarEstadosExpedienteAlumnoAsync(int id)
    {
        return Ok(await Mediator.Send(new ProcesarExpedienteByIdSinEstadoCommand { IdExpedienteAlumno = id }));
    }

    [Route("set-estados-hitos-especializaciones")]
    [HttpPut]
    public async Task<ActionResult> SetEstadosHitosEspecializacionesDeExpedientesAlumnosAsync(
        SetEstadosHitosEspecializacionesExpedientesAlumnosCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result);
    }

    [Route("{id:int}/relacionar-expediente")]
    [HttpPost]
    public async Task<ActionResult> RelacionarExpedienteAlumnoByIdAsync(int id)
    {
        return Ok(await Mediator.Send(new ProcesarRelacionExpedienteAlumnoByIdCommand { IdExpedienteAlumno = id }));
    }

    [Route("relacionar-expedientes")]
    [HttpPost]
    public async Task<ActionResult> RelacionarExpedientesAlumnosBackgroundAsync(
        RelacionarExpedientesAlumnosBackgroundCommand request)
    {
        var result = await Mediator.Send(request);
        return Ok(result);
    }

    [Route("{id:int}/titulacion-acceso-especializaciones")]
    [HttpGet]
    public async Task<ActionResult<ExpedienteTitulacionAccesoEspecializacionesItemDto>>
        GetTitulacionAccesoAndEspecializacionesByIdExpedienteAsync(int id)
    {
        var result = await Mediator.Send(new GetTitulacionAccesoAndEspecializacionesByIdExpedienteQuery(id));
        return Ok(result);
    }
    
    [Route("alumno/{idAlumnoIntegracion}")]
    [HttpGet]
    public async Task<ActionResult> GetInfoAlumnoByIdAlumnoIntegracion(string idAlumnoIntegracion)
    {
        return Ok(await Mediator.Send(new GetInfoAlumnoByIdAlumnoIntegracionQuery(idAlumnoIntegracion)));
    }

    [Route("estado-expediente/query")]
    [HttpPost]
    public async Task<ActionResult<EstadoExpedienteListItemDto[]>> GetEstadosExpedientesAsync(
        EstadosExpedientesCriteriaRequest criteria)
    {
        criteria.Offset = criteria.Limit * (criteria.Offset - 1);
        var result = await Mediator.Send(criteria.GetQueryParameters());
        return Ok(result);
    }

    [Route("tipos-relaciones/query")]
    [HttpPost]
    public async Task<ActionResult<TipoRelacionExpedienteListItemDto[]>> GetTiposRelacionesExpedientesAsync(
        TiposRelacionesExpedientesCriteriaRequest criteria)
    {
        criteria.Offset = criteria.Limit * (criteria.Offset - 1);
        var result = await Mediator.Send(criteria.GetQueryParameters());
        return Ok(result);
    }

    [Route("{id:int}/configuracion-universidad")]
    [HttpGet]
    public async Task<ActionResult<ConfiguracionExpedienteUniversidadDto>> GetConfiguracionExpedienteUniversidadByIdExpedienteAsync(int id)
    {
        var result = await Mediator.Send(new GetConfiguracionExpedienteUniversidadByIdExpedienteQuery(id));
        return Ok(result);
    }

    [Route("{id:int}/consolidar-requisitos")]
    [HttpPost]
    public async Task<IActionResult> ConsolidarRequisitosComportamientosExpedienteAsync(int id)
    {
        await Mediator.Send(new ConsolidarRequisitosComportamientosExpedienteCommand(id));
        return Ok();
    }

    [Route("{id:int}/especializaciones")]
    [HttpGet]
    public async Task<IActionResult> GetEspecializacionesExpedienteByIdExpedienteAsync(int id)
    {
        var result = await Mediator.Send(new GetEspecializacionesExpedienteByIdExpedienteQuery(id));
        return Ok(result);
    }
}