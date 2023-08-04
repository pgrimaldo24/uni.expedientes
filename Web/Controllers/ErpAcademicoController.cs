using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.FilesTypes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Personal;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Parameters;
using Unir.Expedientes.Application.Common.Queries.ErpAcademico.GetViasAccesoTreeNode;
using Unir.Expedientes.WebUi.Model.RequestParameters;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.ApiKey;
using Unir.Framework.WebApiSuperTypes.Auth.Handlers.Oidc;
using Unir.Framework.WebApiSuperTypes.Controllers;

namespace Unir.Expedientes.WebUi.Controllers
{
    [Route("api/v1/integracion/erp-academico")]
    [Authorize(AuthenticationSchemes = OidcAuthenticationOptions.DEFAULT_SCHEME + "," +
                                       ApiKeyAuthenticationOptions.DEFAULT_SCHEME)]
    public class ErpAcademicoController : ControllerSuperType
    {
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public ErpAcademicoController(IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        [Route("alumnos")]
        [HttpPost]
        public async Task<ActionResult<List<AlumnoAcademicoModel>>> GetAlumnos(AlumnoListParameters parameters)
        {
            var result =
                await _erpAcademicoServiceClient.GetAlumnos(parameters);

            return Ok(result);
        }

        [Route("personas")]
        [HttpPost]
        public async Task<ActionResult<PersonaAcademicoModel[]>> GetPersonas(string[] usersIds)
        {
            var result =
                await _erpAcademicoServiceClient.GetPersonas(usersIds);

            return Ok(result);
        }

        [Route("trabajadores")]
        [HttpPost]
        public async Task<ActionResult<List<TrabajadorAcademicoModel>>> GetTrabajadores(TrabajadorListParameters parameters)
        {
            var result =
                await _erpAcademicoServiceClient.GetTrabajadores(parameters);

            return Ok(result);
        }

        [Route("versiones-planes")]
        [HttpPost]
        public async Task<ActionResult<List<VersionPlanAcademicoModel>>> GetVersionesPlanes(
            VersionPlanListParameters parameters)
        {
            var result =
                await _erpAcademicoServiceClient.GetVersionesPlanes(parameters);

            return Ok(result);
        }

        [Route("vias-acceso")]
        [HttpPost]
        public async Task<ActionResult<List<ViaAccesoAcademicoModel>>> GetViasAcceso(
            ViaAccesoListParameters parameters)
        {
            var result =
                await _erpAcademicoServiceClient.GetViasAcceso(parameters);

            return Ok(result);
        }

        [Route("vias-acceso-tree-nodes")]
        [HttpPost]
        public async Task<ActionResult<List<ViaAccesoTreeNodeDto>>> GetViasAccesoTreeNodo(
            ViaAccesoTreeNodoCriteriaRequest criteria)
        {
            var result = await Mediator.Send(criteria.GetQueryParameters());
            return Ok(result);
        }

        [Route("planes/{id:int}/es-superado")]
        [HttpPost]
        public async Task<ActionResult<List<VersionPlanAcademicoModel>>> ItIsPlanSurpassed(int id, 
            EsPlanSuperadoParameters parameters)
        {
            var result =
                await _erpAcademicoServiceClient.ItIsPlanSurpassed(id, parameters);

            return Ok(result);
        }

        [Route("asignaturas-planes/advanced-search")]
        [HttpPost]
        public async Task<ActionResult<List<AsignaturaPlanErpAcademicoModel>>> GetAsignaturasPlanes(
            AsignaturaPlanListParameters parameters)
        {
            var result =
                await _erpAcademicoServiceClient.GetAsignaturasPlanes(parameters);
            return Ok(result);
        }

        [Route("universidades")]
        [HttpPost]
        public async Task<ActionResult<List<UniversidadAcademicoModel>>> GetUniversidadesAsync(UniversidadListParameters parameters)
        {
            var result =
                await _erpAcademicoServiceClient.GetUniversidades(parameters);

            return Ok(result);
        }

        [Route("universidades/{idUniversidad}")]
        [HttpGet]
        public async Task<ActionResult<List<MatriculaErpAcademicoModel>>> GetUniversidadById(
            int idUniversidad)
        {
            var result = await _erpAcademicoServiceClient.GetUniversidadById(idUniversidad);
            return Ok(result);
        }

        [Route("especializaciones")]
        [HttpPost]
        public async Task<ActionResult<List<UniversidadAcademicoModel>>> GetEspecializacionesAsync(EspecializacionListParameters parameters)
        {
            var result =
                await _erpAcademicoServiceClient.GetEspecializaciones(parameters);

            return Ok(result);
        }

        [Route("plan/{idPlan}/estudio/{idEstudio}")]
        [HttpGet]
        public async Task<ActionResult<List<UniversidadAcademicoModel>>> GetIdsPlanesARelacionar(int idPlan, int idEstudio)
        {
            var result = await _erpAcademicoServiceClient.GetIdsPlanesARelacionar(idPlan, idEstudio);
            return Ok(result);
        }

        [Route("alumno/{idIntegracionAlumno}")]
        [HttpGet]
        public async Task<ActionResult<AlumnoAcademicoModel>> GetAlumno(int idIntegracionAlumno)
        {
            var result = await _erpAcademicoServiceClient.GetAlumno(idIntegracionAlumno);
            return Ok(result);
        }

        [Route("alumno/{idIntegracionAlumno}/matriculas")]
        [HttpPost]
        public async Task<ActionResult<List<MatriculaErpAcademicoModel>>> GetMatriculasAlumno(string idIntegracionAlumno, AlumnoMatriculasParameters parameters)
        {
            parameters.IdIntegracionAlumno = idIntegracionAlumno;
            var result = await _erpAcademicoServiceClient.GetMatriculasAlumno(parameters);
            return Ok(result);
        }

        [Route("alumno/{idAlumno}/documentos")]
        [HttpGet]
        public async Task<ActionResult<List<DocumentoAlumnoModel>>> GetDocumentosAlumno(string idAlumno)
        {
            var result = await _erpAcademicoServiceClient.GetDocumentosAlumno(idAlumno);
            return Ok(result);
        }

        [Route("modos-requerimiento-documentacion")]
        [HttpPost]
        public async Task<ActionResult<List<MatriculaErpAcademicoModel>>> GetModosRequerimientoDocumentacionAsync(
            ModoRequerimientoDocumentacionListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetModosRequerimientoDocumentacion(parameters);
            return Ok(result);
        }
        
        [Route("modos-requerimiento-documentacion/{idModoRequerimiento}")]
        [HttpGet]
        public async Task<ActionResult<List<MatriculaErpAcademicoModel>>> GetModosRequerimientoDocumentacionAsync(
            string idModoRequerimiento)
        {
            var result = await _erpAcademicoServiceClient.GetModoRequerimientoDocumentacionById(idModoRequerimiento);
            return Ok(result);
        }

        [Route("files-types")]
        [HttpGet]
        public async Task<ActionResult<List<FileTypeAcademicoModel>>> GetFilesTypes()
        {
            var result = await _erpAcademicoServiceClient.GetFylesTypes();
            return Ok(result);
        }

        [Route("tipos-estudios")]
        [HttpPost]
        public async Task<ActionResult<List<TipoEstudioAcademicoModel>>> GetTiposEstudiosAsync(
            TipoEstudioListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetTiposEstudios(parameters);
            return Ok(result);
        }

        [Route("estudios")]
        [HttpPost]
        public async Task<ActionResult<List<EstudioAcademicoModel>>> GetEstudiosAsync(
            EstudioListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetEstudios(parameters);
            return Ok(result);
        }

        [Route("planes-estudios")]
        [HttpPost]
        public async Task<ActionResult<List<PlanAcademicoModel>>> GetPlanesEstudiosAsync(
            PlanEstudioListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetPlanes(parameters);
            return Ok(result);
        }

        [Route("tipos-asignaturas")]
        [HttpPost]
        public async Task<ActionResult<List<TipoAsignaturaErpAcademicoModel>>> GetTiposAsignaturasAsync(
            TiposAsignaturasListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetTiposAsignaturas(parameters);
            return Ok(result);
        }

        [Route("asignaturas-planes")]
        [HttpPost]
        public async Task<ActionResult<List<AsignaturaPlanErpAcademicoModel>>> GetAsignaturasPlanesAsync(
            AsignaturaPlanListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetAsignaturasPlanesParaNivelDeUso(parameters);
            return Ok(result);
        }

        [Route("idiomas")]
        [HttpPost]
        public async Task<ActionResult<List<IdiomaAcademicoModel>>> GetIdiomas(IdiomaListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetIdiomas(parameters);
            return Ok(result);
        }

        [Route("plan-requerimientos")]
        [HttpPost]
        public async Task<ActionResult<List<RequerimientoPlanAcademicoModel>>> GetRequerimientosPlan(
            RequerimientoPlanListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetRequerimientosPlan(parameters);
            return Ok(result);
        }

        [Route("trayectos-requerimientos-plan")]
        [HttpPost]
        public async Task<ActionResult<List<TrayectoPlanModel>>> GetTrayectosRequerimientosPlan(
            RequerimientoPlanListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetTrayectosRequerimientosPlan(parameters);
            return Ok(result);
        }

        [Route("plan/{idPlan:int}/requerimiento/{idRequerimiento:int}")]
        [HttpGet]
        public async Task<ActionResult<RequerimientoPlanAcademicoModel>> GetRequerimientoByIdPlanAndIdRequerimiento(
            int idPlan, int idRequerimiento)
        {
            var result = await _erpAcademicoServiceClient
                .GetRequerimientoByIdPlanAndIdRequerimiento(idPlan, idRequerimiento);
            return Ok(result);
        }
        
        [Route("matriculas-documento")]
        [HttpGet]
        public async Task<ActionResult<AlumnoAcademicoModel>> GetAlumnoMatriculasDocumentos(int idIntegracionAlumno)
        {
            var result = await _erpAcademicoServiceClient.GetAlumnoMatriculasDocumentos(idIntegracionAlumno);
            return Ok(result);
        }

        [Route("grafo-plan")]
        [HttpPost]
        public async Task<IActionResult> GetGrafoDePlan(NodosListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetGrafoDePlan(parameters);
            return Ok(result);
        }

        [Route("planes-estudios/combo-list")]
        [HttpPost]
        public async Task<IActionResult> GetPlanesEstudiosComboListAsync(PlanesListParameters parameters)
        {
            var result = await _erpAcademicoServiceClient.GetPlanesEstudios(parameters);
            return Ok(result);
        }
    }
}
