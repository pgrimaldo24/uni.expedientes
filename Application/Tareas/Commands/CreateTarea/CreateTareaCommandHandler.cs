using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Tareas.Commands.CreateTarea
{
    public class CreateTareaCommandHandler : IRequestHandler<CreateTareaCommand, Tarea>
    {
        private readonly IExpedientesContext _context;
        public CreateTareaCommandHandler(IExpedientesContext context)
        {
            _context = context;
        }
        public async Task<Tarea> Handle(CreateTareaCommand request, CancellationToken cancellationToken)
        {
            var tarea = AssignTarea(request);
            var created = await _context.Tareas.AddAsync(tarea, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return created.Entity;
        }

        protected internal virtual Tarea AssignTarea(CreateTareaCommand createTareaCommand)
        {
            return new Tarea
            {
                JobId = createTareaCommand.JobId,
                FechaInicio = createTareaCommand.FechaInicio,
                FechaFin = createTareaCommand.FechaFin,
                Total = createTareaCommand.Total,
                Completadas = createTareaCommand.Completadas,
                Fallidas = createTareaCommand.Fallidas,
                IdRefUniversidad = createTareaCommand.IdRefUniversidad,
                IdRefEstudio = createTareaCommand.IdRefEstudio
            };
        }
    }
}
