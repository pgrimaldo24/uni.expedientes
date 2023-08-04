using MediatR;
using System;

namespace Unir.Expedientes.Application.TiposSituacionEstadoExpedientes.Commands.CreateTipoSituacionEstadoExpediente
{
    public class CreateTipoSituacionEstadoExpedienteCommand : IRequest
    {
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Descripcion { get; set; }
        public int TipoSituacionEstadoId { get; set; }
        public int ExpedienteAlumnoId { get; set; }
    }
}
