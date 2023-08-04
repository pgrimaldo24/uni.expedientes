using System;
using System.Collections.Generic;
using MediatR;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaVariacionRecuperada
{
    public class CreateMatriculaVariacionRecuperadaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public int IdVariacion { get; set; }
        public string VariacionIdIntegracion { get; set; }
        public IEnumerable<int> IdsAsignaturasOfertadasExcluidas { get; set; }
        public IEnumerable<int> IdsAsignaturasOfertadasAdicionadas { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
