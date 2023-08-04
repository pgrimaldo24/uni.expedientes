using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;

namespace Unir.Expedientes.Application.Common.Interfaces
{
    public interface IEvaluacionesServiceClient
    {
        Task<ConfiguracionVersionEscalaModel> GetConfiguracionEscalaFromNivelesAsociadosEscalas(
            int? idAsignaturaOfertada, int? idAsignaturaPlan = null);
    }
}
