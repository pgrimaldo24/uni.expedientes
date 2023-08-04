using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.TiposSeguimientosExpedientes.Queries.GetTiposSeguimientosExpedientes
{
    public class GetTiposSeguimientosExpedientesQueryHandler : IRequestHandler<GetTiposSeguimientosExpedientesQuery, TipoSeguimientoExpedienteListItemDto[]>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetTiposSeguimientosExpedientesQueryHandler(IExpedientesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TipoSeguimientoExpedienteListItemDto[]> Handle(GetTiposSeguimientosExpedientesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.TipoSeguimientosExpediente.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.Nombre))
            {
                query = query.Where(ts => ts.Nombre.Contains(request.Nombre));
            }

            if (request.HasPaginationInfo)
            {
                query = query.Paginate(request);
            }

            var tiposSeguimientos = await query
                .OrderBy(mc => mc.Nombre)
                .ProjectTo<TipoSeguimientoExpedienteListItemDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);

            return tiposSeguimientos;
        }
    }
}
