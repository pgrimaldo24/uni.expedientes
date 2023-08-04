using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.AsignaturasExpediente.Queries.GetAsignaturasExpedientesByIdExpediente
{
    public class GetAsignaturasExpedientesByIdExpedienteQueryHandler : 
        IRequestHandler<GetAsignaturasExpedientesByIdExpedienteQuery, List<AsignaturaExpedienteListItemDto>>
    {
        private readonly IExpedientesContext _context;
        private readonly IMapper _mapper;
        private const string Apto = "apto";
        private const string NoApto = "no apto";

        public GetAsignaturasExpedientesByIdExpedienteQueryHandler(
            IExpedientesContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<AsignaturaExpedienteListItemDto>> Handle(
            GetAsignaturasExpedientesByIdExpedienteQuery request, CancellationToken cancellationToken)
        {
            var asignaturaExpedientes = await _context.AsignaturasExpedientes
                .Include(ae => ae.AsignaturasCalificaciones)
                .Where(ae => ae.ExpedienteAlumnoId == request.IdExpedienteAlumno)
                .ToListAsync(cancellationToken);

            var asignaturasExpedientesDto = _mapper.Map<List<AsignaturaExpediente>,
                List<AsignaturaExpedienteListItemDto>>(asignaturaExpedientes);

            asignaturasExpedientesDto.ForEach(ae =>
            {
                ae.Calificacion = GetCalificacion(ae.AsignaturasCalificaciones);
            });

            return asignaturasExpedientesDto;
        }

        protected internal virtual string GetCalificacion(ICollection<AsignaturaCalificacionDto> asignaturasCalificaciones)
        {
            if (!asignaturasCalificaciones.Any()) return string.Empty;
            var calificaciones = asignaturasCalificaciones.OrderByDescending(ac => ac.Calificacion)
                .ThenByDescending(ac => ac.Convocatoria);
            var asignaturaCalificacion = calificaciones.FirstOrDefault(ac => ac.Superada) 
                               ?? calificaciones.FirstOrDefault(ac => !ac.Superada);
            var calificacion = asignaturaCalificacion.Calificacion <= 0 && 
                               (asignaturaCalificacion.NombreCalificacion?.ToLower() == Apto ||
                               asignaturaCalificacion.NombreCalificacion?.ToLower() == NoApto)
                ? string.Empty : asignaturaCalificacion.Calificacion?.ToString(CultureInfo.InvariantCulture);
            calificacion = $"{calificacion} {asignaturaCalificacion.NombreCalificacion}".Trim();
            return calificacion;
        }
    }
}
