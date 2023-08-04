using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.AsignaturasExpediente.Commands.EditAsignaturaExpedienteToSeguimientoByNotaFinal
{
    public class EditAsignaturaExpedienteToSeguimientoByNotaFinalCommandHandler : IRequestHandler<EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand>
    {
        private readonly IExpedientesContext _context;

        public EditAsignaturaExpedienteToSeguimientoByNotaFinalCommandHandler(IExpedientesContext context)
        {
            _context = context;
        }


        public async Task<Unit> Handle(EditAsignaturaExpedienteToSeguimientoByNotaFinalCommand request, CancellationToken cancellationToken)
        {
            var asignaturaExpediente = await 
                _context.AsignaturasExpedientes.FirstOrDefaultAsync(ae => ae.Id == request.IdAsignaturaExpedienteAlumno, cancellationToken);

            if (asignaturaExpediente is null)
            {
                return Unit.Value;
            }

            var idSituacionAsignatura = request.EsSuperada ? SituacionAsignatura.Superada :
                request.EsMatriculaHonor ? SituacionAsignatura.MatriculaHonor :
                request.EsNoPresentado ? SituacionAsignatura.NoPresentada : SituacionAsignatura.NoSuperada; 

            asignaturaExpediente.SituacionAsignaturaId = idSituacionAsignatura;
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
