using MediatR;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetProcesosEnSegundoPlano;

namespace Unir.Expedientes.Application.Hub
{
    public class ExpedientesHub : Hub<IExpedientesClient>
    {
        private readonly IMediator _mediator;
        public ExpedientesHub(IMediator mediator)
        {
            _mediator = mediator;
        }
        public override async Task OnConnectedAsync()
        {
            var mensaje = await _mediator.Send(new GetProcesosEnSegundoPlanoQuery());
            await Clients.Caller.StatusJobs(mensaje);
        }
    }
}
