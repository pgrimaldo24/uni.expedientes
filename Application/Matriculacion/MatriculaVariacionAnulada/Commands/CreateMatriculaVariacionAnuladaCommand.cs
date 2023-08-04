using DocumentFormat.OpenXml.Office2010.ExcelAc;
using MediatR;
using System;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Matriculacion.MatriculaVariacionAnulada.Commands
{
    public class CreateMatriculaVariacionAnuladaCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public List<int> IdsAsignaturasOfertadasExcluidas { get; set; }
        public List<int> IdsAsignaturasOfertadasAdicionadas { get; set; }
        public DateTime FechaHora { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
