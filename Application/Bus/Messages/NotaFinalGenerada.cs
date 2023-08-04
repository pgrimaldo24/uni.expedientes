using System.Collections.Generic;
using Unir.Expedientes.Application.Bus.Messages.Common;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.Application.Bus.Messages
{
    public class NotaFinalGenerada : IMessage
    {
        public string Plataforma { get; set; }
        public bool Provisional { get; set; }
        public int IdCurso { get; set; }
        public int IdUsuarioPublicadorConfirmador { get; set; }
        public List<NotaCommonStruct> Notas { get; set; }
    }
}