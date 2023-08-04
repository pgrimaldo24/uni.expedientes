using System;
using System.Linq;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Bus.Messages;
using Unir.Framework.Crosscutting.Bus;
using MediatR;
using Unir.Expedientes.Application.NotaFinalGenerada;
namespace Unir.Expedientes.WebUi.Subscriptions
{
    public class NotaFinalGeneradaSubscriptor : ISubscriber<NotaFinalGenerada>
    {
        private readonly IMediator _mediator;
        public NotaFinalGeneradaSubscriptor(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task HandleAsync(MessageContext<NotaFinalGenerada> message)
        {
            var dataMessage = message.Message;
            if (!dataMessage.Notas.Any())
                throw new ArgumentNullException("No hay notas en la Nota Final Generada");
            var request = new NotaFinalGeneradaCommand()
            {
                Plataforma = dataMessage.Plataforma,
                Provisional = dataMessage.Provisional,
                IdCurso = dataMessage.IdCurso,
                IdUsuarioPublicadorConfirmador = dataMessage.IdUsuarioPublicadorConfirmador,
                Notas = dataMessage.Notas.Select(n => new NotaCommonStruct
                {
                    IdAlumno = n.IdAlumno,
                    Convocatoria = n.Convocatoria,
                    Calificacion = n.Calificacion,
                    EsMatriculaHonor = n.EsMatriculaHonor,
                    NoPresentado = n.NoPresentado,
                    FechaPublicado = n.FechaPublicado,
                    FechaConfirmado = n.FechaConfirmado
                }).ToList()
            };
            await _mediator.Send(request);
        }
    }
}