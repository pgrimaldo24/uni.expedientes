using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetTiposRequisitosExpedientes
{
    public class GetTiposRequisitosExpedientesQueryHandler : IRequestHandler<GetTiposRequisitosExpedientesQuery, TipoRequisitoExpedienteListItemDto[]>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetTiposRequisitosExpedientesQueryHandler(IExpedientesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TipoRequisitoExpedienteListItemDto[]> Handle(GetTiposRequisitosExpedientesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.TiposRequisitosExpedientes
                        .AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.FilterNombre))
            {
                query = query.Where(tre => tre.Nombre.Contains(request.FilterNombre));
            }
            if (request.HasPaginationInfo)
            {
                query = query.Paginate(request);
            }

            var tiposRequisitos = await query
                .OrderBy(tre => tre.Nombre)
                .ProjectTo<TipoRequisitoExpedienteListItemDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
            return tiposRequisitos;
        }
    }
}
