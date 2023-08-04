using System;

namespace Unir.Expedientes.Application.BackgroundJob.SetEstadosHitosEspecializacionesExpedientesAlumnos;

public class SetEstadosHitosEspecializacionesExpedientesAlumnosParameters
{
    public int? IdUniversidad { get; set; }
    public int? IdEstudio { get; set; }
    public DateTime? FechaAperturaDesde { get; set; }
    public DateTime? FechaAperturaHasta { get; set; }
    public string AccountId { get; set; }
}