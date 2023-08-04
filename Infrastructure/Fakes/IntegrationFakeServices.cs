using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Expedientes;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.Results;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.Crosscutting.Files;

namespace Unir.Expedientes.Infrastructure.Fakes
{
    public class IntegrationFakeServices : IIntegrationFakeServices
    {
        private const string FakeGetBloqueoExpedientesFileTemplate = "Fakes/GetBloqueoExpedientes";
        private const string FakeObtenerExpedienteFromServiciosGestorFileTemplate = "Fakes/ObtenerExpedienteFromServiciosGestor";
        private const string FakeGetComprobacionMatriculasDocumentacionFileTemplate = "Fakes/GetComprobacionMatriculasDocumentacion";
        private const string FakeObtenerExpedienteFromServiciosGestorAcademicoFileTemplate = "Fakes/ObtenerExpedienteFromServiciosGestorAcademico";
        private const string FakeObtenerFechaPrimeraMatriculaFileTemplate = "Fakes/ObtenerFechaPrimeraMatricula";
        private const string FakeIsPlanSuperadoFileTemplate = "Fakes/IsPlanSuperado";
        private const string FakeExpedienteGestorFormatoErpFileTemplate = "Fakes/ExpedienteGestorFormatoErp";
        private const string FakeObtenerIdsPlanesARelacionarFileTemplate = "Fakes/ObtenerIdsPlanesARelacionar";
        private const string FakeObtenerSolicitudesExpedicionTitulosFileTemplate = "Fakes/SolicitudesExpedicionTitulos";
        private const string FakeObtenerConfiguracionEscalaFromNivelesAsociadosEscalasFileTemplate = "Fakes/ConfiguracionEscalaFromNivelesAsociadosEscalas";
        private const string FakeReconocimientosFileTemplate = "Fakes/Reconocimientos";

        private readonly IFileManager _fileManager;

