using System;
using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.SetEstadosHitosEspecializacionesExpedientesAlumnos;

public class SetEstadosHitosEspecializacionesExpedientesAlumnosCommand : IRequest<string>
{
    public int? IdUniversidad { get; set; }
    public int? IdEstudio { get; set; }
    public DateTime? FechaAperturaDesde { get; set; }
    public DateTime? FechaAperturaHasta { get; set; }
}