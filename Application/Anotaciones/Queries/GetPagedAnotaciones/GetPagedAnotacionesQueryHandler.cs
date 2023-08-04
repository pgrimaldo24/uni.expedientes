using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Settings;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.ApplicationSuperTypes.Models.Results;
using Unir.Framework.Crosscutting.Security;
using Unir.Framework.Crosscutting.Security.Model;

namespace Unir.Expedientes.Application.Anotaciones.Queries.GetPagedAnotaciones
{
    public class
        GetPagedAnotacionesQueryHandler : IRequestHandler<GetPagedAnotacionesQuery, ResultListDto<AnotacionListItemDto>>
    {
        private static readonly string[] RolesCargosFuncionales =
        {
            AppConfiguration.KeyAdminRole, AppConfiguration.KeyGestorRole
        };
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        private readonly IIdentityService _identityService;
        private readonly ISecurityService _securityService;

        public GetPagedAnotacionesQueryHandler(IExpedientesContext context, 
            IMapper mapper, IIdentityService identityService, ISecurityService securityService)
        {
            _context = context;
            _mapper = mapper;
            _identityService = identityService;
            _securityService = securityService;
        }

        public async Task<ResultListDto<AnotacionListItemDto>> Handle(GetPagedAnotacionesQuery request, CancellationToken cancellationToken)
        {
            var resultListDto = new ResultListDto<AnotacionListItemDto>();
            var infoSecurity = _identityService.GetUserIdentityInfo();
            if (infoSecurity == null) return resultListDto;

            var account = await _securityService.GetAccountByIdAsync(infoSecurity.Id, null);
            if (account == null) return resultListDto;
            account.Roles = infoSecurity.Roles;

            var query = _context.Anotaciones
                .Include(a => a.ExpedienteAlumno)
                .Include(a => a.RolesAnotaciones)
                .Where(a => a.ExpedienteAlumno.Id == request.IdExpedienteAlumno)
                .AsQueryable();

            query = ApplyQuery(query, request, account);
            var total = await query.CountAsync(cancellationToken);

            var anotaciones = await query                
                .OrderByDescending(a => a.Fecha)
                .Paginate(request)
                .ProjectTo<AnotacionListItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            await GetInformacionCuentaSeguridad(anotaciones);
            resultListDto.Elements = anotaciones;
            resultListDto.TotalElements = total;

            return resultListDto;
        }

        protected internal virtual IQueryable<Anotacion> ApplyQuery(IQueryable<Anotacion> query,
            GetPagedAnotacionesQuery request, AccountModel accountInfo)
        {
            if (request.FechaDesde.HasValue)
            {
                query = query.Where(a => a.Fecha.Date >= request.FechaDesde);
            }

            if (request.FechaHasta.HasValue)
            {
                query = query.Where(a => a.Fecha.Date <= request.FechaHasta);
            }

            if (!string.IsNullOrWhiteSpace(request.Texto))
            {
                query = query.Where(a => a.Mensaje.Contains(request.Texto) || a.Resumen.Contains(request.Texto));
            }

            var roles = accountInfo.Roles.Where(r => RolesCargosFuncionales.Contains(r)).ToList();
            query = query.Where(a =>
                a.EsPublica || a.IdRefCuentaSeguridad == accountInfo.Id || roles.Any(rol => rol == AppConfiguration.KeyAdminRole) ||
                (a.RolesAnotaciones != null && a.RolesAnotaciones.Any(rol => roles.Any(r => r == rol.Rol))));

            return query;
        }

        protected internal virtual async Task GetInformacionCuentaSeguridad(List<AnotacionListItemDto> results)
        {
            foreach (var result in results)
            {
                if (string.IsNullOrEmpty(result.IdRefCuentaSeguridad)) continue;
                var account = await _securityService.GetAccountByIdAsync(result.IdRefCuentaSeguridad, null);
                if (account == null) continue;
                result.NombreUsuario = $"{account.FirstName} {account.Surname}";
            }
        }
    }
}
