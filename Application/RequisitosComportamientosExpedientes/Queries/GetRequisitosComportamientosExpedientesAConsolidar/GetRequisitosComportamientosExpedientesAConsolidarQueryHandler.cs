using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.RequisitosComportamientosExpedientes.Queries.GetRequisitosComportamientosExpedientesAConsolidar
{
    public class GetRequisitosComportamientosExpedientesAConsolidarQueryHandler :
        IRequestHandler<GetRequisitosComportamientosExpedientesAConsolidarQuery, List<RequisitoComportamientoExpediente>>
    {
        private readonly IExpedientesContext _context;
        public GetRequisitosComportamientosExpedientesAConsolidarQueryHandler(
            IExpedientesContext context)
        {
            _context = context;
        }
        public async Task<List<RequisitoComportamientoExpediente>> Handle(
            GetRequisitosComportamientosExpedientesAConsolidarQuery request, CancellationToken cancellationToken)
        {
            var idsComportamientos = new List<int>();
            idsComportamientos.AddRange(await 
                GetIdsComportamientosAsignaturasNivelesDeUso(request.ExpedienteAlumno.Id, cancellationToken));
            idsComportamientos.AddRange(await 
                GetIdsComportamientosExpedientesNivelesDeUso(request.ExpedienteAlumno, cancellationToken));
            idsComportamientos = idsComportamientos.Distinct().ToList();
            if (!idsComportamientos.Any()) return null;

            var comportamientos = await _context.ComportamientosExpedientes
                .Include(ce => ce.RequisitosComportamientosExpedientes)
                .ThenInclude(rce => rce.RequisitoExpediente)
                .Where(ce => idsComportamientos.Contains(ce.Id) && !ce.Bloqueado)
                .ToListAsync(cancellationToken);
            if (!comportamientos.Any()) return null;

            var idsRequisitos = request.ExpedienteAlumno.ConsolidacionesRequisitosExpedientes?
                .Select(ea => ea.RequisitoExpedienteId) ?? new List<int>();
            var requisitosComportamientosExpediente = comportamientos
                .SelectMany(c => c.RequisitosComportamientosExpedientes)
                .Where(rce => !idsRequisitos.Contains(rce.RequisitoExpedienteId) && !rce.RequisitoExpediente.Bloqueado);
            return requisitosComportamientosExpediente.ToList();
        }

        protected internal virtual async Task<IEnumerable<int>> GetIdsComportamientosAsignaturasNivelesDeUso(
            int idExpedienteAlumno, CancellationToken cancellationToken)
        {
            var idsComportamientos = new List<int>();
            var asignaturasExpedientes = await _context.AsignaturasExpedientes
                .Where(ae => ae.ExpedienteAlumnoId == idExpedienteAlumno)
                .ToListAsync(cancellationToken);
            if (!asignaturasExpedientes.Any()) return idsComportamientos;

            var idsComportamientosTipoAsignatura = asignaturasExpedientes
                .Join(_context.NivelesUsoComportamientosExpedientes,
                    asignaturaExpediente => new { asignaturaExpediente.IdRefTipoAsignatura, TipoNivelUsoId = TipoNivelUso.TipoAsignatura },
                    nivelUso => new { nivelUso.IdRefTipoAsignatura, nivelUso.TipoNivelUsoId },
                    (asignaturaExpediente, nivelUso) => new { AsignaturaExpediente = asignaturaExpediente, NivelUso = nivelUso })
                .Where(aenu => aenu.AsignaturaExpediente.SituacionAsignaturaId != SituacionAsignatura.Anulada)
                .Select(aenu => aenu.NivelUso.ComportamientoExpedienteId);

            var idsComportamientosAsignaturaPlan = asignaturasExpedientes
                .Join(_context.NivelesUsoComportamientosExpedientes,
                    asignaturaExpediente => new { asignaturaExpediente.IdRefAsignaturaPlan, TipoNivelUsoId = TipoNivelUso.AsignaturaPlan },
                    nivelUso => new { nivelUso.IdRefAsignaturaPlan, nivelUso.TipoNivelUsoId },
                    (asignaturaExpediente, nivelUso) => new { AsignaturaExpediente = asignaturaExpediente, NivelUso = nivelUso })
                .Where(aenu => aenu.AsignaturaExpediente.SituacionAsignaturaId != SituacionAsignatura.Anulada)
                .Select(aenu => aenu.NivelUso.ComportamientoExpedienteId);

            idsComportamientos.AddRange(idsComportamientosTipoAsignatura);
            idsComportamientos.AddRange(idsComportamientosAsignaturaPlan);
            return idsComportamientos.Distinct();
        }

        protected internal virtual async Task<IEnumerable<int>> GetIdsComportamientosExpedientesNivelesDeUso(
            ExpedienteAlumno expedienteAlumno, CancellationToken cancellationToken)
        {
            var nivelesUsoComportamientos = await _context.NivelesUsoComportamientosExpedientes
                .Where(nuce 
                    => (nuce.IdRefPlan == expedienteAlumno.IdRefPlan && nuce.TipoNivelUsoId == TipoNivelUso.PlanEstudio)
                    || (nuce.IdRefEstudio == expedienteAlumno.IdRefEstudio && nuce.TipoNivelUsoId == TipoNivelUso.Estudio)
                    || (nuce.IdRefTipoEstudio == expedienteAlumno.IdRefTipoEstudio && nuce.TipoNivelUsoId == TipoNivelUso.TipoEstudio)                    
                    || (nuce.IdRefUniversidad == expedienteAlumno.IdRefUniversidad && nuce.TipoNivelUsoId == TipoNivelUso.Universidad))
                .ToListAsync();
            return nivelesUsoComportamientos.Select(nuc => nuc.ComportamientoExpedienteId).Distinct();
        }
    }
}
