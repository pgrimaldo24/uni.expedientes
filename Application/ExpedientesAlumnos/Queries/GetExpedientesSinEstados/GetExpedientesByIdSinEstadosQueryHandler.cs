using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesSinEstados
{
    public class GetExpedientesSinEstadoQueryHandler : IRequestHandler<GetExpedientesSinEstadosQuery, List<ExpedienteAlumno>>
    {
        private readonly IExpedientesContext _context;

        public GetExpedientesSinEstadoQueryHandler(IExpedientesContext context)
        {
            _context = context;
        }

        public async Task<List<ExpedienteAlumno>> Handle(GetExpedientesSinEstadosQuery request, CancellationToken cancellationToken)
        {

            var queryable = _context.ExpedientesAlumno
                .Include(s => s.ExpedientesEspecializaciones)
                .AsQueryable();
            var query = ApplyQuery(queryable, request);
            var expedientesAlumnos = await query
                    .ToListAsync(cancellationToken);
            return expedientesAlumnos;
        }

        protected internal virtual IQueryable<ExpedienteAlumno> ApplyQuery(IQueryable<ExpedienteAlumno> query,
            GetExpedientesSinEstadosQuery request)
        {
            query = query.Where(e => e.Estado == null);

            if (request.IdExpedienteAlumno.HasValue)
            {
                query = query.Where(e => e.Id == request.IdExpedienteAlumno);
                return query;
            }

            if (!string.IsNullOrWhiteSpace(request.IdRefUniversidad))
            {
                query = query.Where(e => e.IdRefUniversidad == request.IdRefUniversidad);
            }

            if (!string.IsNullOrWhiteSpace(request.IdRefEstudio))
            {
                query = query.Where(e => e.IdRefEstudio == request.IdRefEstudio);
            }

            if (request.FechaAperturaDesde.HasValue)
            {
                query = query.Where(e => e.FechaApertura.Value.Date >= request.FechaAperturaDesde.Value.Date);
            }

            if (request.FechaAperturaHasta.HasValue)
            {
                query = query.Where(e => e.FechaApertura.Value.Date <= request.FechaAperturaHasta.Value.Date);
            }

            return query;
        }
    }
}
