using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.TiposSituacionEstadoExpedientes.Queries.GetUltimoTipoSituacionEstadoByIdExpediente
{
    public class GetUltimoTipoSituacionEstadoByIdExpedienteQueryHandler :
        IRequestHandler<GetUltimoTipoSituacionEstadoByIdExpedienteQuery, TipoSituacionEstadoExpedienteDto>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetUltimoTipoSituacionEstadoByIdExpedienteQueryHandler(
            IExpedientesContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<TipoSituacionEstadoExpedienteDto> Handle(GetUltimoTipoSituacionEstadoByIdExpedienteQuery request, 
            CancellationToken cancellationToken)
        {
            var tiposSituacionsEstadosExpedientes = await _context.TiposSituacionEstadoExpedientes
                .Where(tsee => tsee.ExpedienteAlumnoId == request.IdExpedienteAlumno)
                .ToListAsync(cancellationToken);
            if (!tiposSituacionsEstadosExpedientes.Any())
                return null;

            var tiposSituacionsEstadosExpedientesDto = _mapper.Map<List<TipoSituacionEstadoExpediente>, 
                List<TipoSituacionEstadoExpedienteDto>>(tiposSituacionsEstadosExpedientes);
            var result = tiposSituacionsEstadosExpedientesDto.OrderByDescending(tsee => tsee.Id).First();
            return result;
        }
    }
}
