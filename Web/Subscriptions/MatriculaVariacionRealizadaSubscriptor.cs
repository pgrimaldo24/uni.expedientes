using MediatR;
using System;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Matriculacion.MatriculaVariacionRealizada.Commands;
using Unir.Expedientes.WebUi.Model.Subscriptions;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaVariacionRealizadaSubscriptor : ISubscriber<MatriculaVariacionRealizada>
    {
        private readonly IMediator _mediator;
        public MatriculaVariacionRealizadaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(MessageContext<MatriculaVariacionRealizada> message)
        {
            var dataMessage = message.Message;

            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            if (dataMessage.IdsAsignaturasOfertadasAdicionadas is null)
                throw new ArgumentNullException(nameof(dataMessage.IdsAsignaturasOfertadasAdicionadas));

            if (dataMessage.IdsAsignaturasOfertadasExcluidas is null)
                throw new ArgumentNullException(nameof(dataMessage.IdsAsignaturasOfertadasExcluidas));

            if (string.IsNullOrEmpty(dataMessage.CausaEnumDominio))
                throw new ArgumentNullException(nameof(dataMessage.CausaEnumDominio));

            var request = new CreateMatriculaVariacionRealizadaCommand
            {
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                CausaEnumDominio = dataMessage.CausaEnumDominio,
                IdsAsignaturasOfertadasAdicionadas = dataMessage.IdsAsignaturasOfertadasAdicionadas,
                IdsAsignaturasOfertadasExcluidas = dataMessage.IdsAsignaturasOfertadasExcluidas,
                FechaHoraAlta = dataMessage.FechaHoraAlta,
                Mensaje = MensajesRabbit.MatriculaVariacionRealizada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }
    }
}
