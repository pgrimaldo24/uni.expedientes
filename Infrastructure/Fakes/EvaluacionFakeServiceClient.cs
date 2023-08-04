
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;

namespace Unir.Expedientes.Infrastructure.Fakes
{
    public class EvaluacionFakeServiceClient : IEvaluacionesServiceClient
    {
        private readonly IIntegrationFakeServices _integrationFakeServices;

        public EvaluacionFakeServiceClient(IIntegrationFakeServices integrationFakeServices)
        {
            _integrationFakeServices = integrationFakeServices;
        }

        public async Task<ConfiguracionVersionEscalaModel> GetConfiguracionEscalaFromNivelesAsociadosEscalas(
            int? idAsignaturaOfertada, int? idAsignaturaPlan)
        {
            var result = await _integrationFakeServices
                .GetConfiguracionEscalaFromNivelesAsociadosEscalasFakeFileTemplate(idAsignaturaOfertada, idAsignaturaPlan);
            return result;
        }
    }
}
