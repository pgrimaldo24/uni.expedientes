using MediatR;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Commands.AddConsolidacionesRequisitosExpedienteUncommit
{
    public class AddConsolidacionesRequisitosExpedienteUncommitCommand : IRequest
    {
        public ExpedienteAlumno ExpedienteAlumno { get; set; }
        public AddConsolidacionesRequisitosExpedienteUncommitCommand(
            ExpedienteAlumno expedienteAlumno)
        {
            ExpedienteAlumno = expedienteAlumno;
        }
    }
}
