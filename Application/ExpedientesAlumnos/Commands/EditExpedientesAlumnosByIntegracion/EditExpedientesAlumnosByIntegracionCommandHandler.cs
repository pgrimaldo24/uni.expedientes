using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedientesAlumnosByIntegracion
{
    public class EditExpedientesAlumnosByIntegracionCommandHandler : IRequestHandler<EditExpedientesAlumnosByIntegracionCommand>
    {
        private readonly IExpedientesContext _context;

        public EditExpedientesAlumnosByIntegracionCommandHandler(IExpedientesContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(EditExpedientesAlumnosByIntegracionCommand request, CancellationToken cancellationToken)
        {
            if (!request.ExpedientesAlumnos.Any())
                return Unit.Value;
            var idsExpedientes = request.ExpedientesAlumnos.Select(x => x.Id).ToArray();
            var expedientesAlumnos = await _context.ExpedientesAlumno.Where(ea => idsExpedientes.Contains(ea.Id))
                .ToListAsync(cancellationToken);
            foreach (var expedienteAlumnoRequest in request.ExpedientesAlumnos)
            {
                var expedienteAlumno = expedientesAlumnos.FirstOrDefault(ea => ea.Id == expedienteAlumnoRequest.Id);
                if (expedienteAlumno == null)
                    continue;

                expedienteAlumno.IdRefVersionPlan = expedienteAlumnoRequest.IdRefVersionPlan;
                var nroVersion = !string.IsNullOrWhiteSpace(expedienteAlumnoRequest.IdRefVersionPlan)
                    ? expedienteAlumnoRequest.NroVersion?.ToString()
                    : "-";
                var descripcion = $"Versión del Plan {nroVersion}. Por Integración.";
                expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteModificadoVersionPlan, descripcion);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
