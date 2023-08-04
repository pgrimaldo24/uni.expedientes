using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.Tareas.Commands.EditTarea
{
    public class EditTareaCommandHandler : IRequestHandler<EditTareaCommand>
    {
        private readonly IExpedientesContext _context;
        public EditTareaCommandHandler(IExpedientesContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(EditTareaCommand request, CancellationToken cancellationToken)
        {
            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.Id == request.IdTarea, cancellationToken);
            if (tarea is null)
                throw new NotFoundException(nameof(Tarea), request.IdTarea);

            AssignTarea(request, tarea);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual void AssignTarea(EditTareaCommand editTareaCommand, Tarea tarea)
        {
            tarea.FechaFin = editTareaCommand.FechaFin;
            tarea.Total = editTareaCommand.Total;
            tarea.Completadas = editTareaCommand.Completadas;
            tarea.Fallidas = editTareaCommand.Fallidas;
        }
    }
}
