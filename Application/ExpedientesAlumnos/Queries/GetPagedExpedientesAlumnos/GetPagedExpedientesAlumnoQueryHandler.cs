using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.ApplicationSuperTypes.Models.Results;
using Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetApplyQueryExpedientesAlumnos;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetPagedExpedientesAlumnos;

public class GetPagedExpedientesAlumnoQueryHandler : IRequestHandler<GetPagedExpedientesAlumnoQuery, ResultListDto<ExpedienteAlumnoPagedListItemDto>>
{
    private readonly IExpedientesContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetPagedExpedientesAlumnoQueryHandler> _localizer;
    private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;
    private readonly IMediator _mediator;

    public GetPagedExpedientesAlumnoQueryHandler(IExpedientesContext context,
        IMapper mapper,
        IStringLocalizer<GetPagedExpedientesAlumnoQueryHandler> localizer,
        IErpAcademicoServiceClient erpAcademicoServiceClient,
        IMediator mediator)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        _erpAcademicoServiceClient = erpAcademicoServiceClient;
        _mediator = mediator;
    }

    public async Task<ResultListDto<ExpedienteAlumnoPagedListItemDto>> Handle(GetPagedExpedientesAlumnoQuery request,
        CancellationToken cancellationToken)
    {
        if (!request.HasPaginationInfo)
            throw new BadRequestException(_localizer["Los campos offset y limit son obligatorios"]);

        var queryable = _context.ExpedientesAlumno
            .Include(ea => ea.Seguimientos)
            .ThenInclude(s => s.TipoSeguimiento)
            .Include(ea => ea.Anotaciones)            
            .AsNoTracking()
            .AsQueryable();

        var query = await _mediator.Send(
            new GetApplyQueryExpedientesAlumnosQuery(queryable, request), cancellationToken);

        var expedientesAlumno = await query
            .OrderBy(c => c.IdRefIntegracionAlumno)
            .Paginate(request)
            .ProjectTo<ExpedienteAlumnoPagedListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var total = await query.CountAsync(cancellationToken);

        return new ResultListDto<ExpedienteAlumnoPagedListItemDto>
        {
            Elements = await ProyectarIntegracionExpedientesAlumnosErp(expedientesAlumno, cancellationToken),
            TotalElements = total
        };
    }

    protected internal virtual IQueryable<ExpedienteAlumno> ApplyQuery(IQueryable<ExpedienteAlumno> query,
        GetPagedExpedientesAlumnoQuery request)
    {
        if (request.FilterIdExpedienteAlumno.HasValue)
        {
            query = query.Where(e => e.Id == request.FilterIdExpedienteAlumno);
        }

        if (request.FilterIdRefUniversidad.HasValue)
        {
            query = query.Where(e => e.IdRefUniversidad == request.FilterIdRefUniversidad.ToString());
        }

        if (request.FiltersIdsRefPlan != null && request.FiltersIdsRefPlan.Any())
        {
            query = query.Where(e => request.FiltersIdsRefPlan.Contains(e.IdRefPlan));
        }

        if (request.FilterIdRefPlan.HasValue)
        {
            query = query.Where(e => e.IdRefPlan == request.FilterIdRefPlan.ToString());
        }

        if (!string.IsNullOrWhiteSpace(request.FilterNombreAlumno))
        {
            query = query.Where(c => c.AlumnoNombre.StartsWith(request.FilterNombreAlumno));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterPrimerApellido))
        {
            query = query.Where(c => c.AlumnoApellido1.StartsWith(request.FilterPrimerApellido));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterSegundoApellido))
        {
            query = query.Where(c => c.AlumnoApellido2.StartsWith(request.FilterSegundoApellido));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterNroDocIdentificacion))
        {
            query = query.Where(c => c.AlumnoNroDocIdentificacion.StartsWith(request.FilterNroDocIdentificacion));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterIdRefIntegracionAlumno))
        {
            query = query.Where(e => e.IdRefIntegracionAlumno.Equals(request.FilterIdRefIntegracionAlumno));
        }

        return query;
    }

    protected internal virtual async Task<List<ExpedienteAlumnoPagedListItemDto>> ProyectarIntegracionExpedientesAlumnosErp(
        List<ExpedienteAlumnoPagedListItemDto> expedientesAlumnosDtos, CancellationToken cancellationToken)
    {
        if (!expedientesAlumnosDtos.Any()) return expedientesAlumnosDtos;

        var idsIntegracion = expedientesAlumnosDtos.Select(e => e.Id.ToString()).ToArray();
        var expedientesErp = await _erpAcademicoServiceClient.GetExpedientesAsync(idsIntegracion, cancellationToken);
        if (expedientesErp == null || !expedientesErp.Any()) 
            return expedientesAlumnosDtos;

        expedientesAlumnosDtos.ForEach(e =>
        {
            var expedienteErp = expedientesErp.FirstOrDefault(expErp => expErp.IdIntegracion == e.Id.ToString());
            e.IdUniversidad = expedienteErp?.Plan?.Estudio.AreaAcademica.Centro.Universidad.Id;
            e.UniversidadDisplayName = expedienteErp?.Plan?.Estudio.AreaAcademica.Centro.Universidad.DisplayName;
            e.CentroEstudioDisplayName = expedienteErp?.Plan?.Estudio.AreaAcademica.Centro.DisplayName;
            e.TipoEstudioDisplayName = expedienteErp?.Plan?.Estudio.Tipo.DisplayName;
            e.TituloDisplayName = expedienteErp?.Plan?.Titulo.DisplayName;
        });
        return expedientesAlumnosDtos;
    }
}