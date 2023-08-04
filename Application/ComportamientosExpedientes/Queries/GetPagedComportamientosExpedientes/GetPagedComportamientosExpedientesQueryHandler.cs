using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;
using Unir.Framework.ApplicationSuperTypes.Models.Results;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetPagedComportamientosExpedientes
{
    public class GetPagedComportamientosExpedientesQueryHandler :
        IRequestHandler<GetPagedComportamientosExpedientesQuery, ResultListDto<ComportamientosExpedientesListItemDto>>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<GetPagedComportamientosExpedientesQueryHandler> _localizer;

        public GetPagedComportamientosExpedientesQueryHandler(
            IExpedientesContext context,
            IMapper mapper,
            IStringLocalizer<GetPagedComportamientosExpedientesQueryHandler> localizer)
        {
            _context = context;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task<ResultListDto<ComportamientosExpedientesListItemDto>> Handle(
            GetPagedComportamientosExpedientesQuery request, CancellationToken cancellationToken)
        {
            if (!request.HasPaginationInfo)
                throw new BadRequestException(_localizer["Los campos offset y limit son obligatorios"]);

            var resultListDto = new ResultListDto<ComportamientosExpedientesListItemDto>();
            var query = _context.ComportamientosExpedientes.AsQueryable();
            query = ApplyQuery(query, request);
            var total = await query.CountAsync(cancellationToken);

            var requisitos = await query
                .Paginate(request)
                .ProjectTo<ComportamientosExpedientesListItemDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            resultListDto.Elements = requisitos;
            resultListDto.TotalElements = total;
            return resultListDto;
        }

        protected internal virtual IQueryable<ComportamientoExpediente> ApplyQuery(IQueryable<ComportamientoExpediente> query,
            GetPagedComportamientosExpedientesQuery request)
        {
            query = query.Where(c => !c.Bloqueado);
            if (!string.IsNullOrWhiteSpace(request.Nombre))
            {
                query = query.Where(c => c.Nombre.Contains(request.Nombre));
            }

            if (request.EstaVigente.HasValue)
            {
                query = query.Where(c => c.EstaVigente == request.EstaVigente);
            }

            if (!string.IsNullOrWhiteSpace(request.NivelUso))
            {
                query = query.Where(c => c.NivelesUsoComportamientosExpedientes.Any(nuce =>
                    nuce.IdRefUniversidad.Contains(request.NivelUso) ||
                    nuce.AcronimoUniversidad.Contains(request.NivelUso) ||
                    nuce.IdRefTipoEstudio.Contains(request.NivelUso) ||
                    nuce.NombreTipoEstudio.Contains(request.NivelUso) ||
                    nuce.IdRefEstudio.Contains(request.NivelUso) ||
                    nuce.NombreEstudio.Contains(request.NivelUso) ||
                    nuce.IdRefPlan.Contains(request.NivelUso) ||
                    nuce.NombrePlan.Contains(request.NivelUso) ||
                    nuce.IdRefTipoAsignatura.Contains(request.NivelUso) ||
                    nuce.NombreTipoAsignatura.Contains(request.NivelUso) ||
                    nuce.IdRefAsignaturaPlan.Contains(request.NivelUso) ||
                    nuce.IdRefAsignatura.Contains(request.NivelUso) ||
                    nuce.NombreAsignatura.Contains(request.NivelUso)));
            }

            if (request.IdsCondiciones != null && request.IdsCondiciones.Any())
            {
                query = query.Where(r => r.RequisitosComportamientosExpedientes
                                    .Any(rce => request.IdsCondiciones.Contains(rce.RequisitoExpediente.Id)));
            }
            return query;
        }
    }
}
