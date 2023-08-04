using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditViaAccesoPlanExpedienteAlumno;

public class EditViaAccesoPlanExpedienteAlumnoCommandHandler : IRequestHandler<EditViaAccesoPlanExpedienteAlumnoCommand>
{
    private readonly IExpedientesContext _context;

    public EditViaAccesoPlanExpedienteAlumnoCommandHandler(IExpedientesContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(EditViaAccesoPlanExpedienteAlumnoCommand request,
        CancellationToken cancellationToken)
    {
        var expedienteAlumno =
            await _context.ExpedientesAlumno
                .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
        if (expedienteAlumno == null)
            throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

        var descripcionSeguimiento =
            $"Vía de Acceso Plan anterior con id.: {expedienteAlumno.IdRefViaAccesoPlan}. Por Integración.";
        expedienteAlumno.IdRefViaAccesoPlan = request.IdRefViaAccesoPlan;
        expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteModificadoViaAcceso,
            descripcionSeguimiento);
        await _context.SaveChangesAsync(cancellationToken);
        return await Task.FromResult(Unit.Value);
    }
}