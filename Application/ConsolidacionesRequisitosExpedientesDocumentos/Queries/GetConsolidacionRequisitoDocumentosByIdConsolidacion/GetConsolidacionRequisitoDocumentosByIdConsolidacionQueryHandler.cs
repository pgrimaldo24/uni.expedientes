using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientesDocumentos.Queries.GetConsolidacionRequisitoDocumentosByIdConsolidacion
{
    public class GetConsolidacionRequisitoDocumentosByIdConsolidacionQueryHandler :
        IRequestHandler<GetConsolidacionRequisitoDocumentosByIdConsolidacionQuery, ConsolidacionRequisitoExpedienteDocumentoListItemDto[]>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetConsolidacionRequisitoDocumentosByIdConsolidacionQueryHandler(
            IExpedientesContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ConsolidacionRequisitoExpedienteDocumentoListItemDto[]> Handle(GetConsolidacionRequisitoDocumentosByIdConsolidacionQuery request, 
            CancellationToken cancellationToken)
        {
            var query = _context.ConsolidacionesRequisitosExpedientesDocumentos
                       .Where(cred => cred.ConsolidacionRequisitoExpediente.Id == request.IdConsolidacionRequisitoExpediente)
                       .AsQueryable();

            var consolidacionesDocumentos = await query
                .OrderBy(cred => cred.Fichero)
                .ProjectTo<ConsolidacionRequisitoExpedienteDocumentoListItemDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
            return consolidacionesDocumentos;
        }
    }
}
