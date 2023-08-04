using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetApplyQueryExpedientesAlumnos
{
    public class GetApplyQueryExpedientesAlumnosQueryHandler :
        IRequestHandler<GetApplyQueryExpedientesAlumnosQuery, IQueryable<ExpedienteAlumno>>
    {
        public async Task<IQueryable<ExpedienteAlumno>> Handle(GetApplyQueryExpedientesAlumnosQuery request, 
            CancellationToken cancellationToken)
        {
            var filter = request.Filter;
            var query = request.Queryable;

            if (filter.FilterIdExpedienteAlumno.HasValue)
            {
                query = query.Where(e => e.Id == filter.FilterIdExpedienteAlumno);
            }

            if (filter.FilterIdRefUniversidad.HasValue)
            {
                query = query.Where(e => e.IdRefUniversidad == filter.FilterIdRefUniversidad.ToString());
            }

            if (filter.FiltersIdsRefPlan != null && filter.FiltersIdsRefPlan.Any())
            {
                query = query.Where(e => filter.FiltersIdsRefPlan.Contains(e.IdRefPlan));
            }

            if (filter.FilterIdRefPlan.HasValue)
            {
                query = query.Where(e => e.IdRefPlan == filter.FilterIdRefPlan.ToString());
            }

            if (!string.IsNullOrWhiteSpace(filter.FilterNombreAlumno))
            {
                query = query.Where(c => c.AlumnoNombre.StartsWith(filter.FilterNombreAlumno));
            }

            if (!string.IsNullOrWhiteSpace(filter.FilterPrimerApellido))
            {
                query = query.Where(c => c.AlumnoApellido1.StartsWith(filter.FilterPrimerApellido));
            }

            if (!string.IsNullOrWhiteSpace(filter.FilterSegundoApellido))
            {
                query = query.Where(c => c.AlumnoApellido2.StartsWith(filter.FilterSegundoApellido));
            }

            if (!string.IsNullOrWhiteSpace(filter.FilterNroDocIdentificacion))
            {
                query = query.Where(c => c.AlumnoNroDocIdentificacion.StartsWith(filter.FilterNroDocIdentificacion));
            }

            if (!string.IsNullOrWhiteSpace(filter.FilterIdRefIntegracionAlumno))
            {
                query = query.Where(e => e.IdRefIntegracionAlumno.Equals(filter.FilterIdRefIntegracionAlumno));
            }
            return await Task.FromResult(query);
        }
    }
}
