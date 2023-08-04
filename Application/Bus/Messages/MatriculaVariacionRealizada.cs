using System;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaVariacionRealizada : MatriculaVariacionBaseMessage
    {
        public int[] IdsAsignaturasOfertadasAdicionadas { get; set; }
        public int[] IdsAsignaturasOfertadasExcluidas { get; set; }
        public string CausaEnumDominio { get; set; }
        public DateTime FechaHoraAlta { get; set; }
    }
}
