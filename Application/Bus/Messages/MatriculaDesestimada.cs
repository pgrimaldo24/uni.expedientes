using System;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaDesestimada : MatriculaBaseMessage
    {
        public int? IdCausa { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
