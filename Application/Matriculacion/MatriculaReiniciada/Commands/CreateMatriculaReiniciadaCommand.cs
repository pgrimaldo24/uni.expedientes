using MediatR;
using System;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaReiniciada.Commands
{
    public class CreateMatriculaReiniciadaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
