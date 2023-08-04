using System.Collections.Generic;

namespace Unir.Expedientes.Application.BackgroundJob.SetEstadosHitosEspecializacionesExpedientesAlumnos;

public class EstadoHitosEspecializacionesExpedienteAlumnoBackgroundDto
{
    public EstadoHitosEspecializacionesExpedienteAlumnoBackgroundDto()
    {
        IdsExpedientesFallidos = new List<int>();
    }

    public int Fallidos { get; set; }
    public int Correctos { get; set; }
    public int TotalExpedientesEncontrados { get; set; }
    public List<int> IdsExpedientesFallidos { get; set; }
}