using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.HitosConseguidos.Queries.GetHitosConseguidosByIdExpediente
{
    public class GetHitosConseguidosByIdExpedienteQueryHandler :
        IRequestHandler<GetHitosConseguidosByIdExpedienteQuery, List<HitoConseguidoDto>>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetHitosConseguidosByIdExpedienteQueryHandler(
            IExpedientesContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<HitoConseguidoDto>> Handle(
            GetHitosConseguidosByIdExpedienteQuery request, CancellationToken cancellationToken)
        {
            var hitosConseguidos = await _context.HitosConseguidos
                .Include(hc => hc.TipoConseguido)
                .Where(hc => hc.ExpedienteAlumnoId == request.IdExpedienteAlumno)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<HitoConseguido>, List<HitoConseguidoDto>>(hitosConseguidos);
        }
    }
}
