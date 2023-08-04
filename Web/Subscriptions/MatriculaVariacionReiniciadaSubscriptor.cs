using MediatR;
using System;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Matriculacion.MatriculaVariacionReiniciada.Commands;
using Unir.Expedientes.WebUi.Model.Subscriptions;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaVariacionReiniciadaSubscriptor : ISubscriber<MatriculaVariacionReiniciada>
    {
        private readonly IMediator _mediator;
        public MatriculaVariacionReiniciadaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(MessageContext<MatriculaVariacionReiniciada> message)
        {
            var dataMessage = message.Message;
            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            var request = new CreateMatriculaVariacionReiniciadaCommand
            {
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                FechaHora = dataMessage.FechaHora,
                Mensaje = MensajesRabbit.MatriculaVariacionReiniciada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }

    }
}
