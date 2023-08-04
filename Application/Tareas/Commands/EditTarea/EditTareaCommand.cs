using MediatR;
using System;

namespace Unir.Expedientes.Application.Tareas.Commands.EditTarea
{
    public class EditTareaCommand : IRequest
    {
        public int IdTarea { get; set; }
        public DateTime? FechaFin { get; set; }
        public int Total { get; set; }
        public int Completadas { get; set; }
        public int Fallidas { get; set; }
    }
}
