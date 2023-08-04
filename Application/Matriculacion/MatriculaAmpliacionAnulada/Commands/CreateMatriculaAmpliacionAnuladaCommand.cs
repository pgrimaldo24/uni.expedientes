using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionAnulada.Commands
{
    public class CreateMatriculaAmpliacionAnuladaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public IEnumerable<int> IdsAsignaturasOfertadasAdicionadas { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
