using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetTitulacionAccesoAndEspecializacionesByIdExpediente;

public class GetTitulacionAccesoAndEspecializacionesByIdExpedienteQueryHandler : IRequestHandler<
    GetTitulacionAccesoAndEspecializacionesByIdExpedienteQuery, ExpedienteTitulacionAccesoEspecializacionesItemDto>
{
    private readonly IExpedientesContext _context;
    private readonly IMapper _mapper;

    public GetTitulacionAccesoAndEspecializacionesByIdExpedienteQueryHandler(IExpedientesContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ExpedienteTitulacionAccesoEspecializacionesItemDto> Handle(
        GetTitulacionAccesoAndEspecializacionesByIdExpedienteQuery request, CancellationToken cancellationToken)
    {
        var expedienteAlumno = await _context.ExpedientesAlumno
            .Include(ea => ea.TitulacionAcceso)
            .Include(ea => ea.ExpedientesEspecializaciones)
            .FirstOrDefaultAsync(ea => ea.Id == request.IdExpedienteAlumno, cancellationToken);
        if (expedienteAlumno == null)
            throw new NotFoundException(nameof(ExpedienteAlumno), request.IdExpedienteAlumno);

        return _mapper.Map<ExpedienteAlumno, ExpedienteTitulacionAccesoEspecializacionesItemDto>(expedienteAlumno);
    }
}