using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetAllEstadosExpedientes
{
    public class GetAllEstadosExpedientesQueryHandler : IRequestHandler<GetAllEstadosExpedientesQuery, EstadoExpedienteListItemDto[]>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetAllEstadosExpedientesQueryHandler(IExpedientesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<EstadoExpedienteListItemDto[]> Handle(GetAllEstadosExpedientesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.EstadosExpedientes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.FilterNombre))
            {
                query = query.Where(ee => ee.Nombre.Contains(request.FilterNombre));
            }
            if (request.HasPaginationInfo)
            {
                query = query.Paginate(request);
            }

            var estadosExpedientes = await query
                .OrderBy(mc => mc.Nombre)
                .ProjectTo<EstadoExpedienteListItemDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
            return estadosExpedientes;
        }
    }
}
