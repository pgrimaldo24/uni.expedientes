using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.TareasDetalle.Commands.CreateTareaDetalle
{
    public class CreateTareaDetalleCommandHandler : IRequestHandler<CreateTareaDetalleCommand>
    {
        private readonly IExpedientesContext _context;
        public CreateTareaDetalleCommandHandler(IExpedientesContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(CreateTareaDetalleCommand request, CancellationToken cancellationToken)
        {
            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == request.IdTarea, cancellationToken);
            if (tarea is null)
                throw new NotFoundException(nameof(Tarea), request.IdTarea);

            var expedienteAlumno = await _context.ExpedientesAlumno.FirstOrDefaultAsync(s => s.Id == request.IdExpediente, cancellationToken);
            if (expedienteAlumno is null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpediente);

            var nuevaTareaDetalle = AssignTareaDetalle(request, tarea, expedienteAlumno);
            await _context.TareasDetalle.AddAsync(nuevaTareaDetalle, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual TareaDetalle AssignTareaDetalle(CreateTareaDetalleCommand createTareaDetalleCommand, Tarea tarea, ExpedienteAlumno expedienteAlumno)
        {
            return new TareaDetalle
            {
                Tarea = tarea,
                Expediente = expedienteAlumno,
                FechaInicio = createTareaDetalleCommand.FechaInicio,
                FechaFin = createTareaDetalleCommand.FechaFin,
                CompletadaOk = createTareaDetalleCommand.CompletadaOk,
                Mensaje = createTareaDetalleCommand.Mensaje
            };
        }
    }
}
