using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetExpedientesAlumnosARelacionar
{
    public class GetExpedientesAlumnosARelacionarQueryHandler : IRequestHandler<GetExpedientesAlumnosARelacionarQuery, List<ExpedienteAlumno>>
    {
        private readonly IExpedientesContext _context;

        public GetExpedientesAlumnosARelacionarQueryHandler(IExpedientesContext context)
        {
            _context = context;
        }

        public async Task<List<ExpedienteAlumno>> Handle(GetExpedientesAlumnosARelacionarQuery request, CancellationToken cancellationToken)
        {
            var queryable = _context.ExpedientesAlumno.AsQueryable();
            var query = ApplyQuery(queryable, request);
            var expedientesAlumnos = await query.ToListAsync(cancellationToken);
            return expedientesAlumnos;
        }

        protected internal virtual IQueryable<ExpedienteAlumno> ApplyQuery(IQueryable<ExpedienteAlumno> queryable,
        GetExpedientesAlumnosARelacionarQuery request)
        {
            queryable = queryable.Where(e => e.RelacionesExpedientesOrigen == null || 
                                             e.RelacionesExpedientesOrigen.All(re => re.TipoRelacion.Id != TipoRelacionExpediente.CambioPlan));
            if (request.IdExpedienteAlumno.HasValue)
            {
                queryable = queryable.Where(e => e.Id == request.IdExpedienteAlumno);
                return queryable;
            }

            if (!string.IsNullOrWhiteSpace(request.IdRefUniversidad))
            {
                queryable = queryable.Where(e => e.IdRefUniversidad == request.IdRefUniversidad);
            }

            if (!string.IsNullOrWhiteSpace(request.IdRefEstudio))
            {
                queryable = queryable.Where(e => e.IdRefEstudio == request.IdRefEstudio);
            }

            if (request.FechaAperturaDesde.HasValue)
            {
                queryable = queryable.Where(s => s.FechaApertura >= request.FechaAperturaDesde.Value);
            }
            
            if (request.FechaAperturaHasta.HasValue)
            {
                queryable = queryable.Where(s => s.FechaApertura <= request.FechaAperturaHasta.Value);
            }

            return queryable;
        }
    }
}
