using System;
using System.Collections.Generic;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaAmpliacionRealizada : MatriculaAmpliacionBaseMessage
    {
        public List<int> IdsAsignaturasOfertadasAdicionadas { get; set; }
        public DateTime FechaHoraAlta { get; set; }
    }
}
