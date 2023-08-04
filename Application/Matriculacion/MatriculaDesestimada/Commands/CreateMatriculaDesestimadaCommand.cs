using System;
using MediatR;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaDesestimada.Commands
{
    public class CreateMatriculaDesestimadaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public int? IdCausa { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
