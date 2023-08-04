using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetTiposRelacionesExpedientes
{
    public class GetTiposRelacionesExpedientesQueryHandler : IRequestHandler<GetTiposRelacionesExpedientesQuery, TipoRelacionExpedienteListItemDto[]>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetTiposRelacionesExpedientesQueryHandler(IExpedientesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TipoRelacionExpedienteListItemDto[]> Handle(GetTiposRelacionesExpedientesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.TiposRelacionesExpediente
                .Where(tre => tre.EsLogro)
                .AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.FilterNombre))
            {
                query = query.Where(ee => ee.Nombre.Contains(request.FilterNombre));
            }
            if (request.HasPaginationInfo)
            {
                query = query.Paginate(request);
            }

            var tiposRelaciones = await query
                .OrderBy(mc => mc.Nombre)
                .ProjectTo<TipoRelacionExpedienteListItemDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
            return tiposRelaciones;
        }
    }
}
