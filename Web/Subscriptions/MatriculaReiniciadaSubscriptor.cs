using MediatR;
using System.Threading.Tasks;
using System;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Matriculacion.MatriculaReiniciada.Commands;
using Unir.Framework.Crosscutting.Bus;
using Unir.Expedientes.WebUi.Model.Subscriptions;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaReiniciadaSubscriptor : ISubscriber<MatriculaReiniciada>
    {
        private readonly IMediator _mediator;
        public MatriculaReiniciadaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(MessageContext<MatriculaReiniciada> message)
        {
            var dataMessage = message.Message;

            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            var request = new CreateMatriculaReiniciadaCommand
            {
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                FechaHora = dataMessage.FechaHora,
                Mensaje = MensajesRabbit.MatriculaReiniciada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }
    }
}
