using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.TiposSituacionEstadoExpedientes.Commands.CreateTipoSituacionEstadoExpediente
{
    public class CreateTipoSituacionEstadoExpedienteCommandHandler : IRequestHandler<CreateTipoSituacionEstadoExpedienteCommand>
    {
        private readonly IExpedientesContext _context;

        public CreateTipoSituacionEstadoExpedienteCommandHandler(IExpedientesContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CreateTipoSituacionEstadoExpedienteCommand request, CancellationToken cancellationToken)
        {
            var tipoSituacionEstado = await _context.TiposSituacionEstado
                .FirstOrDefaultAsync(tse => tse.Id == request.TipoSituacionEstadoId, cancellationToken);
            if (tipoSituacionEstado == null)
                throw new NotFoundException(nameof(TipoSituacionEstado), request.TipoSituacionEstadoId);

            var expedienteAlumno = await _context.ExpedientesAlumno
                .FirstOrDefaultAsync(tse => tse.Id == request.ExpedienteAlumnoId, cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.ExpedienteAlumnoId);

            var tipoSituacionEstadoExpediente = AssignTipoSituacionEstadoExpediente(tipoSituacionEstado, expedienteAlumno, request);
            await _context.TiposSituacionEstadoExpedientes.AddAsync(tipoSituacionEstadoExpediente, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        protected internal virtual TipoSituacionEstadoExpediente AssignTipoSituacionEstadoExpediente(
            TipoSituacionEstado tipoSituacionEstado, ExpedienteAlumno expedienteAlumno,
            CreateTipoSituacionEstadoExpedienteCommand request)
        {
            return new TipoSituacionEstadoExpediente
            {
                FechaInicio = request.FechaInicio,
                FechaFin = request.FechaFin,
                Descripcion = request.Descripcion,
                TipoSituacionEstado = tipoSituacionEstado,
                ExpedienteAlumno = expedienteAlumno
            };
        }
    }
}
