using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Matriculacion.MatriculaAmpliacionRealizada.Commands;
using Unir.Expedientes.WebUi.Model.Subscriptions;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaAmpliacionRealizadaSubscriptor : ISubscriber<MatriculaAmpliacionRealizada>
    {
        private readonly IMediator _mediator;
        public MatriculaAmpliacionRealizadaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(MessageContext<MatriculaAmpliacionRealizada> message)
        {
            var dataMessage = message.Message;

            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            if (dataMessage.IdsAsignaturasOfertadasAdicionadas is null || !dataMessage.IdsAsignaturasOfertadasAdicionadas.Any())
                throw new ArgumentNullException(nameof(dataMessage.IdsAsignaturasOfertadasAdicionadas));

            var request = new CreateMatriculaAmpliacionRealizadaCommand
            {
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                IdsAsignaturasOfertadasAdicionadas = dataMessage.IdsAsignaturasOfertadasAdicionadas,
                FechaHoraAlta = dataMessage.FechaHoraAlta,
                Mensaje = MensajesRabbit.MatriculaAmpliacionRealizada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }
    }
}
