using System;
using Unir.Expedientes.Application.Bus.Messages.Common;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class MatriculaRealizada : MatriculaBaseMessage
    {
        public DateTime? FechaRecepcion { get; set; }
    }
}
