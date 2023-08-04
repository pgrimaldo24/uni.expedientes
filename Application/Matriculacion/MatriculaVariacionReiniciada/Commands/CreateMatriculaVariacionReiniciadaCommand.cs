using MediatR;
using System;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaVariacionReiniciada.Commands
{
    public class CreateMatriculaVariacionReiniciadaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
