using System;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaAnulada : MatriculaBaseMessage
    {
        public int IdTipoBaja { get; set; }
        public AsignaturaMatriculada[] AsignaturasAnuladas { get; set; }
        public int IdCausaBaja { get; set; }
        public DateTime FechaHoraBaja { get; set; }
    }
}
