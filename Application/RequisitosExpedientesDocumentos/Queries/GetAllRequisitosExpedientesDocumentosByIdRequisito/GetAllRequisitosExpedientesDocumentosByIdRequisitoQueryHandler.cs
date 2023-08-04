using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.RequisitosExpedientesDocumentos.Queries.GetAllRequisitosExpedientesDocumentosByIdRequisito
{
    public class GetAllRequisitosExpedientesDocumentosByIdRequisitoQueryHandler : IRequestHandler<GetAllRequisitosExpedientesDocumentosByIdRequisitoQuery, List<RequisitoExpedienteDocumentoDto>>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetAllRequisitosExpedientesDocumentosByIdRequisitoQueryHandler(IExpedientesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RequisitoExpedienteDocumentoDto>> Handle(GetAllRequisitosExpedientesDocumentosByIdRequisitoQuery request, CancellationToken cancellationToken)
        {
            var tiposSolicitudesTilulosResponsablesFirma = await _context.RequisitosExpedientesDocumentos
                .Where(red => red.RequisitoExpediente.Id == request.IdRequisitoExpediente)
                .ToListAsync(cancellationToken);

            var result = _mapper.Map<List<RequisitoExpedienteDocumento>, List<RequisitoExpedienteDocumentoDto>>(tiposSolicitudesTilulosResponsablesFirma);
            return result;
        }
    }
}
