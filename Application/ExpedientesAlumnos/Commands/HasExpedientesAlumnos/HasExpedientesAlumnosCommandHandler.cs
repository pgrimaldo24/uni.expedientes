using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Commands.HasExpedientesAlumnos
{
    public class HasExpedientesAlumnosCommandHandler : IRequestHandler<HasExpedientesAlumnosCommand, bool>
    {
        private readonly IExpedientesContext _context;

        public HasExpedientesAlumnosCommandHandler(IExpedientesContext context)
        {
            _context = context;
        }

        public Task<bool> Handle(HasExpedientesAlumnosCommand request, CancellationToken cancellationToken)
        {
            var queryable = _context.ExpedientesAlumno
                .Include(s => s.Seguimientos)
                .AsQueryable();
            var query = ApplyQuery(queryable, request);
            return query.AnyAsync(cancellationToken);
        }

        protected internal virtual IQueryable<ExpedienteAlumno> ApplyQuery(IQueryable<ExpedienteAlumno> query,
            HasExpedientesAlumnosCommand request)
        {
            if (request.FilterIdPlan.HasValue)
            {
                query = query.Where(e => e.IdRefPlan == request.FilterIdPlan.Value.ToString());
            }

            if (!string.IsNullOrWhiteSpace(request.FilterIdRefIntegracionAlumno))
            {
                query = query.Where(e => e.IdRefIntegracionAlumno == request.FilterIdRefIntegracionAlumno);
            }

            if (request.FiltersIdsRefVersionPlan != null && request.FiltersIdsRefVersionPlan.Any())
            {
                query = query.Where(e => request.FiltersIdsRefVersionPlan.Contains(e.IdRefVersionPlan));
            }

            if (request.FiltersIdsRefPlan != null && request.FiltersIdsRefPlan.Any())
            {
                query = query.Where(e => request.FiltersIdsRefPlan.Contains(e.IdRefPlan));
            }

            if (request.FilterIdSeguimientos.HasValue)
            {
                query = query.Where(e => e.Seguimientos.Any(se => se.Id == request.FilterIdSeguimientos.Value));
            }

            if (!string.IsNullOrWhiteSpace(request.FilterIdRefNodo))
            {
                query = query.Where(e => e.IdRefNodo == request.FilterIdRefNodo);
            }

            return query;
        }
    }
}
