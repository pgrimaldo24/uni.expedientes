using System;
using System.Collections.Generic;
using MediatR;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaAnulada.Commands
{
    public class CreateMatriculaAnuladaCommand : IRequest
    {
        public int IdTipoBaja { get; set; }
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public List<int> IdsAsignaturasOfertadas { get; set; }
        public DateTime FechaHoraBaja { get; set; }
        public int IdCausaBaja { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
