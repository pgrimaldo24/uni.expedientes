using System;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaAmpliacionReiniciada : MatriculaAmpliacionBaseMessage
    {
        public DateTime FechaHora { get; set; }
    }
}
