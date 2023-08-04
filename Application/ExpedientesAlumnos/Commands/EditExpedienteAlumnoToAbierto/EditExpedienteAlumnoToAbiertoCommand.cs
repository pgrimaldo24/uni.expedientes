using System;
using MediatR;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumnoToAbierto;

public class EditExpedienteAlumnoToAbiertoCommand : IRequest
{
    public int Id { get; set; }
    public DateTime? FechaApertura { get; set; }
    public DateTime? FechaRecepcionMensaje { get; set; }
}