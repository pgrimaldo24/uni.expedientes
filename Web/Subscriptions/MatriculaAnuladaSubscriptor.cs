using MediatR;
using System.Threading.Tasks;
using System;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Framework.Crosscutting.Bus;
using Unir.Expedientes.Application.Matriculacion.MatriculaAnulada.Commands;
using System.Linq;
using Unir.Expedientes.WebUi.Model.Subscriptions;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaAnuladaSubscriptor : ISubscriber<MatriculaAnulada>
    {
        private readonly IMediator _mediator;
        public MatriculaAnuladaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(MessageContext<MatriculaAnulada> message)
        {
            var dataMessage = message.Message;

            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            if (dataMessage.AsignaturasAnuladas is null || !dataMessage.AsignaturasAnuladas.Any())
                throw new ArgumentNullException(nameof(dataMessage.AsignaturasAnuladas));

            var request = new CreateMatriculaAnuladaCommand
            {
                IdTipoBaja = dataMessage.IdTipoBaja,
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                IdsAsignaturasOfertadas = dataMessage.AsignaturasAnuladas.Select(an => an.IdAsignaturaOfertada).ToList(),
                IdCausaBaja = dataMessage.IdCausaBaja,
                FechaHoraBaja = dataMessage.FechaHoraBaja,
                Mensaje = MensajesRabbit.MatriculaAnulada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }
    }
}
