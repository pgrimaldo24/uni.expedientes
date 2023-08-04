using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
namespace Unir.Expedientes.Application.Anotaciones.Commands.DeleteAnotacion
{
    public class DeleteAnotacionCommandHandler : IRequestHandler<DeleteAnotacionCommand>
    {
        private readonly IExpedientesContext _context;

        public DeleteAnotacionCommandHandler(IExpedientesContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteAnotacionCommand request, CancellationToken cancellationToken)
        {
            var anotacion = await _context.Anotaciones
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);
            if (anotacion == null)
                throw new NotFoundException(nameof(Anotacion), request.Id);

            _context.Remove(anotacion);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
