using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetPagedRequisitosExpedientes;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Models.RequestParameters;

namespace Unir.Expedientes.Application.RequisitosExpedientes.Queries.GetRequisitosExpedientesBloqueados
{
    public class GetRequisitosExpedientesBloqueadosQueryHandler : 
        IRequestHandler<GetRequisitosExpedientesBloqueadosQuery, RequisitosExpedientesListItemDto[]>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetRequisitosExpedientesBloqueadosQueryHandler(
            IExpedientesContext context, 
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<RequisitosExpedientesListItemDto[]> Handle(GetRequisitosExpedientesBloqueadosQuery request, 
            CancellationToken cancellationToken)
        {
            var query = _context.RequisitosComportamientosExpedientes
                .Include(rce => rce.RequisitoExpediente)
                .Include(rce => rce.ComportamientoExpediente)
                .ThenInclude(ce => ce.NivelesUsoComportamientosExpedientes)
                .Where(rce => rce.RequisitoExpediente.Bloqueado 
                            && rce.ComportamientoExpediente.Bloqueado
                            && rce.ComportamientoExpediente.NivelesUsoComportamientosExpedientes
                                .Any(nuce => nuce.IdRefUniversidad == request.FilterIdRefUniversidad
                                        && nuce.TipoNivelUsoId == TipoNivelUso.Universidad))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.FilterNombre))
            {
                query = query.Where(re => re.RequisitoExpediente.Nombre.Contains(request.FilterNombre));
            }
            if (request.HasPaginationInfo)
            {
                query = query.Paginate(request);
            }

            var requisitos = await query
                .Select(rc => rc.RequisitoExpediente)
                .OrderBy(mc => mc.Nombre)
                .ProjectTo<RequisitosExpedientesListItemDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
            return requisitos;
        }
    }
}
