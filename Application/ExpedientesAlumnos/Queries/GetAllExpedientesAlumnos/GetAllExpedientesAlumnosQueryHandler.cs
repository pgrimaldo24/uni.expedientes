using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllExpedientesAlumnos
{
    public class GetAllExpedientesAlumnosQueryHandler : IRequestHandler<GetAllExpedientesAlumnosQuery, ExpedienteAlumnoListItemDto[]>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetAllExpedientesAlumnosQueryHandler(IExpedientesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ExpedienteAlumnoListItemDto[]> Handle(GetAllExpedientesAlumnosQuery request, CancellationToken cancellationToken)
        {
            var queryable = _context.ExpedientesAlumno
                .Include(a => a.AsignaturasExpedientes)
                .ThenInclude(a => a.SituacionAsignatura)
                .Include(s => s.Seguimientos)
                .ThenInclude(s => s.TipoSeguimiento)
                .AsQueryable();

            var query = ApplyQuery(queryable, request);

            var expedientesAlumnos = await query
                .OrderBy(e => e.IdRefIntegracionAlumno)
                .ProjectTo<ExpedienteAlumnoListItemDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);

            return expedientesAlumnos;
        }

        protected internal virtual IQueryable<ExpedienteAlumno> ApplyQuery(IQueryable<ExpedienteAlumno> query,
            GetAllExpedientesAlumnosQuery request)
        {
            if (request.FilterIdExpedienteAlumno.HasValue)
            {
                query = query.Where(e => e.Id == request.FilterIdExpedienteAlumno.Value);
            }

            if (request.FiltersIdsExpedientesAlumnos != null && request.FiltersIdsExpedientesAlumnos.Any())
            {
                query = query.Where(e => request.FiltersIdsExpedientesAlumnos.Contains(e.Id));
            }

            if (request.FilterIdsRefIntegracionAlumnos != null && request.FilterIdsRefIntegracionAlumnos.Any())
            {
                query = query.Where(e => request.FilterIdsRefIntegracionAlumnos.Contains(e.IdRefIntegracionAlumno));
            }

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
