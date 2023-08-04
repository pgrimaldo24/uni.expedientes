using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

namespace Unir.Expedientes.Application.Common.Interfaces
{
    public interface IErpAcademicoServiceClient
    {
        Task<ExpedienteAcademicoModel[]> GetExpedientesAsync(string[] idsIntegracion,
            CancellationToken cancellationToken);
        Task<List<AlumnoAcademicoModel>> GetAlumnos(AlumnoListParameters parameters);
        Task<ExpedienteAcademicoModel> GetExpediente(int id);
        Task<PersonaAcademicoModel[]> GetPersonas(string[] userIds);
        Task<List<TrabajadorAcademicoModel>> GetTrabajadores(TrabajadorListParameters parameters);
        Task<NodoErpAcademicoModel> GetNodo(int id);
        Task<Unit> ModifyExpedienteByIdIntegracion(string idIntegracion,
            ExpedienteEditParameters parameters);
        Task<List<VersionPlanAcademicoModel>> GetVersionesPlanes(VersionPlanListParameters parameters);
        Task<List<ViaAccesoAcademicoModel>> GetViasAcceso(ViaAccesoListParameters parameters);
        Task<List<AsignaturaPlanErpAcademicoModel>> GetAsignaturasPlanes(AsignaturaPlanListParameters parameters);
        Task<PlanSuperadoErpAcademicoModel> ItIsPlanSurpassed(int idPlan, EsPlanSuperadoParameters parameters);

        Task<ValidateAlumnoMatriculacionErpAcademicoModel> ValidateAlumnoMatriculacion(
            ValidateAlumnoMatriculacionParameters parameters);

        Task<List<AsignaturaPlanTitulacionErpAcademico>> GetAsignaturasPlanesParaTitulacion(
            AsignaturasPlanTitulacionParameters parameters);

        Task<List<int>> GetAsignaturasEspecializacionPlan(int idEspecializacion, int idPlan);
        Task<UsuarioModel> GetUserNameByIdSeguridad(string idSeguridad);
        Task<List<UniversidadAcademicoModel>> GetUniversidades(UniversidadListParameters parameters);

        Task<List<EspecializacionIntegrationModel>> GetEspecializaciones(
            EspecializacionListParameters parameters);
        Task<EspecializacionAcademicoModel> GetEspecializacion(int especializacionId);
        Task<List<ViaAccesoPlanAcademicoModel>> GetViasAccesosPlanes(ViaAccesoPlanListParameters parameters);
        Task<DateTime?> GetFirstFechaPrimerMatricula(string idRefIntegracionAlumno, string idRefPlan);
        Task<List<int>> GetIdsPlanesARelacionar(int idPlan, int idEstudio);
        Task<AlumnoAcademicoModel> GetAlumno(int idIntegracionAlumno);
        Task<AlumnoAcademicoModel> GetAlumnoMatriculasDocumentos(int idIntegracionAlumno);
        Task<List<MatriculaErpAcademicoModel>> GetMatriculasAlumno(AlumnoMatriculasParameters parameters);
        Task<string> GetFotoAlumnoById(int alumnoId);
        Task<DocumentoAlumnoModel> GetDocumentosAlumno(string idAlumno);
        Task<List<ModoRequerimientoDocumentacionAcademicoModel>> GetModosRequerimientoDocumentacion(
            ModoRequerimientoDocumentacionListParameters parameters);
        Task<ModoRequerimientoDocumentacionAcademicoModel> GetModoRequerimientoDocumentacionById(string idModoRequerimiento);
        Task<List<FileTypeAcademicoModel>> GetFylesTypes();
        Task<UniversidadAcademicoModel> GetUniversidadById(int idUniversidad);
        Task<List<TipoEstudioAcademicoModel>> GetTiposEstudios(TipoEstudioListParameters parameters);
        Task<List<EstudioAcademicoModel>> GetEstudios(EstudioListParameters parameters);
        Task<List<PlanAcademicoModel>> GetPlanes(PlanEstudioListParameters parameters);
        Task<List<TipoAsignaturaErpAcademicoModel>> GetTiposAsignaturas(
            TiposAsignaturasListParameters parameters);
        Task<List<AsignaturaPlanErpAcademicoModel>> GetAsignaturasPlanesParaNivelDeUso(
            AsignaturaPlanListParameters parameters);
        Task<TipoEstudioAcademicoModel> GetTipoEstudioById(int idTipoEstudio);
        Task<EstudioAcademicoModel> GetEstudioById(int idEstudio);
        Task<PlanAcademicoModel> GetPlanEstudioById(int idPlanEstudio);
        Task<TipoAsignaturaErpAcademicoModel> GetTipoAsignaturaById(int idTipoAsignatura);
        Task<AsignaturaPlanErpAcademicoModel> GetAsignaturaPlanById(int idAsignatura);
        Task<List<IdiomaAcademicoModel>> GetIdiomas(IdiomaListParameters parameters);
        Task<IdiomaAcademicoModel> GetIdioma(int id);
        Task<List<MatriculaAcademicoModel>> GetMatriculasByIdExpediente(int id);
        Task<List<AsignaturaMatriculadaModel>> GetAsignaturasMatricula(int idMatricula);
        Task<List<AsignaturaOfertadaMigracionModel>> GetAsignaturasOfertadasByExpediente(List<string> idsExpediente);
        Task<List<RequerimientoPlanAcademicoModel>> GetRequerimientosPlan(RequerimientoPlanListParameters parameters);
        Task<List<TrayectoPlanModel>> GetTrayectosRequerimientosPlan(RequerimientoPlanListParameters parameters);
        Task<RequerimientoPlanAcademicoModel> GetRequerimientoByIdPlanAndIdRequerimiento(int idPlan, int idRequerimiento);
        Task<MatriculaAcademicoModel> GetMatriculaByIdIntegracionMatricula(string idIntegracionMatricula);
        Task<CausaBajaMatriculaModel> GetCausaBajaMatricula(int idCausa);
        Task<TipoBajaMatriculaModel> GetTipoBajaMatricula(int idTipo);
        Task<GrafoAcademicoModel> GetGrafoDePlan(NodosListParameters parameters);
        Task<List<AsignaturaMatriculadaModel>> GetAsignaturasMatriculadasParaNotaFinal(
            int idRefCurso, List<string> idRefsAlumno);
        Task<List<PlanEstudioErpAcademicoModel>> GetPlanesEstudios(PlanesListParameters parameters);
    }
}
