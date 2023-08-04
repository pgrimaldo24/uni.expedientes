using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteComportamientosExpedientesMasivo
{
    public class DeleteComportamientosExpedientesMasivoCommand : IRequest<IList<string>>
    {
        public ICollection<int> IdsComportamientos { get; set; }
    }
}
