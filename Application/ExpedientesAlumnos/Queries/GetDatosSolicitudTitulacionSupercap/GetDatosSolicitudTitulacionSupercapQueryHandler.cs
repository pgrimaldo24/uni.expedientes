using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Exceptions;

namespace Unir.Expedientes.Application.ExpedientesAlumnos.Queries.GetDatosSolicitudTitulacionSupercap;

public class GetDatosSolicitudTitulacionSupercapQueryHandler : IRequestHandler<GetDatosSolicitudTitulacionSupercapQuery, ExpedienteAlumnoSolicitudTitulacionSupercapDto>
{
    private readonly IExpedientesContext _context;
    private readonly IStringLocalizer<GetDatosSolicitudTitulacionSupercapQueryHandler> _localizer;
    private readonly IMapper _mapper;

    public GetDatosSolicitudTitulacionSupercapQueryHandler(IExpedientesContext context,
        IStringLocalizer<GetDatosSolicitudTitulacionSupercapQueryHandler> localizer, IMapper mapper)
    {
        _context = context;
        _localizer = localizer;
        _mapper = mapper;
    }

    public async Task<ExpedienteAlumnoSolicitudTitulacionSupercapDto> Handle(
        GetDatosSolicitudTitulacionSupercapQuery request, CancellationToken cancellationToken)
    {
        var expedienteAlumno = await _context
            .ExpedientesAlumno
            .Include(ea => ea.TitulacionAcceso)
            .FirstOrDefaultAsync(
                ea => ea.IdRefIntegracionAlumno == request.IdRefIntegracionAlumno && ea.IdRefPlan == request.IdRefPlan,
                cancellationToken);
        if (expedienteAlumno == null)
            throw new BadRequestException(_localizer[
                $"No se ha encontrado un Expediente con el {nameof(request.IdRefPlan)}: '{request.IdRefPlan}' y {nameof(request.IdRefIntegracionAlumno)}: '{request.IdRefIntegracionAlumno}'."]);

        return _mapper.Map<ExpedienteAlumno, ExpedienteAlumnoSolicitudTitulacionSupercapDto>(expedienteAlumno);
    }
}