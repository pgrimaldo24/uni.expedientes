using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.SharedDtos;

namespace Unir.Expedientes.Application.Hub
{
    public interface IExpedientesClient
    {
        Task NewJob(ProcesoBackgroundDto mensaje);
        Task StatusJobs(ProcesoBackgroundDto[] mensaje);
        Task FinishedJob(ProcesoBackgroundDto mensaje);
    }
}
