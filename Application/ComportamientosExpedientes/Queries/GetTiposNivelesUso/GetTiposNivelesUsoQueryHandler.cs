using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ComportamientosExpedientes.Queries.GetTiposNivelesUso
{
    public class GetTiposNivelesUsoQueryHandler : IRequestHandler<GetTiposNivelesUsoQuery, TipoNivelUsoListItemDto[]>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetTiposNivelesUsoQueryHandler(IExpedientesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TipoNivelUsoListItemDto[]> Handle(GetTiposNivelesUsoQuery request, CancellationToken cancellationToken)
        {
            var query = _context.TiposNivelesUso
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.FilterNombre))
            {
                query = query.Where(tnu => tnu.Nombre.Contains(request.FilterNombre));
            }
            if (request.HasPaginationInfo)
            {
                query = query.Paginate(request);
            }

            var tiposNivelesUso = await query
                .OrderBy(tnu => tnu.Orden)
                .ProjectTo<TipoNivelUsoListItemDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
            return tiposNivelesUso;
        }
    }
}
