using MediatR;
using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionRealizada.Commands
{
    public class CreateMatriculaAmpliacionRealizadaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public List<int> IdsAsignaturasOfertadasAdicionadas { get; set; }
        public DateTime FechaHoraAlta { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
