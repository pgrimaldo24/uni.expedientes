using MediatR;
using System;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Matriculacion.MatriculaDesestimada.Commands;
using Unir.Expedientes.WebUi.Model.Subscriptions;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaDesestimadaSubscriptor : ISubscriber<MatriculaDesestimada>
    {
        private readonly IMediator _mediator;
        public MatriculaDesestimadaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(MessageContext<MatriculaDesestimada> message)
        {
            var dataMessage = message.Message;

            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            var request = new CreateMatriculaDesestimadaCommand
            {
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                IdCausa = dataMessage.IdCausa,
                FechaHora = dataMessage.FechaHora,
                Mensaje = MensajesRabbit.MatriculaDesestimada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }
    }
}
