using Unir.Framework.Crosscutting.BackgroundJob;

namespace Unir.Expedientes.Application.BackgroundJob
{
    public interface IBackgroundJobApplicationService
    {
        JobDetails GetJobDetails(string procesoId);

    }
}
