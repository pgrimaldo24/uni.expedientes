using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Matriculacion.MatriculaVariacionAnulada.Commands;
using Unir.Expedientes.WebUi.Model.Subscriptions;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaVariacionAnuladaSubscriptor : ISubscriber<MatriculaVariacionAnulada>
    {
        private readonly IMediator _mediator;
        public MatriculaVariacionAnuladaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task HandleAsync(MessageContext<MatriculaVariacionAnulada> message)
        {
            var dataMessage = message.Message;

            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            if ((dataMessage.IdsAsignaturasOfertadasAdicionadas is null || !dataMessage.IdsAsignaturasOfertadasAdicionadas.Any()) &&
                (dataMessage.IdsAsignaturasOfertadasExcluidas is null || !dataMessage.IdsAsignaturasOfertadasExcluidas.Any()))
                throw new ArgumentNullException("Debe ingresar asignaturas en el listado de asignaturas adicionadas o excluidas");

            var request = new CreateMatriculaVariacionAnuladaCommand
            {
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                IdsAsignaturasOfertadasAdicionadas = dataMessage.IdsAsignaturasOfertadasAdicionadas is null ? new List<int>() : dataMessage.IdsAsignaturasOfertadasAdicionadas.ToList(),
                IdsAsignaturasOfertadasExcluidas = dataMessage.IdsAsignaturasOfertadasExcluidas is null ? new List<int>() : dataMessage.IdsAsignaturasOfertadasExcluidas.ToList(),
                FechaHora = dataMessage.FechaHora,
                Mensaje = MensajesRabbit.MatriculaVariacionAnulada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }
    }
}
