using MediatR;
using System;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Matriculacion.Common.Commands.MatriculaDesestimadaCommon;
using Unir.Expedientes.WebUi.Model.Subscriptions;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaVariacionDesestimadaSubscriptor : ISubscriber<MatriculaVariacionDesestimada>
    {
        private readonly IMediator _mediator;
        public MatriculaVariacionDesestimadaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task HandleAsync(MessageContext<MatriculaVariacionDesestimada> message)
        {
            var dataMessage = message.Message;

            if (dataMessage.AlumnoIdIntegracion == null)
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (dataMessage.MatriculaIdIntegracion == null)
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            var request = new CreateMatriculaDesestimadaCommonCommand
            {
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                UniversidadIdIntegracion = dataMessage.UniversidadIdIntegracion,
                IdVariacion = dataMessage.IdVariacion,
                VariacionIdIntegracion = dataMessage.VariacionIdIntegracion,
                Motivo = dataMessage.Motivo,
                FechaHora = dataMessage.FechaHora,
                EsAmpliacion = false,
                Mensaje = MensajesRabbit.MatriculaVariacionDesestimada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }
    }
}
