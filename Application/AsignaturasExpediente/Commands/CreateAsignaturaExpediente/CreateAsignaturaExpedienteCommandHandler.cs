using System.Linq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.AsignaturasExpediente.Commands.CreateAsignaturaExpediente
{
    public class CreateAsignaturaExpedienteCommandHandler : IRequestHandler<CreateAsignaturaExpedienteCommand, int>
    {
        private readonly IExpedientesContext _context;
        private readonly IStringLocalizer<CreateAsignaturaExpedienteCommandHandler> _localizer;
        public CreateAsignaturaExpedienteCommandHandler(
            IExpedientesContext context,
            IStringLocalizer<CreateAsignaturaExpedienteCommandHandler> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public async Task<int> Handle(CreateAsignaturaExpedienteCommand request, CancellationToken cancellationToken)
        {
            var newAsignatura = AssignNewAsignaturaExpendiente(request);

            await ValidateProperties(newAsignatura, request, cancellationToken);

            await _context.AsignaturasExpedientes.AddAsync(newAsignatura, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return newAsignatura.Id;
        }

        protected internal virtual AsignaturaExpediente AssignNewAsignaturaExpendiente(CreateAsignaturaExpedienteCommand request)
        {
            return new AsignaturaExpediente
            {
                IdRefAsignaturaPlan = request.IdRefAsignaturaPlan,
                NombreAsignatura = request.NombreAsignatura,
                CodigoAsignatura = request.CodigoAsignatura,
                OrdenAsignatura = request.OrdenAsignatura,
                Ects = request.Ects,
                IdRefTipoAsignatura = request.IdRefTipoAsignatura,
                SimboloTipoAsignatura = request.SimboloTipoAsignatura,
                OrdenTipoAsignatura = request.OrdenTipoAsignatura,
                NombreTipoAsignatura = request.NombreTipoAsignatura,
                IdRefCurso = request.IdRefCurso,
                NumeroCurso = request.NumeroCurso,
                AnyoAcademicoInicio = request.AnyoAcademicoInicio,
                AnyoAcademicoFin = request.AnyoAcademicoFin,
                PeriodoLectivo = request.PeriodoLectivo,
                DuracionPeriodo = request.DuracionPeriodo,
                SimboloDuracionPeriodo = request.SimboloDuracionPeriodo,
                IdRefIdiomaImparticion = request.IdRefIdiomaImparticion,
                SimboloIdiomaImparticion = request.SimboloIdiomaImparticion,
                Reconocida = request.Reconocida,
                SituacionAsignaturaId = request.SituacionAsignaturaId,
                ExpedienteAlumnoId = request.ExpedienteAlumnoId
            };
        }

        protected internal virtual async Task ValidateProperties(AsignaturaExpediente asignaturaExpediente,
            CreateAsignaturaExpedienteCommand request, CancellationToken cancellationToken)
        {
            if (!await _context.SituacionesAsignaturas
                    .AnyAsync(sa => sa.Id == request.SituacionAsignaturaId, cancellationToken))
                throw new NotFoundException(nameof(SituacionAsignatura), request.SituacionAsignaturaId);

            if (!await _context.ExpedientesAlumno
                    .AnyAsync(ea => ea.Id == request.ExpedienteAlumnoId, cancellationToken))
                throw new NotFoundException(nameof(ExpedienteAlumno), request.ExpedienteAlumnoId);

            var errors = asignaturaExpediente.VerificarPropiedadesRequeridosParaCrear();

            if (errors.Any())
                throw new ValidationErrorsException(errors.ToArray());
        }
    }
}