        public IntegrationFakeServices(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public async Task<bool> UpdateFileTemplateFakeToGetBloqueoExpediente(int idIntegracionAlumno, int idPlanErp,
            ExpedienteBloqueoModel expedienteBloqueo)
        {
            var path = $"{FakeGetBloqueoExpedientesFileTemplate}_{idIntegracionAlumno}_{idPlanErp}";
            return await UpdateFileTemplateFake(expedienteBloqueo, path);
        }

        public async Task<ExpedienteBloqueoModel> GetBloqueoExpedienteFromFakeFileTemplate(int idAlumno, int? idPlan = null)
        {
            var path = $"{FakeGetBloqueoExpedientesFileTemplate}_{idAlumno}_{idPlan}";
            var resultGetData = await GetDataFromFakeFileTemplate(path, typeof(ExpedienteBloqueoModel));
            return (ExpedienteBloqueoModel)resultGetData;
        }

        public async Task<bool> UpdateFileTemplateFakeToGetExpedienteFromServiciosGestorUnir(string idIntegracionAlumno,
            int idPlan, ExpedienteErpComercialIntegrationModel expedienteErpComercialIntegrationModel)
        {
            var path = $"{FakeObtenerExpedienteFromServiciosGestorFileTemplate}_{idIntegracionAlumno}_{idPlan}";
            return await UpdateFileTemplateFake(expedienteErpComercialIntegrationModel, path);
        }

        public async Task<ExpedienteErpComercialIntegrationModel> GetExpedienteFromServiciosGestorUnirFromFakeFileTemplate(string idIntegracionAlumno, int idPlan)
        {
            var path = $"{FakeObtenerExpedienteFromServiciosGestorFileTemplate}_{idIntegracionAlumno}_{idPlan}";
            var resultGetData = await GetDataFromFakeFileTemplate(path, typeof(ExpedienteErpComercialIntegrationModel));
            return (ExpedienteErpComercialIntegrationModel)resultGetData;
        }

        public async Task<bool> UpdateFileTemplateFakeToObtenerExpedienteFromServiciosGestorAcademicoUnir(
            string idIntegracionAlumno,
            int idPlan, ExpedienteExpedientesIntegrationModel expedienteExpedientesIntegrationModel)
        {
            var path = $"{FakeObtenerExpedienteFromServiciosGestorAcademicoFileTemplate}_{idIntegracionAlumno}_{idPlan}";
            return await UpdateFileTemplateFake(expedienteExpedientesIntegrationModel, path);
        }

        public async Task<ExpedienteExpedientesIntegrationModel>
            GetExpedienteFromServiciosGestorAcademicoUnirFakeFileTemplate(string idIntegracionAlumno,
                int idPlan)
        {
            var path =
                $"{FakeObtenerExpedienteFromServiciosGestorAcademicoFileTemplate}_{idIntegracionAlumno}_{idPlan}";
            var resultGetData = await GetDataFromFakeFileTemplate(path, typeof(ExpedienteExpedientesIntegrationModel));
            return (ExpedienteExpedientesIntegrationModel)resultGetData;
        }

        public async Task<bool> UpdateFileTemplateFakeToValidateAlumnoMatriculacion(string idRefPlan,
            string idIntegracionAlumno, ValidateAlumnoMatriculacionErpAcademicoModel validateAlumnoMatriculacionModel)
        {
            var path = $"{FakeGetComprobacionMatriculasDocumentacionFileTemplate}_{idRefPlan}_{idIntegracionAlumno}";
            return await UpdateFileTemplateFake(validateAlumnoMatriculacionModel, path);
        }

        public async Task<ValidateAlumnoMatriculacionErpAcademicoModel> GetValidateAlumnoMatriculacionFromFakeFileTemplate(string idRefPlan,
            string idIntegracionAlumno)
        {
            var path = $"{FakeGetComprobacionMatriculasDocumentacionFileTemplate}_{idRefPlan}_{idIntegracionAlumno}";
            var resultGetData = await GetDataFromFakeFileTemplate(path, typeof(ValidateAlumnoMatriculacionErpAcademicoModel));
            return (ValidateAlumnoMatriculacionErpAcademicoModel)resultGetData;
        }

        public async Task<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>> 
            GetExpedienteFromServiciosGestorAcademicoUnirFromFakeFileTemplate(string idIntegracionAlumno, int idPlan)
        {
            return new ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>
            {
                Value = await GetExpedienteFromServiciosGestorAcademicoUnirFakeFileTemplate(idIntegracionAlumno, idPlan)
            };
        }

        public async Task<bool> UpdateFechaPrimeraMatriculaFake(int idRefIntegracionAlumno, int idRefPlan, DateTime? fechaPrimeraMatricula)
        {
            var path = $"{FakeObtenerFechaPrimeraMatriculaFileTemplate}_{idRefIntegracionAlumno}_{idRefPlan}";
            return await UpdateFileTemplateFake(fechaPrimeraMatricula, path);
        }

        public async Task<DateTime?> GetFechaPrimeraMatriculaFromFakeFileTemplate(int idRefIntegracionAlumno, int idRefPlan)
        {
            var path = $"{FakeObtenerFechaPrimeraMatriculaFileTemplate}_{idRefIntegracionAlumno}_{idRefPlan}";
            var resultGetData = await GetDataFromFakeFileTemplate(path, typeof(DateTime?));
            return (DateTime?)resultGetData;
        }

        public async Task<bool> UpdateIsPlanSuperadoFake(int idPlan, int idNodo, PlanSuperadoErpAcademicoModel planSuperadoErpAcademico)
        {
            var path = $"{FakeIsPlanSuperadoFileTemplate}_{idPlan}_{idNodo}";
            return await UpdateFileTemplateFake(planSuperadoErpAcademico, path);
        }

        public async Task<PlanSuperadoErpAcademicoModel> GetIsPlanSuperadoFromFakeFileTemplate(int idPlan, int idNodo)
        {
            var path = $"{FakeIsPlanSuperadoFileTemplate}_{idPlan}_{idNodo}";
            var resultGetData = await GetDataFromFakeFileTemplate(path, typeof(PlanSuperadoErpAcademicoModel));
            return (PlanSuperadoErpAcademicoModel)resultGetData;
        }

        public async Task<bool> UpdateExpedienteGestorFormatoErpFake(
            string idIntegracionAlumno,
            int idPlan, ExpedienteExpedientesIntegrationModel expedienteExpedientesIntegrationModel)
        {
            var path = $"{FakeExpedienteGestorFormatoErpFileTemplate}_{idIntegracionAlumno}_{idPlan}";
            return await UpdateFileTemplateFake(expedienteExpedientesIntegrationModel, path);
        }

        public async Task<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>>
            GetExpedienteGestorFormatoErpFromFakeFileTemplate(string idIntegracionAlumno, int idPlan)
        {
            var path = $"{FakeExpedienteGestorFormatoErpFileTemplate}_{idIntegracionAlumno}_{idPlan}";
            var expedienteIntegracion =
                (ExpedienteExpedientesIntegrationModel)await GetDataFromFakeFileTemplate(path,
                    typeof(ExpedienteExpedientesIntegrationModel));
            return new ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>
            {
                Value = expedienteIntegracion
            };
        }

        public async Task<bool> UpdateIdsPlanesARelacionarFake(int idPlan, int idEstudio, List<int> idsPlanes)
        {
            var path = $"{FakeObtenerIdsPlanesARelacionarFileTemplate}_{idPlan}_{idEstudio}";
            return await UpdateFileTemplateFake(idsPlanes, path);
        }

        public async Task<List<int>> GetIdsPlanesARelacionarFakeFileTemplate(int idPlan, int idEstudio)
        {
            var path = $"{FakeObtenerIdsPlanesARelacionarFileTemplate}_{idPlan}_{idEstudio}";
            var idsPlanes = await GetDataFromFakeFileTemplate(path, typeof(List<int>));
            return (List<int>)idsPlanes;
        }

        public async Task<bool> UpdateSolicitudesExpedicionTitulosFake(int idIntegracionAlumno, 
            int idPlan, bool excluirCanceladas, List<SolicitudExpedicionTitulosModel> solicitudes)
        {
            var path = $"{FakeObtenerSolicitudesExpedicionTitulosFileTemplate}_{idIntegracionAlumno}_{idPlan}_{excluirCanceladas}";
            return await UpdateFileTemplateFake(solicitudes, path);
        }

        public async Task<List<SolicitudExpedicionTitulosModel>> GetSolicitudesExpedicionTitulosFakeFileTemplate(
            int idIntegracionAlumno, int idPlan, bool excluirCanceladas)
        {
            var path = $"{FakeObtenerSolicitudesExpedicionTitulosFileTemplate}_{idIntegracionAlumno}_{idPlan}_{excluirCanceladas}";
            var solicitudes = await GetDataFromFakeFileTemplate(path, typeof(List<SolicitudExpedicionTitulosModel>));
            return (List<SolicitudExpedicionTitulosModel>)solicitudes;
        }

        public async Task<bool> UpdateConfiguracionEscalaFromNivelesAsociadosEscalasFake(
            int? idAsignaturaOfertada, int? idAsignaturaPlan, ConfiguracionVersionEscalaModel configuracionVersionEscala)
        {
            var path = $"{FakeObtenerConfiguracionEscalaFromNivelesAsociadosEscalasFileTemplate}_{idAsignaturaOfertada}_{idAsignaturaPlan}";
            return await UpdateFileTemplateFake(configuracionVersionEscala, path);
        }

        public async Task<ConfiguracionVersionEscalaModel> GetConfiguracionEscalaFromNivelesAsociadosEscalasFakeFileTemplate(
            int? idAsignaturaOfertada, int? idAsignaturaPlan)
        {
            var path = $"{FakeObtenerConfiguracionEscalaFromNivelesAsociadosEscalasFileTemplate}_{idAsignaturaOfertada}_{idAsignaturaPlan}";
            var configuracion = await GetDataFromFakeFileTemplate(path, typeof(ConfiguracionVersionEscalaModel));
            return (ConfiguracionVersionEscalaModel)configuracion;
        }

        public async Task<bool> UpdateReconocimientosFake(string idIntegracionAlumno, 
            int idEstudio, ReconocimientoIntegrationGestorModel reconocimiento)
        {
            var path = $"{FakeReconocimientosFileTemplate}_{idIntegracionAlumno}_{idEstudio}";
            return await UpdateFileTemplateFake(reconocimiento, path);
        }

        public async Task<ReconocimientoIntegrationGestorModel> GetReconocimientosFakeFileTemplate(
            string idIntegracionAlumno, int idEstudio)
        {
            var path = $"{FakeReconocimientosFileTemplate}_{idIntegracionAlumno}_{idEstudio}";
            var reconocimiento = await GetDataFromFakeFileTemplate(path, typeof(ReconocimientoIntegrationGestorModel));
            return (ReconocimientoIntegrationGestorModel)reconocimiento;
        }

        private async Task<bool> UpdateFileTemplateFake(object data, string path)
        {
            path = $"{path}.xml";

            if (data == null)
                throw new BadRequestException($"Fake Error: '{nameof(data)}' Is Null");

            bool result;
            try
            {
                var xmlSerialiazed = new XmlSerializer(data.GetType());
                byte[] dataBytes;
                await using (var ms = new MemoryStream())
                {
                    xmlSerialiazed.Serialize(ms, data);
                    dataBytes = ms.ToArray();
                }
                result = await _fileManager.WriteFileAsync(path, dataBytes);
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Fake Error: '{ex.InnerException?.Message ?? ex.Message}'");
            }

            return result;
        }

        private async Task<object> GetDataFromFakeFileTemplate(string path, Type type)
        {
            path = $"{path}.xml";

            if (!_fileManager.Exist(path))
                throw new BadRequestException($"Fake Error: '{path}' Doesn't Exist");

            var content = await _fileManager.ReadFileContentAsync(path);
            if (content == null || content.Length == 0)
                throw new BadRequestException($"Fake Error: '{path}' Doesn't have Content");

            object result;
            try
            {
                await using var memoryStream = new MemoryStream(content);
                using var reader = new StreamReader(memoryStream);
                var contentString = await reader.ReadToEndAsync();
                using var stringReader = new StringReader(contentString);
                using var xmlTextReader = new XmlTextReader(stringReader) { Normalization = false };
                var xmlSerializer = new XmlSerializer(type);
                result = xmlSerializer.Deserialize(xmlTextReader);
            }
            catch (Exception ex)
            {
                throw new BadRequestException($"Fake Error: '{ex.InnerException?.Message ?? ex.Message}'");
            }

            return result;
        }

    }
}
