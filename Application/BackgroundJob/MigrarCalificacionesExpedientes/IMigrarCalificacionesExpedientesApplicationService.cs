namespace Unir.Expedientes.Application.BackgroundJob.MigrarCalificacionesExpedientes
{
    public interface IMigrarCalificacionesExpedientesApplicationService : IBackgroundJobApplicationService
    {
        string MigrarCalificacionesExpedientes(string identityInfoId, string creadoPor, MigrarCalificacionesExpedientesParameters parameters);
    }
}
