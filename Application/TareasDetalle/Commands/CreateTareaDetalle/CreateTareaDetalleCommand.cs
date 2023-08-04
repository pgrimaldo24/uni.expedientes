using MediatR;
using System;

namespace Unir.Expedientes.Application.TareasDetalle.Commands.CreateTareaDetalle
{
    public class CreateTareaDetalleCommand : IRequest
    {
        public int IdExpediente { get; set; }
        public int IdTarea { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool CompletadaOk { get; set; }
        public string Mensaje { get; set; }
    }
}
