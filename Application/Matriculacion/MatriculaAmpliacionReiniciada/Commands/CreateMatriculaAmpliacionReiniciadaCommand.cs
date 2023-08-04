using System;
using MediatR;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionReiniciada.Commands
{
    public class CreateMatriculaAmpliacionReiniciadaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
