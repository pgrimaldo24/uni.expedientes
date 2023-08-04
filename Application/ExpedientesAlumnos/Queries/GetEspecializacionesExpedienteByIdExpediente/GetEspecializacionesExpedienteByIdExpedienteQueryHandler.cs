using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetEspecializacionesExpedienteByIdExpediente
{
    public class GetEspecializacionesExpedienteByIdExpedienteQueryHandler : IRequestHandler<
        GetEspecializacionesExpedienteByIdExpedienteQuery, List<ExpedienteEspecializacionDto>>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetEspecializacionesExpedienteByIdExpedienteQueryHandler(
            IExpedientesContext context, 
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ExpedienteEspecializacionDto>> Handle(
            GetEspecializacionesExpedienteByIdExpedienteQuery request, CancellationToken cancellationToken)
        {
            var tieneHitoEspecializacion = await _context.HitosConseguidos
                .AnyAsync(hc => hc.ExpedienteAlumnoId == request.IdExpedienteAlumno
                            && hc.TipoConseguidoId == TipoHitoConseguido.Especializacion, 
                            cancellationToken);
            if (!tieneHitoEspecializacion)
                return new List<ExpedienteEspecializacionDto>();

            var especializaciones = await _context.ExpedientesEspecializaciones
                .Where(ee => ee.ExpedienteAlumnoId == request.IdExpedienteAlumno)
                .OrderByDescending(ee => ee.Id)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ExpedienteEspecializacion>,
                List<ExpedienteEspecializacionDto>>(especializaciones);
        }
    }
}
