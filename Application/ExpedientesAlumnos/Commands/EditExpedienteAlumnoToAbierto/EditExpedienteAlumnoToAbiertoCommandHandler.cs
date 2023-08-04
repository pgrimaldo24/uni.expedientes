using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.EditExpedienteAlumnoToAbierto;

public class EditExpedienteAlumnoToAbiertoCommandHandler : IRequestHandler<EditExpedienteAlumnoToAbiertoCommand>
{
    private readonly IExpedientesContext _context;

    public EditExpedienteAlumnoToAbiertoCommandHandler(IExpedientesContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(EditExpedienteAlumnoToAbiertoCommand request, CancellationToken cancellationToken)
    {
        var expedienteAlumno =
            await _context.ExpedientesAlumno
                .FirstOrDefaultAsync(ea => ea.Id == request.Id, cancellationToken);
        if (expedienteAlumno == null)
            throw new NotFoundException(nameof(ExpedienteAlumno), request.Id);

        expedienteAlumno.Estado = await _context.EstadosExpedientes.FirstAsync(e => e.Id == EstadoExpediente.Abierto, cancellationToken);
        expedienteAlumno.FechaApertura = request.FechaApertura ?? expedienteAlumno.FechaApertura;
        await AddHitosConseguidos(request.FechaRecepcionMensaje, expedienteAlumno, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }

    protected internal virtual async Task AddHitosConseguidos(DateTime? fechaInicio, ExpedienteAlumno expediente, CancellationToken cancellationToken)
    {
        if (fechaInicio == null) return;
        var tipoHitoConseguido = TipoHitoConseguido.PrimeraMatricula;
        var tipoHitoConseguidoExpediente = await _context.TiposHitoConseguidos.FirstAsync(t => t.Id == tipoHitoConseguido, cancellationToken);
        expediente.AddHitosConseguidos(tipoHitoConseguidoExpediente, fechaInicio.Value);
    }
}