namespace Unir.Expedientes.Application.BackgroundJob.SetEstadosHitosEspecializacionesExpedientesAlumnos
{
    public interface ISetEstadosHitosEspecializacionesExpedientesAlumnosApplicationService
    {
        string SetEstadosHitosEspecializacionesDeExpedientesAlumnos(
            SetEstadosHitosEspecializacionesExpedientesAlumnosParameters parameters);
    }
}
