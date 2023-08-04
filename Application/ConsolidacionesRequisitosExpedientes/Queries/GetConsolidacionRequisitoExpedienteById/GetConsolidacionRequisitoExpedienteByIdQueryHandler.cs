using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ConsolidacionesRequisitosExpedientes.Queries.GetConsolidacionRequisitoExpedienteById
{
    public class GetConsolidacionRequisitoExpedienteByIdQueryHandler : 
        IRequestHandler<GetConsolidacionRequisitoExpedienteByIdQuery, ConsolidacionRequisitoExpedienteDto>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetConsolidacionRequisitoExpedienteByIdQueryHandler(
            IExpedientesContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ConsolidacionRequisitoExpedienteDto> Handle(GetConsolidacionRequisitoExpedienteByIdQuery request,
            CancellationToken cancellationToken)
        {
            var consolidacionRequisito = await _context.ConsolidacionesRequisitosExpedientes
                .Include(cre => cre.EstadoRequisitoExpediente)
                .Include(cre => cre.TipoRequisitoExpediente)
                .Include(cre => cre.RequisitoExpediente)
                .ThenInclude(re => re.RequisitosExpedientesDocumentos)
                .Include(cre => cre.RequisitoExpediente)
                .ThenInclude(cre => cre.RolesRequisitosExpedientes)
                .FirstOrDefaultAsync(cre => cre.Id == request.Id);
            if (consolidacionRequisito == null)
                throw new NotFoundException(nameof(ConsolidacionRequisitoExpediente), request.Id);

            return _mapper.Map<ConsolidacionRequisitoExpediente, ConsolidacionRequisitoExpedienteDto>(consolidacionRequisito);
        }
    }
}
