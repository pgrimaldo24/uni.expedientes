using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.HasBloqueoExpedienteAlumno
{
    public class HasBloqueoExpedienteAlumnoCommandHandler : IRequestHandler<HasBloqueoExpedienteAlumnoCommand, bool>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<HasBloqueoExpedienteAlumnoCommandHandler> _localizer;
        private readonly IExpedientesGestorUnirServiceClient _expedientesGestorUnirServiceClient;

        public HasBloqueoExpedienteAlumnoCommandHandler(IExpedientesContext context,
            IStringLocalizer<HasBloqueoExpedienteAlumnoCommandHandler> localizer,
            IExpedientesGestorUnirServiceClient expedientesGestorUnirServiceClient)
        {
            _context = context;
            _localizer = localizer;
            _expedientesGestorUnirServiceClient = expedientesGestorUnirServiceClient;
        }

        public async Task<bool> Handle(HasBloqueoExpedienteAlumnoCommand request, CancellationToken cancellationToken)
        {
            var expedienteAlumno =
                await _context.ExpedientesAlumno.FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno,
                    cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            if (!int.TryParse(expedienteAlumno.IdRefIntegracionAlumno, out _))
                throw new BadRequestException(_localizer[$"El valor {nameof(expedienteAlumno.IdRefIntegracionAlumno)} del Expediente es inválido"]);

            if (!int.TryParse(expedienteAlumno.IdRefPlan, out _))
                throw new BadRequestException(_localizer[$"El valor {nameof(expedienteAlumno.IdRefPlan)} del Expediente es inválido"]);

            var resultBloqueo = await _expedientesGestorUnirServiceClient.GetBloqueoExpediente(
                int.Parse(expedienteAlumno.IdRefIntegracionAlumno), int.Parse(expedienteAlumno.IdRefPlan));

            const int codigoResultadoOk = 0;
            return resultBloqueo.CodigoResultado == codigoResultadoOk && resultBloqueo.Bloqueado;
        }
    }
}
