using System;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaRecuperada : MatriculaBaseMessage
    {
        public AsignaturaMatriculada[] AsignaturasActivadas { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
