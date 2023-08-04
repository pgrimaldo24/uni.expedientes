using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetCausasEstadosRequisitosConsolidadasByIdRequisito
{
    public class GetCausasEstadosRequisitosConsolidadasByIdRequisitoQueryHandler :
        IRequestHandler<GetCausasEstadosRequisitosConsolidadasByIdRequisitoQuery, CausaEstadoRequisitoConsolidadaExpedienteListItemDto[]>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetCausasEstadosRequisitosConsolidadasByIdRequisitoQueryHandler(
            IExpedientesContext context, 
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<CausaEstadoRequisitoConsolidadaExpedienteListItemDto[]> Handle(GetCausasEstadosRequisitosConsolidadasByIdRequisitoQuery request, 
            CancellationToken cancellationToken)
        {
            var query = _context.CausasEstadosRequisitosConsolidadasExpedientes
                .Where(cerc => cerc.RequisitoExpediente.Id == request.IdRequisito 
                            && cerc.EstadoRequisitoExpediente.Id == request.IdEstadoConsolidacion)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.FilterNombre))
            {
                query = query.Where(ee => ee.Nombre.Contains(request.FilterNombre));
            }
            if (request.HasPaginationInfo)
            {
                query = query.Paginate(request);
            }

            var causas = await query
                .OrderBy(mc => mc.Nombre)
                .ProjectTo<CausaEstadoRequisitoConsolidadaExpedienteListItemDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
            return causas;
        }
    }
}
