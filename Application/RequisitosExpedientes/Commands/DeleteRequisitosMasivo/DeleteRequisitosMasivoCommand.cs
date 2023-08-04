using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Commands.DeleteRequisitosMasivo
{
    public class DeleteRequisitosMasivoCommand : IRequest<IList<string>>
    {
        public ICollection<int> IdsRequisitos { get; set; }
    }
}
