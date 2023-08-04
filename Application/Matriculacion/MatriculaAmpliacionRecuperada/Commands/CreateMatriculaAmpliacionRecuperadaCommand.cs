using MediatR;
using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionRecuperada.Commands
{
    public class CreateMatriculaAmpliacionRecuperadaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public DateTime FechaHora { get; set; }
        public IEnumerable<int> IdsAsignaturasOfertadasAdicionadas { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
