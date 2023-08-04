using MediatR;
using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaRecuperada.Commands
{
    public class CreateMatriculaRecuperadaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public List<int> IdsAsignaturasOfertadas { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
