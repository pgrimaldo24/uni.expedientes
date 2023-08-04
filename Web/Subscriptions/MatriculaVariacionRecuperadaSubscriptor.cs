using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Expedientes.Application.Matriculacion.MatriculaVariacionRecuperada;
using Unir.Expedientes.WebUi.Model.Subscriptions;
using Unir.Framework.Crosscutting.Bus;

namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class MatriculaVariacionRecuperadaSubscriptor : ISubscriber<MatriculaVariacionRecuperada>
    {
        private readonly IMediator _mediator;
        public MatriculaVariacionRecuperadaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleAsync(MessageContext<MatriculaVariacionRecuperada> message)
        {
            var dataMessage = message.Message;

            if (string.IsNullOrEmpty(dataMessage.AlumnoIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.AlumnoIdIntegracion));

            if (string.IsNullOrEmpty(dataMessage.MatriculaIdIntegracion))
                throw new ArgumentNullException(nameof(dataMessage.MatriculaIdIntegracion));

            if ((dataMessage.IdsAsignaturasOfertadasAdicionadas is null || !dataMessage.IdsAsignaturasOfertadasAdicionadas.Any()) && 
                (dataMessage.IdsAsignaturasOfertadasExcluidas is null || !dataMessage.IdsAsignaturasOfertadasExcluidas.Any()))
                throw new ArgumentNullException("Debe ingresar asignaturas en el listado de asignaturas adicionadas o excluidas");

            var request = new CreateMatriculaVariacionRecuperadaCommand
            {
                MatriculaIdIntegracion = dataMessage.MatriculaIdIntegracion,
                AlumnoIdIntegracion = dataMessage.AlumnoIdIntegracion,
                IdsAsignaturasOfertadasAdicionadas = (dataMessage.IdsAsignaturasOfertadasAdicionadas is null || !dataMessage.IdsAsignaturasOfertadasAdicionadas.Any()) ? new List<int>() : dataMessage.IdsAsignaturasOfertadasAdicionadas.ToList(),
                IdsAsignaturasOfertadasExcluidas = (dataMessage.IdsAsignaturasOfertadasExcluidas is null || !dataMessage.IdsAsignaturasOfertadasExcluidas.Any())  ? new List<int>() : dataMessage.IdsAsignaturasOfertadasExcluidas.ToList(),
                FechaHora = dataMessage.FechaHora,
                IdVariacion = dataMessage.IdVariacion,
                VariacionIdIntegracion = dataMessage.VariacionIdIntegracion,
                Mensaje = MensajesRabbit.MatriculaVariacionRecuperada,
                Origen = MensajesRabbit.Origen
            };
            await _mediator.Send(request);
        }
    }
}
