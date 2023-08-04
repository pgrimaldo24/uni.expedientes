using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Infrastructure.Fakes;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/integracion/fake")]
    public class IntegracionFakeController : ControllerSuperType
    {
        private readonly IIntegrationFakeServices _integrationFakeService;

        public IntegracionFakeController(IIntegrationFakeServices integrationFakeService)
        {
            _integrationFakeService = integrationFakeService;
        }

        [HttpPut]
        [Route("expedientes/bloqueos")]
        public async Task<ActionResult> UpdateFileTemplateFakeToGetBloqueoExpediente(ExpedienteBloqueoModel expedienteBloqueo, int idIntegracionAlumno, int idPlanErp)
        {
            var result = await _integrationFakeService.UpdateFileTemplateFakeToGetBloqueoExpediente(idIntegracionAlumno, idPlanErp, expedienteBloqueo);
            return Ok(result);
        }

        [HttpGet]
        [Route("expedientes/bloqueos")]
        public async Task<ActionResult<ExpedienteBloqueoModel>> GetBloqueoExpedienteFromFakeFileTemplate(
            int idIntegracionAlumno, int idPlanErp)
        {
            var result =
                await _integrationFakeService.GetBloqueoExpedienteFromFakeFileTemplate(idIntegracionAlumno, idPlanErp);
            return Ok(result);
        }

        [HttpPut]
        [Route("expedientes/asignaturas-formato-erp-comercial")]
        public async Task<ActionResult> UpdateFileTemplateFakeToObtenerExpedienteFromServiciosGestorUnir(string idIntegracionAlumno, int idPlan,
            ExpedienteErpComercialIntegrationModel expedienteErpComercialIntegrationModel)
        {
            var result = await _integrationFakeService.UpdateFileTemplateFakeToGetExpedienteFromServiciosGestorUnir(idIntegracionAlumno, idPlan, expedienteErpComercialIntegrationModel);
            return Ok(result);
        }

        [HttpGet]
        [Route("expedientes/asignaturas-formato-erp-comercial")]
        public async Task<ActionResult<ExpedienteErpComercialIntegrationModel>> GetExpedienteFromServiciosGestorUnirFromFakeFileTemplate(
            string idIntegracionAlumno, int idPlan)
        {
            var result = await
                _integrationFakeService.GetExpedienteFromServiciosGestorUnirFromFakeFileTemplate(
                    idIntegracionAlumno, idPlan);

            return Ok(result);
        }

        [HttpPut]
        [Route("expedientes/asignaturas-formato-erp-academico-completo")]
        public async Task<ActionResult> UpdateFileTemplateFakeToObtenerExpedienteFromServiciosGestorAcademicoUnir(string idIntegracionAlumno, int idPlan,
            ExpedienteExpedientesIntegrationModel model)
        {
            var result = await _integrationFakeService.UpdateFileTemplateFakeToObtenerExpedienteFromServiciosGestorAcademicoUnir(idIntegracionAlumno, idPlan, model);
            return Ok(result);
        }

        [HttpGet]
        [Route("expedientes/asignaturas-formato-erp-academico-completo")]
        public async Task<ActionResult<ExpedienteExpedientesIntegrationModel>>
            GetExpedienteFromServiciosGestorAcademicoUnirFakeFileTemplate(string idIntegracionAlumno,
                int idPlan)
        {
            var result = await
                _integrationFakeService.GetExpedienteFromServiciosGestorAcademicoUnirFakeFileTemplate(
                    idIntegracionAlumno, idPlan);
            return Ok(result);
        }

        [HttpPut]
        [Route("erp/comprobacion-matriculacion-documentacion")]
        public async Task<ActionResult>
            UpdateFileTemplateFakeToValidateAlumnoMatriculacion(string idIntegracionAlumno, string idRefPlan,
                ValidateAlumnoMatriculacionErpAcademicoModel matriculacionDocumentacionSuperadaModel)
        {
            var result =
                await _integrationFakeService.UpdateFileTemplateFakeToValidateAlumnoMatriculacion(idRefPlan,
                    idIntegracionAlumno, matriculacionDocumentacionSuperadaModel);
            return Ok(result);
        }

        [HttpGet]
        [Route("erp/comprobacion-matriculacion-documentacion")]
        public async Task<ActionResult<ValidateAlumnoMatriculacionErpAcademicoModel>>
            GetValidateAlumnoMatriculacionFromFakeFileTemplate(string idIntegracionAlumno, string idRefPlan)
        {
            var result = await 
                _integrationFakeService.GetValidateAlumnoMatriculacionFromFakeFileTemplate(idRefPlan, idIntegracionAlumno);
            return Ok(result);
        }

        [HttpGet]
        [Route("expedientes/asignaturas-formato-gestor")]
        public async Task<ActionResult<ExpedienteErpComercialIntegrationModel>> GetExpedienteFromServiciosGestorAcademicoUnirFromFakeFileTemplate(
            string idIntegracionAlumno, int idPlan)
        {
            var result = await
                _integrationFakeService.GetExpedienteFromServiciosGestorAcademicoUnirFromFakeFileTemplate(
                    idIntegracionAlumno, idPlan);

            return Ok(result);
        }

        [HttpPut]
        [Route("matriculas/first-fecha-pre-matricula")]
        public async Task<ActionResult> UpdateFileTemplateFakeToPrimeraFechaPrematricula(DateTime? primeraFechaPrematricula, int idIntegracionAlumno, int idPlanErp)
        {
            var result = await _integrationFakeService.UpdateFechaPrimeraMatriculaFake(idIntegracionAlumno, idPlanErp, primeraFechaPrematricula);
            return Ok(result);
        }

        [HttpGet]
        [Route("matriculas/first-fecha-pre-matricula")]
        public async Task<ActionResult> GetPrimeraFechaPrematriculaFromFakeFileTemplate(
            int idIntegracionAlumno, int idPlan)
        {
            var result = await
                _integrationFakeService.GetFechaPrimeraMatriculaFromFakeFileTemplate(
                    idIntegracionAlumno, idPlan);

            return Ok(result);
        }

        [HttpPut]
        [Route("planes/{idPlan:int}/es-superado")]
        public async Task<ActionResult> UpdateFileTemplateFakeToIsPlanSuperado(int idPlan, int idNodo, PlanSuperadoErpAcademicoModel planSuperadoErpAcademico)
        {
            var result = await _integrationFakeService.UpdateIsPlanSuperadoFake(idPlan, idNodo, planSuperadoErpAcademico);
            return Ok(result);
        }

        [HttpGet]
        [Route("planes/{idPlan:int}/es-superado")]
        public async Task<ActionResult> GetIsPlanSuperadoFromFakeFileTemplate(int idPlan, int idNodo)
        {
            var result = await
                _integrationFakeService.GetIsPlanSuperadoFromFakeFileTemplate(idPlan, idNodo);

            return Ok(result);
        }

        [HttpPut]
        [Route("asignaturas/formato/erp-academico-completo")]
        public async Task<ActionResult> UpdateFileTemplateFakeToIsPlanSuperado(string idAlumno, int idPlanErp, ExpedienteExpedientesIntegrationModel expedienteExpedientesIntegration)
        {
            var result = await _integrationFakeService.UpdateExpedienteGestorFormatoErpFake(idAlumno, idPlanErp, expedienteExpedientesIntegration);
            return Ok(result);
        }

        [HttpGet]
        [Route("asignaturas/formato/erp-academico-completo")]
        public async Task<ActionResult> GetIsPlanSuperadoFromFakeFileTemplate(string idAlumno, int idPlanErp)
        {
            var result = await
                _integrationFakeService.GetExpedienteGestorFormatoErpFromFakeFileTemplate(idAlumno, idPlanErp);

            return Ok(result);
        }

        [HttpPut]
        [Route("plan/{idPlan:int}/estudio/{idEstudio:int}")]
        public async Task<ActionResult> UpdateIdsPlanesARelacionarFake(int idPlan, int idEstudio, List<int> idsPlanes)
        {
            var result = await _integrationFakeService.UpdateIdsPlanesARelacionarFake(idPlan, idEstudio, idsPlanes);
            return Ok(result);
        }

        [HttpGet]
        [Route("plan/{idPlan:int}/estudio/{idEstudio:int}")]
        public async Task<ActionResult> GetIdsPlanesARelacionarFakeFileTemplate(int idPlan, int idEstudio)
        {
            var result = await _integrationFakeService.GetIdsPlanesARelacionarFakeFileTemplate(idPlan, idEstudio);
            return Ok(result);
        }

        [HttpPut]
        [Route("estados-solicitudes")]
        public async Task<ActionResult> UpdateSolicitudesExpedicionTitulosFake(int idIntegracionAlumno, 
            int idPlan, bool excluirCanceladas, List<SolicitudExpedicionTitulosModel> solicitudes)
        {
            var result = await _integrationFakeService.UpdateSolicitudesExpedicionTitulosFake(idIntegracionAlumno, idPlan, excluirCanceladas, solicitudes);
            return Ok(result);
        }

        [HttpGet]
        [Route("estados-solicitudes")]
        public async Task<ActionResult> GetSolicitudesExpedicionTitulosFakeFileTemplate(int idIntegracionAlumno, int idPlan, bool excluirCanceladas)
        {
            var result = await _integrationFakeService.GetSolicitudesExpedicionTitulosFakeFileTemplate(idIntegracionAlumno, idPlan, excluirCanceladas);
            return Ok(result);
        }

        [HttpPut]
        [Route("configuracion-escala-from-niveles-asociados-escalas")]
        public async Task<ActionResult> UpdateConfiguracionEscalaFromNivelesAsociadosEscalasFake(int? idAsignaturaOfertada, 
            int? idAsignaturaPlan, ConfiguracionVersionEscalaModel configuracionVersionEscala)
        {
            var result = await _integrationFakeService
                .UpdateConfiguracionEscalaFromNivelesAsociadosEscalasFake(idAsignaturaOfertada, idAsignaturaPlan, configuracionVersionEscala);
            return Ok(result);
        }

        [HttpGet]
        [Route("configuracion-escala-from-niveles-asociados-escalas")]
        public async Task<ActionResult> GetConfiguracionEscalaFromNivelesAsociadosEscalasFakeFileTemplate(
            int? idAsignaturaOfertada, int? idAsignaturaPlan)
        {
            var result = await _integrationFakeService
                .GetConfiguracionEscalaFromNivelesAsociadosEscalasFakeFileTemplate(idAsignaturaOfertada, idAsignaturaPlan);
            return Ok(result);
        }

        [HttpPut]
        [Route("reconocimientos")]
        public async Task<ActionResult> UpdateReconocimientosFake(string idIntegracionAlumno, 
            int idEstudio, ReconocimientoIntegrationGestorModel reconocimiento)
        {
            var result = await _integrationFakeService
                .UpdateReconocimientosFake(idIntegracionAlumno, idEstudio, reconocimiento);
            return Ok(result);
        }

        [HttpGet]
        [Route("reconocimientos")]
        public async Task<ActionResult> GetReconocimientosFakeFileTemplate(
            string idIntegracionAlumno, int idEstudio)
        {
            var result = await _integrationFakeService
                .GetReconocimientosFakeFileTemplate(idIntegracionAlumno, idEstudio);
            return Ok(result);
        }
    }
}
