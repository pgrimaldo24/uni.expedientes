using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesMigrarCalificaciones
{
    public class GetExpedientesMigrarCalificacionesQueryHandler : IRequestHandler<GetExpedientesMigrarCalificacionesQuery, List<ExpedienteAlumno>>
    {
        private readonly IExpedientesContext _context;
        public GetExpedientesMigrarCalificacionesQueryHandler(IExpedientesContext context)
        {
            _context = context;
        }
        public async Task<List<ExpedienteAlumno>> Handle(GetExpedientesMigrarCalificacionesQuery request, CancellationToken cancellationToken)
        {
            var queryable = _context.ExpedientesAlumno
                .Include(p => p.ExpedientesEspecializaciones)
                .Include(hc => hc.HitosConseguidos)
                .Include(ae => ae.AsignaturasExpedientes)
                .ThenInclude(ac => ac.AsignaturasCalificaciones)
                .AsQueryable();
            var query = ApplyQuery(queryable, request);
            var expedientesAlumnos = await query.ToListAsync(cancellationToken);
            return expedientesAlumnos;
        }

        protected internal virtual IQueryable<ExpedienteAlumno> ApplyQuery(IQueryable<ExpedienteAlumno> queryable,
        GetExpedientesMigrarCalificacionesQuery request)
        {
            queryable = queryable.Where(e => !e.Migrado);
            if (!string.IsNullOrWhiteSpace(request.IdRefUniversidad))
            {
                queryable = queryable.Where(e => e.IdRefUniversidad == request.IdRefUniversidad);
            }

            if (!string.IsNullOrWhiteSpace(request.IdRefEstudio))
            {
                queryable = queryable.Where(e => e.IdRefEstudio == request.IdRefEstudio);
            }
            return queryable;
        }
    }
}
