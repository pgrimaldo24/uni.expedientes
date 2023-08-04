using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.FilesTypes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio.Migracion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Personal;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Security;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Titulos;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Parameters;

namespace Unir.Expedientes.Infrastructure.Fakes
{
    public class ErpAcademicoFakeServiceClient : IErpAcademicoServiceClient
    {
        private readonly IIntegrationFakeServices _integrationFakeServices;

        public ErpAcademicoFakeServiceClient(IIntegrationFakeServices integrationFakeServices)
        {
            _integrationFakeServices = integrationFakeServices;
        }

        public Task<List<AlumnoAcademicoModel>> GetAlumnos(AlumnoListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<ExpedienteAcademicoModel> GetExpediente(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ExpedienteAcademicoModel[]> GetExpedientesAsync(string[] idsIntegracion, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<NodoErpAcademicoModel> GetNodo(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PersonaAcademicoModel[]> GetPersonas(string[] userIds)
        {
            throw new NotImplementedException();
        }

        public Task<List<TrabajadorAcademicoModel>> GetTrabajadores(TrabajadorListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<VersionPlanAcademicoModel>> GetVersionesPlanes(VersionPlanListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<ViaAccesoAcademicoModel>> GetViasAcceso(ViaAccesoListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<AsignaturaPlanErpAcademicoModel>> GetAsignaturasPlanes(AsignaturaPlanListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<PlanSuperadoErpAcademicoModel> ItIsPlanSurpassed(int idPlan, EsPlanSuperadoParameters parameters)
        {
            var result = _integrationFakeServices.GetIsPlanSuperadoFromFakeFileTemplate(idPlan, parameters.IdNodo);
            return result;
        }

        public async Task<ValidateAlumnoMatriculacionErpAcademicoModel> ValidateAlumnoMatriculacion(ValidateAlumnoMatriculacionParameters parameters)
        {
            var result =
                await _integrationFakeServices.GetValidateAlumnoMatriculacionFromFakeFileTemplate(parameters.IdRefPlan,
                    parameters.IdIntegracionAlumno);
            return result;
        }

        public Task<List<AsignaturaPlanTitulacionErpAcademico>> GetAsignaturasPlanesParaTitulacion(AsignaturasPlanTitulacionParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<int>> GetAsignaturasEspecializacionPlan(int idEspecializacion, int idPlan)
        {
            throw new NotImplementedException();
        }

        public Task<UsuarioModel> GetUserNameByIdSeguridad(string idSeguridad)
        {
            throw new NotImplementedException();
        }

        public Task<List<UniversidadAcademicoModel>> GetUniversidades(UniversidadListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<EspecializacionIntegrationModel>> GetEspecializaciones(EspecializacionListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<Unit> ModifyExpedienteByIdIntegracion(string idIntegracion, ExpedienteEditParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<ViaAccesoPlanAcademicoModel>> GetViasAccesosPlanes(ViaAccesoPlanListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<DateTime?> GetFirstFechaPrimerMatricula(string idRefIntegracionAlumno, string idRefPlan)
        {
            var result = _integrationFakeServices.GetFechaPrimeraMatriculaFromFakeFileTemplate(int.Parse(idRefIntegracionAlumno),
                    int.Parse(idRefPlan));
            return result;
        }

        public async Task<List<int>> GetIdsPlanesARelacionar(int idPlan, int idEstudio)
        {
            var result =
                await _integrationFakeServices.GetIdsPlanesARelacionarFakeFileTemplate(idPlan, idEstudio);
            return result;
        }

        public Task<AlumnoAcademicoModel> GetAlumno(int idIntegracionAlumno)
        {
            throw new NotImplementedException();
        }

        public Task<List<MatriculaErpAcademicoModel>> GetMatriculasAlumno(AlumnoMatriculasParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetFotoAlumnoById(int alumnoId)
        {
            throw new NotImplementedException();
        }

        public Task<DocumentoAlumnoModel> GetDocumentosAlumno(string idAlumno)
        {
            throw new NotImplementedException();
        }

        public Task<List<ModoRequerimientoDocumentacionAcademicoModel>> GetModosRequerimientoDocumentacion(
            ModoRequerimientoDocumentacionListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<FileTypeAcademicoModel>> GetFylesTypes()
        {
            throw new NotImplementedException();
        }

        public Task<ModoRequerimientoDocumentacionAcademicoModel> GetModoRequerimientoDocumentacionById(string idModoRequerimiento)
        {
            throw new NotImplementedException();
        }

        public Task<UniversidadAcademicoModel> GetUniversidadById(int idUniversidad)
        {
            throw new NotImplementedException();
        }
        
        public Task<List<TipoEstudioAcademicoModel>> GetTiposEstudios(TipoEstudioListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<EstudioAcademicoModel>> GetEstudios(EstudioListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlanAcademicoModel>> GetPlanes(PlanEstudioListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<TipoAsignaturaErpAcademicoModel>> GetTiposAsignaturas(TiposAsignaturasListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<AsignaturaPlanErpAcademicoModel>> GetAsignaturasPlanesParaNivelDeUso(AsignaturaPlanListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<TipoEstudioAcademicoModel> GetTipoEstudioById(int idTipoEstudio)
        {
            throw new NotImplementedException();
        }

        public Task<EstudioAcademicoModel> GetEstudioById(int idEstudio)
        {
            throw new NotImplementedException();
        }

        public Task<PlanAcademicoModel> GetPlanEstudioById(int idPlanEstudio)
        {
            throw new NotImplementedException();
        }

        public Task<TipoAsignaturaErpAcademicoModel> GetTipoAsignaturaById(int idTipoAsignatura)
        {
            throw new NotImplementedException();
        }

        public Task<AsignaturaPlanErpAcademicoModel> GetAsignaturaPlanById(int idAsignatura)
        {
            throw new NotImplementedException();
        }

        public Task<AlumnoAcademicoModel> GetAlumnoMatriculasDocumentos(int idIntegracionAlumno)
        {
            throw new NotImplementedException();
        }

        public Task<List<IdiomaAcademicoModel>> GetIdiomas(IdiomaListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<IdiomaAcademicoModel> GetIdioma(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<MatriculaAcademicoModel>> GetMatriculasByIdExpediente(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<AsignaturaMatriculadaModel>> GetAsignaturasMatricula(int idMatricula)
        {
            throw new NotImplementedException();
        }
        
        public Task<List<RequerimientoPlanAcademicoModel>> GetRequerimientosPlan(RequerimientoPlanListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<RequerimientoPlanAcademicoModel> GetRequerimientoByIdPlanAndIdRequerimiento(int idPlan, int idRequerimiento)
        {
            throw new NotImplementedException();
        }

        public Task<MatriculaAcademicoModel> GetMatriculaByIdIntegracionMatricula(string idIntegracionMatricula)
        {
            throw new NotImplementedException();
        }

        public Task<CausaBajaMatriculaModel> GetCausaBajaMatricula(int idCausa)
        {
            throw new NotImplementedException();
        }

        public Task<TipoBajaMatriculaModel> GetTipoBajaMatricula(int idTipo)
        {
            throw new NotImplementedException();
        }
        
        public Task<GrafoAcademicoModel> GetGrafoDePlan(NodosListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<List<TrayectoPlanModel>> GetTrayectosRequerimientosPlan(RequerimientoPlanListParameters parameters)
        {
            throw new NotImplementedException();
        }

        public Task<EspecializacionAcademicoModel> GetEspecializacion(int especializacionId)
        {
            throw new NotImplementedException();
        }

        public Task<List<AsignaturaOfertadaMigracionModel>> GetAsignaturasOfertadasByExpediente(List<string> idsExpediente)
        {
            throw new NotImplementedException();
        }

        public Task<List<AsignaturaMatriculadaModel>> GetAsignaturasMatriculadasParaNotaFinal(int idRefCurso, List<string> idRefsAlumno)
        {
            throw new NotImplementedException();
        }

        public Task<List<PlanEstudioErpAcademicoModel>> GetPlanesEstudios(PlanesListParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}
