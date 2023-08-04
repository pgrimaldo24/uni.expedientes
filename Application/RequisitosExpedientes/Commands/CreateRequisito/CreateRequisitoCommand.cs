using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Commands.CreateRequisito
{
    public class CreateRequisitoCommand : IRequest<int>
    {
        public string Nombre { get; set; }
        public int Orden { get; set; }
    }
}
