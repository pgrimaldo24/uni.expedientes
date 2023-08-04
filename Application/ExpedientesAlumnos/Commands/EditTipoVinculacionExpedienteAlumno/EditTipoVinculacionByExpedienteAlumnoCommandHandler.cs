using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditTipoVinculacionExpedienteAlumno;
public class EditTipoVinculacionByExpedienteAlumnoCommandHandler : IRequestHandler<EditTipoVinculacionByExpedienteAlumnoCommand, bool>
{
    private readonly IExpedientesContext _context;

    public EditTipoVinculacionByExpedienteAlumnoCommandHandler(IExpedientesContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(EditTipoVinculacionByExpedienteAlumnoCommand request,
        CancellationToken cancellationToken)
    {
        var expedienteAlumno =
            await _context.ExpedientesAlumno
                .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
        if (expedienteAlumno == null)
            throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

        if (expedienteAlumno.IdRefTipoVinculacion == request.IdRefTipoVinculacion)
            return false;

        var descripcionSeguimiento =
            $"Tipo de Vinculación anterior con id.: {expedienteAlumno.IdRefTipoVinculacion}. Por Integración.";
        expedienteAlumno.IdRefTipoVinculacion = request.IdRefTipoVinculacion;
        expedienteAlumno.AddSeguimientoNoUser(TipoSeguimientoExpediente.ExpedienteModificadoTipoVinculacion,
            descripcionSeguimiento);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}