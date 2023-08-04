using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetConfiguracionExpedienteUniversidadByIdExpediente
{
    public class GetConfiguracionExpedienteUniversidadQueryHandler : IRequestHandler<GetConfiguracionExpedienteUniversidadByIdExpedienteQuery, ConfiguracionExpedienteUniversidadDto>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;

        public GetConfiguracionExpedienteUniversidadQueryHandler(
            IExpedientesContext context, 
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ConfiguracionExpedienteUniversidadDto> Handle(GetConfiguracionExpedienteUniversidadByIdExpedienteQuery request, 
            CancellationToken cancellationToken)
        {
            var expedienteAlumno = await _context.ExpedientesAlumno
                .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
            if (expedienteAlumno == null)
                throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

            var configuracionUniversidad = await _context.ConfiguracionesExpedientesUniversidades
                .FirstOrDefaultAsync(ceu => ceu.IdRefUniversidad == expedienteAlumno.IdRefUniversidad);
            if (configuracionUniversidad == null)
                throw new NotFoundException(nameof(ConfiguracionExpedienteUniversidad), expedienteAlumno.IdRefUniversidad);

            var configuracionUniversidadDto = _mapper.Map<ConfiguracionExpedienteUniversidad, 
                ConfiguracionExpedienteUniversidadDto>(configuracionUniversidad);
            return configuracionUniversidadDto;
        }
    }
}
