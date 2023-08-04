namespace Unir.Expedientes.Application.BackgroundJob.RelacionarExpedientesAlumnos
{
    public interface IRelacionarExpedientesAlumnosApplicationService
    {
        string RelacionarExpedientesAlumnos(RelacionarExpedientesAlumnosParameters parameters);
    }
}
