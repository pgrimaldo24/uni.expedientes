using System;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaAmpliacionDesestimada : MatriculaAmpliacionBaseMessage
    {
        public string Motivo { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
