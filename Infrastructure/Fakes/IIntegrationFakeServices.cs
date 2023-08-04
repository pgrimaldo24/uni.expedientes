using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.Results;

namespace Unir.Expedientes.Infrastructure.Fakes
{
    public interface IIntegrationFakeServices
    {
        Task<bool> UpdateFileTemplateFakeToGetBloqueoExpediente(int idIntegracionAlumno, int idPlanErp,
            ExpedienteBloqueoModel expedienteBloqueo);

        Task<ExpedienteBloqueoModel> GetBloqueoExpedienteFromFakeFileTemplate(int idAlumno, int? idPlan = null);

        Task<bool> UpdateFileTemplateFakeToGetExpedienteFromServiciosGestorUnir(string idIntegracionAlumno,
            int idPlan, ExpedienteErpComercialIntegrationModel expedienteErpComercialIntegrationModel);

        Task<ExpedienteErpComercialIntegrationModel> GetExpedienteFromServiciosGestorUnirFromFakeFileTemplate(
            string idIntegracionAlumno, int idPlan);

        Task<bool> UpdateFileTemplateFakeToObtenerExpedienteFromServiciosGestorAcademicoUnir(
            string idIntegracionAlumno,
            int idPlan, ExpedienteExpedientesIntegrationModel expedienteExpedientesIntegrationModel);

        Task<ExpedienteExpedientesIntegrationModel>
            GetExpedienteFromServiciosGestorAcademicoUnirFakeFileTemplate(string idIntegracionAlumno,
                int idPlan);

        Task<bool> UpdateFileTemplateFakeToValidateAlumnoMatriculacion(string idRefPlan,
            string idIntegracionAlumno, ValidateAlumnoMatriculacionErpAcademicoModel validateAlumnoMatriculacionModel);

        Task<ValidateAlumnoMatriculacionErpAcademicoModel> GetValidateAlumnoMatriculacionFromFakeFileTemplate(string idRefPlan,
            string idIntegracionAlumno);

        Task<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>> 
            GetExpedienteFromServiciosGestorAcademicoUnirFromFakeFileTemplate(string idIntegracionAlumno, int idPlan);

        Task<bool> UpdateFechaPrimeraMatriculaFake(int idRefIntegracionAlumno, int idRefPlan,
            DateTime? fechaPrimeraMatricula);

        Task<DateTime?> GetFechaPrimeraMatriculaFromFakeFileTemplate(int idRefIntegracionAlumno, int idRefPlan);

        Task<bool> UpdateIsPlanSuperadoFake(int idPlan, int idNodo,
            PlanSuperadoErpAcademicoModel planSuperadoErpAcademico);

        Task<PlanSuperadoErpAcademicoModel> GetIsPlanSuperadoFromFakeFileTemplate(int idPlan, int idNodo);

        Task<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>>
            GetExpedienteGestorFormatoErpFromFakeFileTemplate(string idIntegracionAlumno, int idPlan);

        Task<bool> UpdateExpedienteGestorFormatoErpFake(string idIntegracionAlumno, int idPlan,
            ExpedienteExpedientesIntegrationModel expedienteExpedientesIntegrationModel);

        Task<bool> UpdateIdsPlanesARelacionarFake(int idPlan, int idEstudio, List<int> idsPlanes);
        Task<List<int>> GetIdsPlanesARelacionarFakeFileTemplate(int idPlan, int idEstudio);
        Task<bool> UpdateSolicitudesExpedicionTitulosFake(int idIntegracionAlumno, int idPlan, 
            bool excluirCanceladas, List<SolicitudExpedicionTitulosModel> solicitudes);
        Task<List<SolicitudExpedicionTitulosModel>> GetSolicitudesExpedicionTitulosFakeFileTemplate(
            int idIntegracionAlumno, int idPlan, bool excluirCanceladas);
        Task<bool> UpdateConfiguracionEscalaFromNivelesAsociadosEscalasFake(
            int? idAsignaturaOfertada, int? idAsignaturaPlan, ConfiguracionVersionEscalaModel configuracionVersionEscala);
        Task<ConfiguracionVersionEscalaModel> GetConfiguracionEscalaFromNivelesAsociadosEscalasFakeFileTemplate(
            int? idAsignaturaOfertada, int? idAsignaturaPlan);

        Task<bool> UpdateReconocimientosFake(string idIntegracionAlumno, int idEstudio,
            ReconocimientoIntegrationGestorModel reconocimiento);

        Task<ReconocimientoIntegrationGestorModel> GetReconocimientosFakeFileTemplate(
            string idIntegracionAlumno, int idEstudio);
    }
}
