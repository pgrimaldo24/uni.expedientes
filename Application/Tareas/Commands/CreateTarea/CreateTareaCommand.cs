using MediatR;
using System;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Tareas.Commands.CreateTarea
{
    public class CreateTareaCommand : IRequest<Tarea>
    {
        public int JobId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int Total { get; set; }
        public int Completadas { get; set; }
        public int Fallidas { get; set; }
        public int? IdRefUniversidad { get; set; }
        public int? IdRefEstudio { get; set; }
    }
}
