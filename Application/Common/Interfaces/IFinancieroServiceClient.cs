using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Models.Financiero;
using Unir.Expedientes.Application.Common.Parameters;

namespace Unir.Expedientes.Application.Common.Interfaces
{
    public interface IFinancieroServiceClient
    {
        Task<DeudaClienteModel> GetDeudaCliente(DeudaClienteParameters request);
    }
}
