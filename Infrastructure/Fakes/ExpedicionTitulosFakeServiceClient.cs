using System.Collections.Generic;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Expedientes.Application.Common.Parameters;

namespace Unir.Expedientes.Infrastructure.Fakes
{
    public class ExpedicionTitulosFakeServiceClient : IExpedicionTitulosServiceClient
    {
        private readonly IIntegrationFakeServices _integrationFakeServices;

        public ExpedicionTitulosFakeServiceClient(IIntegrationFakeServices integrationFakeServices)
        {
            _integrationFakeServices = integrationFakeServices;
        }

        public async Task<List<SolicitudExpedicionTitulosModel>> GetSolicitudes(int idIntegracionAlumno, int idPlan, bool excluirCanceladas)
        {
            var result =
                await _integrationFakeServices.GetSolicitudesExpedicionTitulosFakeFileTemplate(idIntegracionAlumno, idPlan, excluirCanceladas);
            return result;
        }

        public Task<TipoSolicitudTituloModel> GetTipoSolicitudTituloById(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<TiposSolicitudesTitulosModel>> GetTiposSolicitudes(TiposSolicitudesParameters tiposSolicitudesParameters)
        {
            throw new System.NotImplementedException();
        }
    }
}
