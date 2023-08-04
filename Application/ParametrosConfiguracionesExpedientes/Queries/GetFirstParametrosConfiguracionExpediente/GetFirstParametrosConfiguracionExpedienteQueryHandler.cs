using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ParametrosConfiguracionesExpedientes.Queries.GetFirstParametrosConfiguracionExpediente
{
    public class GetFirstParametrosConfiguracionExpedienteQueryHandler : IRequestHandler<GetFirstParametrosConfiguracionExpedienteQuery, ParametroConfiguracionExpedienteFirstItemDto>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetFirstParametrosConfiguracionExpedienteQueryHandler(IExpedientesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ParametroConfiguracionExpedienteFirstItemDto> Handle(GetFirstParametrosConfiguracionExpedienteQuery request, CancellationToken cancellationToken)
        {
            var configuracionCondicion8 = await _context.ParametrosConfiguracionesExpedientes
                .Include(pc => pc.ParametrosConfiguracionesExpedientesFilesTypes)
                .FirstOrDefaultAsync(cancellationToken);
            if (configuracionCondicion8 != null)
                return _mapper.Map<ParametroConfiguracionExpedienteFirstItemDto>(configuracionCondicion8);

            const string keyNotFoundException = "Condicion8_ParametrosConfiguracionExpediente";
            throw new NotFoundException(nameof(ParametroConfiguracionExpediente), keyNotFoundException);
        }
    }
}
