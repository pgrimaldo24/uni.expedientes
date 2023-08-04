using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionRequisitoExpedienteById;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionesRequisitosByIdExpediente
{
    public class GetConsolidacionesRequisitosByIdExpedienteQueryHandler : IRequestHandler<
        GetConsolidacionesRequisitosByIdExpedienteQuery, List<ConsolidacionRequisitoExpedienteDto>>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        public GetConsolidacionesRequisitosByIdExpedienteQueryHandler(
            IExpedientesContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ConsolidacionRequisitoExpedienteDto>> Handle(
            GetConsolidacionesRequisitosByIdExpedienteQuery request, CancellationToken cancellationToken)
        {
            var consolidacionesRequisitos = await _context.ConsolidacionesRequisitosExpedientes
                .Include(cre => cre.EstadoRequisitoExpediente)
                .Include(cre => cre.TipoRequisitoExpediente)
                .Include(cre => cre.RequisitoExpediente)
                .ThenInclude(re => re.RequisitosExpedientesDocumentos)
                .Include(cre => cre.RequisitoExpediente)
                .ThenInclude(cre => cre.RolesRequisitosExpedientes)
                .Where(cre => cre.ExpedienteAlumnoId == request.IdExpedienteAlumno)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ConsolidacionRequisitoExpediente>, 
                List<ConsolidacionRequisitoExpedienteDto>>(consolidacionesRequisitos);
        }
    }
}
