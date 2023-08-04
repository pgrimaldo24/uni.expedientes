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

namespace Unir.Expedientes.Application.SeguimientosExpedientes.Queries.GetPagedSeguimientosExpedientesByIdExpediente
{
    public class GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler : IRequestHandler<GetPagedSeguimientosExpedientesByIdExpedienteQuery,
        ResultListDto<SeguimientoExpedienteByIdExpedienteListItemDto>>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler> _localizer;
        private readonly IErpAcademicoServiceClient _erpAcademicoServiceClient;

        public GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler(IExpedientesContext context, IMapper mapper,
            IStringLocalizer<GetPagedSeguimientosExpedientesByIdExpedienteQueryHandler> localizer,
            IErpAcademicoServiceClient erpAcademicoServiceClient)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
            _erpAcademicoServiceClient = erpAcademicoServiceClient;
        }

        public async Task<ResultListDto<SeguimientoExpedienteByIdExpedienteListItemDto>> Handle(GetPagedSeguimientosExpedientesByIdExpedienteQuery request, CancellationToken cancellationToken)
        {
            if (!request.HasPaginationInfo)
                throw new BadRequestException(_localizer["Los campos offset y limit son obligatorios"]);

            var queryable = _context.SeguimientosExpediente
                .Include(s => s.TipoSeguimiento)
                .Include(se => se.ExpedienteAlumno)
                .AsQueryable();
            var query = ApplyQuery(queryable, request);

            var seguimientosExpedientes = await query
                .OrderByDescending(sa => sa.Fecha)
                .Paginate(request)
                .ProjectTo<SeguimientoExpedienteByIdExpedienteListItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            var total = await query.CountAsync(cancellationToken);

            return new ResultListDto<SeguimientoExpedienteByIdExpedienteListItemDto>
            {
                Elements = await ProyectarIntegracionDatosErp(seguimientosExpedientes),
                TotalElements = total
            };
        }

        protected internal virtual IQueryable<SeguimientoExpediente> ApplyQuery(IQueryable<SeguimientoExpediente> query,
            GetPagedSeguimientosExpedientesByIdExpedienteQuery request)
        {
            if (request.FilterIdExpedienteAlumno.HasValue)
            {
                query = query.Where(e => e.ExpedienteAlumno.Id == request.FilterIdExpedienteAlumno);
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

            if (!string.IsNullOrWhiteSpace(request.FilterDescripcion))
            {
                query = query.Where(e => e.Descripcion.Contains(request.FilterDescripcion));
            }

            return query;
        }

        protected internal virtual async Task<List<SeguimientoExpedienteByIdExpedienteListItemDto>> ProyectarIntegracionDatosErp(
            List<SeguimientoExpedienteByIdExpedienteListItemDto> seguimientosExpedientesDtos)
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

        public virtual async Task<List<UsuarioModel>> GetUsuariosInternos(
            List<SeguimientoExpedienteByIdExpedienteListItemDto> seguimientosExpedientes)
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

        protected internal virtual void SetNombreTrabajador(
            SeguimientoExpedienteByIdExpedienteListItemDto seguimiento, List<UsuarioModel> usuariosInternos)
        {
            if (string.IsNullOrEmpty(seguimiento.IdCuentaSeguridad)) return;

            const string errorUsuarioNoRelacionado = "Usuario '{0}' (No coincide)";
            var usuarioInternoErp =
                    usuariosInternos?.FirstOrDefault(p => p.IdCuentaSeguridad == seguimiento.IdCuentaSeguridad);
            seguimiento.NombreTrabajador = usuarioInternoErp != null
                ? usuarioInternoErp.UserName
                : string.Format(errorUsuarioNoRelacionado, seguimiento.IdCuentaSeguridad);
        }
    }
}
