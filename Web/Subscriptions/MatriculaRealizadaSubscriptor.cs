using MediatR;
using System;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Matriculacion.MatriculaRealizada.Commands;
using Unir.Expedientes.WebUi.Model.Subscriptions;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaRealizadaSubscriptor : ISubscriber<MatriculaRealizada>
    {
        private readonly IMediator _mediator;
        public MatriculaRealizadaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task HandleAsync(MessageContext<MatriculaRealizada> message)
        {
            var dataMessage = message.Message;

            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            var request = new CreateMatriculaRealizadaCommand
            {
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                FechaRecepcion = dataMessage.FechaRecepcion ?? DateTime.UtcNow,
                Mensaje = MensajesRabbit.MatriculaRealizada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }
    }
}
