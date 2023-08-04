using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.ExpedientesGestorUnir;
using Unir.Expedientes.Application.Common.Models.Results;

namespace Unir.Expedientes.Application.Common.Interfaces
{
    public interface IExpedientesGestorUnirServiceClient
    {
        Task<ExpedienteBloqueoModel> GetBloqueoExpediente(int idAlumno, int? idPlan = null);

        Task<ExpedienteErpComercialIntegrationModel> GetExpedienteGestorFormatoComercialWithAsignaturasErp(string idIntegracionAlumno,
            int idPlan);

        Task<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>> GetExpedienteGestorFormatoErpWithAsignaturas(
            string idIntegracionAlumno, int idPlan);

        Task<ResultPrimitiveCustomValue<ExpedienteExpedientesIntegrationModel>> GetExpedienteGestorFormatoErp(
            string idIntegracionAlumno, int idPlan);
        Task<ReconocimientoIntegrationGestorModel> GetReconocimientos(string idIntegracionAlumno, int idEstudio);
    }
}
