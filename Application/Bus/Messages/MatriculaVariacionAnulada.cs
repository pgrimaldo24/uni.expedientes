using System;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaVariacionAnulada : MatriculaVariacionBaseMessage
    {
        public int[] IdsAsignaturasOfertadasExcluidas { get; set; }
        public int[] IdsAsignaturasOfertadasAdicionadas { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
