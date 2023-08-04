using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaAnuladaCommon
{
    public class CreateMatriculaAnuladaCommonCommand : IRequest
    {
        public string MatriculaIdIntegracion { get; set; }
        public string AlumnoIdIntegracion { get; set; }
        public int IdTipoBaja { get; set; }
        public int IdCausaBaja { get; set; }
        public List<int> IdsAsignaturasOfertadas { get; set; }
        public bool IsAmpliacion { get; set; }
        public string? Mensaje { get; set; }
        public string? Origen { get; set; }
    }
}
