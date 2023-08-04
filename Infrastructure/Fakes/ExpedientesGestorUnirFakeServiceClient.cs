using System;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.Results;

namespace Unir.Expedientes.Infrastructure.Fakes
{
    public class ExpedientesGestorUnirFakeServiceClient : IExpedientesGestorUnirServiceClient
    {
        protected readonly IIntegrationFakeServices IntegrationFakeServices;

        public ExpedientesGestorUnirFakeServiceClient(IIntegrationFakeServices integrationFakeServices)
        {
            IntegrationFakeServices = integrationFakeServices;
        }


        public Task<ExpedienteBloqueoModel> GetBloqueoExpediente(int idAlumno, int? idPlan = null)
        {
            throw new NotImplementedException();
        }

        public async Task<ExpedienteErpComercialIntegrationModel> GetExpedienteGestorFormatoComercialWithAsignaturasErp(string idIntegracionAlumno, int idPlan)
        {
            var result =
                await IntegrationFakeServices.GetExpedienteFromServiciosGestorUnirFromFakeFileTemplate(idIntegracionAlumno,
                    idPlan);
            return result;
        }

        public async Task<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>> GetExpedienteGestorFormatoErpWithAsignaturas(string idIntegracionAlumno, int idPlan)
        {
            var result =
                await IntegrationFakeServices.GetExpedienteFromServiciosGestorAcademicoUnirFromFakeFileTemplate(idIntegracionAlumno,
                    idPlan);
            return result;
        }

        public async Task<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>> GetExpedienteGestorFormatoErp(string idIntegracionAlumno, int idPlan)
        {
            var result =
                await IntegrationFakeServices.GetExpedienteGestorFormatoErpFromFakeFileTemplate(idIntegracionAlumno,
                    idPlan);
            return result;
        }

        public async Task<ReconocimientoIntegrationGestorModel> GetReconocimientos(string idIntegracionAlumno, int idEstudio)
        {
            var result = await IntegrationFakeServices.
                GetReconocimientosFakeFileTemplate(idIntegracionAlumno, idEstudio);
            return result;
        }
    }
}
