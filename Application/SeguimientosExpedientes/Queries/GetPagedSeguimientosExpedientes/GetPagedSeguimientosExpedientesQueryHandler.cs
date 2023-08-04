using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Security;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientes;

public class GetPagedSeguimientosExpedientesQueryHandler : IRequestHandler<GetPagedSeguimientosExpedientesQuery, 
    ResultListDto<SeguimientoExpedienteListItemDto>>
{
    private readonly IExpedientesContext _context;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler> _localizer;
    private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

    public GetPagedSeguimientosExpedientesQueryHandler(IExpedientesContext context, IMapper mapper,
        IStringLocalizer<GetPagedSeguimientosExpedientesQueryHandler> localizer,
        IErpAcademicoServiceClient erpAcademicoServiceClient)
    {
        _context = context;
        _mapper = mapper;
        _localizer = localizer;
        _erpAcademicoServiceClient = erpAcademicoServiceClient;
    }

    public async Task<ResultListDto<SeguimientoExpedienteListItemDto>> Handle(GetPagedSeguimientosExpedientesQuery request, CancellationToken cancellationToken)
    {
        if (!request.HasPaginationInfo)
            throw new BadRequestException( _localizer["Los campos offset y limit son obligatorios"] );

        var queryable = _context.SeguimientosExpediente
            .Include(s => s.TipoSeguimiento)
            .Include(se => se.ExpedienteAlumno)
            .AsQueryable();

        var query = ApplyQuery(queryable, request);

        var seguimientosExpedientes = await query
            .OrderByDescending(sa => sa.Fecha)
            .Paginate(request)
            .ProjectTo<SeguimientoExpedienteListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);
        var total = await query.CountAsync(cancellationToken);

        return new ResultListDto<SeguimientoExpedienteListItemDto>
        {
            Elements = await ProyectarIntegracionDatosErp(seguimientosExpedientes),
            TotalElements = total
        };
    }

    protected internal virtual IQueryable<SeguimientoExpediente> ApplyQuery(IQueryable<SeguimientoExpediente> query,
        GetPagedSeguimientosExpedientesQuery request)
    {
        if (request.FilterIdExpedienteAlumno.HasValue)
        {
            query = query.Where(e => e.ExpedienteAlumnoId == request.FilterIdExpedienteAlumno);
        }

        if (request.FilterIdRefUniversidad.HasValue)
        {
            query = query.Where(e => e.ExpedienteAlumno.IdRefUniversidad == request.FilterIdRefUniversidad.ToString());
        }

        if (request.FiltersIdsRefPlan != null && request.FiltersIdsRefPlan.Any())
        {
            query = query.Where(e => request.FiltersIdsRefPlan.Contains(e.ExpedienteAlumno.IdRefPlan));
        }

        if (request.FilterIdRefPlan.HasValue)
        {
            query = query.Where(e => e.ExpedienteAlumno.IdRefPlan == request.FilterIdRefPlan.ToString());
        }

        if (request.FilterIdTipoSeguimiento.HasValue)
        {
            query = query.Where(e => e.TipoSeguimientoId == request.FilterIdTipoSeguimiento);
        }

        if (request.FilterFechaDesde.HasValue && request.FilterFechaHasta.HasValue)
        {
            query = query.Where(e =>
                e.Fecha >= request.FilterFechaDesde && e.Fecha < request.FilterFechaHasta.Value.Date.AddDays(1));
        }

        if (request.FilterFechaDesde.HasValue)
        {
            query = query.Where(e => e.Fecha >= request.FilterFechaDesde);
        }

        if (request.FilterFechaHasta.HasValue)
        {
            query = query.Where(e => e.Fecha < request.FilterFechaHasta.Value.Date.AddDays(1));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterNombreAlumno))
        {
            query = query.Where(c => c.ExpedienteAlumno.AlumnoNombre.StartsWith(request.FilterNombreAlumno));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterPrimerApellido))
        {
            query = query.Where(c => c.ExpedienteAlumno.AlumnoApellido1.StartsWith(request.FilterPrimerApellido));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterSegundoApellido))
        {
            query = query.Where(c => c.ExpedienteAlumno.AlumnoApellido2.StartsWith(request.FilterSegundoApellido));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterNroDocIdentificacion))
        {
            query = query.Where(c => c.ExpedienteAlumno.AlumnoNroDocIdentificacion.StartsWith(request.FilterNroDocIdentificacion));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterIdRefIntegracionAlumno))
        {
            query = query.Where(e => e.ExpedienteAlumno.IdRefIntegracionAlumno.Equals(request.FilterIdRefIntegracionAlumno.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterIdCuentaSeguridad))
        {
            query = query.Where(e => e.IdCuentaSeguridad == request.FilterIdCuentaSeguridad);
        }

        return query;
    }

    protected internal virtual async Task<List<SeguimientoExpedienteListItemDto>> ProyectarIntegracionDatosErp(
        List<SeguimientoExpedienteListItemDto> seguimientosExpedientesDtos)
    {
        if (!seguimientosExpedientesDtos.Any()) return seguimientosExpedientesDtos;

        var usuariosInternos = await GetUsuariosInternos(seguimientosExpedientesDtos);
        if (!usuariosInternos.Any()) return seguimientosExpedientesDtos;

        seguimientosExpedientesDtos.ForEach(se =>
        {
            SetNombreTrabajador(se, usuariosInternos);
        });
        return seguimientosExpedientesDtos;
    }

    protected internal virtual void SetNombreTrabajador(
        SeguimientoExpedienteListItemDto seguimiento, List<UsuarioModel> usuariosInternos)
    {
        if (string.IsNullOrEmpty(seguimiento.IdCuentaSeguridad)) return;

        const string errorUsuarioNoRelacionado = "Usuario '{0}' (No coincide)";
        var usuarioInternoErp =
                usuariosInternos?.FirstOrDefault(p => p.IdCuentaSeguridad == seguimiento.IdCuentaSeguridad);
        seguimiento.NombreTrabajador = usuarioInternoErp != null
            ? usuarioInternoErp.UserName
            : string.Format(errorUsuarioNoRelacionado, seguimiento.IdCuentaSeguridad);
    }

    public virtual async Task<List<UsuarioModel>> GetUsuariosInternos(
        List<SeguimientoExpedienteListItemDto> seguimientosExpedientes)
    {
        var result = new List<UsuarioModel>();
        var idsUsuariosInternos = seguimientosExpedientes.Where(x => !string.IsNullOrEmpty(x.IdCuentaSeguridad))
            .Select(x => x.IdCuentaSeguridad).Distinct().ToList();
        if (!idsUsuariosInternos.Any())
            return result;

        foreach (var idUsuario in idsUsuariosInternos)
        {
            var resultUsuarioInterno = await _erpAcademicoServiceClient.GetUserNameByIdSeguridad(idUsuario);
            if (resultUsuarioInterno == null) continue;

            result.Add(resultUsuarioInterno);
        }

        return result;
    }
}