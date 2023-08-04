using System;
using System.Collections.Generic;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaAmpliacionAnulada : MatriculaAmpliacionBaseMessage
    {
        public IEnumerable<int> IdsAsignaturasOfertadasAdicionadas { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
