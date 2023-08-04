using System.Threading.Tasks;

namespace Unir.Expedientes.Application.Common.Interfaces
{
    public interface ISecurityIntegrationServices
    {
        Task<string> GetTokenErpAcademico();
        Task<string> GetTokenServiciosDesUnirExpedientes();
        Task<string> GetTokenGestorDocumental();
    }
}
