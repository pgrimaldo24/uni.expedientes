using MediatR;
using System.Threading.Tasks;
using System;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Framework.Crosscutting.Bus;
using Unir.Expedientes.Application.Matriculacion.MatriculaRecuperada.Commands;
using System.Linq;
using Unir.Expedientes.WebUi.Model.Subscriptions;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaRecuperadaSubscriptor : ISubscriber<MatriculaRecuperada>
    {
        private readonly IMediator _mediator;
        public MatriculaRecuperadaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task HandleAsync(MessageContext<MatriculaRecuperada> message)
        {
            var dataMessage = message.Message;

            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            if (dataMessage.AsignaturasActivadas is null || !dataMessage.AsignaturasActivadas.Any())
                throw new ArgumentNullException(nameof(dataMessage.AsignaturasActivadas));

            var request = new CreateMatriculaRecuperadaCommand
            {
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                IdsAsignaturasOfertadas = dataMessage.AsignaturasActivadas.Select(ac => ac.IdAsignaturaOfertada).ToList(),
                FechaHora = dataMessage.FechaHora,
                Mensaje = MensajesRabbit.MatriculaRecuperada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }

    }
}
