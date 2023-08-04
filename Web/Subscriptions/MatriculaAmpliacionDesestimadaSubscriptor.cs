using MediatR;
using System;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaDesestimadaCommon;
using Unir.Expedientes.WebUi.Model.Subscriptions;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaAmpliacionDesestimadaSubscriptor : ISubscriber<MatriculaAmpliacionDesestimada>
    {
        private readonly IMediator _mediator;

        public MatriculaAmpliacionDesestimadaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(MessageContext<MatriculaAmpliacionDesestimada> message)
        {
            var dataMessage = message.Message;

            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));
            
            var request = new CreateMatriculaDesestimadaCommonCommand
            {
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                Motivo = dataMessage.Motivo,
                FechaHora = dataMessage.FechaHora,
                EsAmpliacion = true,
                Mensaje = MensajesRabbit.MatriculaAmpliacionDesestimada,
                Origen = MensajesRabbit.Origen
            };

            await _mediator.Send(request);
        }

    }
}
