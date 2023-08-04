using MediatR;
using System.Collections.Generic;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Commands.DeleteRequisitosComportamientosExpedientesMasivo
{
    public class DeleteRequisitosComportamientosExpedientesMasivoCommand : IRequest
    {
        public ICollection<int> IdsRequisitosComportamientos { get; set; }
    }
}
