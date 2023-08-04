using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.Common.Commands.EliminarConsolidacionRequisitosAnuladaCommon
{
    public class EliminarConsolidacionRequisitosAnuladaCommonCommandHandler : IRequestHandler<EliminarConsolidacionRequisitosAnuladaCommonCommand>
    {
        private readonly IExpedientesContext _context;
        public EliminarConsolidacionRequisitosAnuladaCommonCommandHandler(IExpedientesContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(EliminarConsolidacionRequisitosAnuladaCommonCommand request, CancellationToken cancellationToken)
        {
            var alumno = request.Alumno;
            var expedienteAlumno = alumno.ExpedienteAlumno;
            var alumnoAcademico = alumno.AlumnoAcademicoModel;
            var matriculaActual = alumno.MatriculaAcademicoModel;

            var asignaturasOtrasMatriculas = alumnoAcademico.Matriculas.Where(m => m.Id != matriculaActual.Id).SelectMany(m => m.AsignaturaMatriculadas).ToList();
            var idsAsignaturasPlanOtrasMatriculas = asignaturasOtrasMatriculas.Select(x => x.AsignaturaOfertada.AsignaturaPlan.Id.ToString()).ToList();
            if (!expedienteAlumno.ConsolidacionesRequisitosExpedientes.Any()) return Unit.Value;
            var asignaturasMatricula = new List<AsignaturaExpediente>();
            foreach (var asignaturaMatriculadaModel in matriculaActual.AsignaturaMatriculadas)
            {
                var AsignaturaExpediente = new AsignaturaExpediente();
                AsignaturaExpediente.IdRefTipoAsignatura = asignaturaMatriculadaModel.AsignaturaOfertada.AsignaturaPlan.Asignatura.TipoAsignatura.Id.ToString();
                AsignaturaExpediente.IdRefAsignaturaPlan = asignaturaMatriculadaModel.AsignaturaOfertada.AsignaturaPlan.Id.ToString();
                asignaturasMatricula.Add(AsignaturaExpediente);
            }

            var nivelesUsoComportamientos = GetNivelesUsoComportamientos(asignaturasMatricula);
            if (!nivelesUsoComportamientos.Any()) return Unit.Value;

            var idsAsignaturasPlanTipos = new List<string>();
            var idsRefTiposAsignaturas = nivelesUsoComportamientos
                .Where(nc => nc.TipoNivelUsoId == TipoNivelUso.TipoAsignatura)
                .Select(nuc => nuc.IdRefTipoAsignatura).ToList();
            if (idsRefTiposAsignaturas.Any())
            {
                var asignaturasPlanes = matriculaActual.AsignaturaMatriculadas
                    .Where(am => request.IdsAsignaturasOfertadas.Contains(am.AsignaturaOfertada.Id))
                    .Select(am => am.AsignaturaOfertada.AsignaturaPlan).ToList();

                idsAsignaturasPlanTipos = asignaturasPlanes
                    .Where(a => idsRefTiposAsignaturas.Contains(a.Asignatura.TipoAsignatura.Id.ToString()))
                    .Select(a => a.Id.ToString()).ToList();
            }

            var idsRefAsignaturasPlan = nivelesUsoComportamientos
                .Where(nc => nc.TipoNivelUsoId == TipoNivelUso.AsignaturaPlan)
                .Select(nuc => nuc.IdRefAsignaturaPlan).ToList();
            idsAsignaturasPlanTipos.AddRange(idsRefAsignaturasPlan);
            idsAsignaturasPlanTipos = idsAsignaturasPlanTipos.Distinct().ToList();

            var idsRefAsignaturasPlanEliminar = idsAsignaturasPlanTipos.Except(idsAsignaturasPlanOtrasMatriculas);

            nivelesUsoComportamientos = nivelesUsoComportamientos.Where(nu => idsRefAsignaturasPlanEliminar.Contains(nu.IdRefAsignaturaPlan));
            var idsComportamientos = nivelesUsoComportamientos.Select(nuc => nuc.ComportamientoExpedienteId).ToList();
            var comportamientos = await _context.ComportamientosExpedientes
                 .Include(ce => ce.RequisitosComportamientosExpedientes)
                 .ThenInclude(rce => rce.RequisitoExpediente)
                 .Where(ce => idsComportamientos.Contains(ce.Id))
                 .ToListAsync(cancellationToken);

            var idsRequisitos = expedienteAlumno.ConsolidacionesRequisitosExpedientes?
                .Select(ea => ea.RequisitoExpedienteId);

            var requisitosComportamientosExpediente = comportamientos
                .SelectMany(c => c.RequisitosComportamientosExpedientes)
                .Where(rce => idsRequisitos.Contains(rce.RequisitoExpedienteId));

            var idsRequisitosEliminar = requisitosComportamientosExpediente.Select(rc => rc.RequisitoExpedienteId).ToList();

            var consolidacionesEliminar = expedienteAlumno.ConsolidacionesRequisitosExpedientes?
                .Where(cr => idsRequisitosEliminar.Contains(cr.RequisitoExpedienteId) && !cr.EsEstadoValidado())
                .ToList();

            if (consolidacionesEliminar != null && consolidacionesEliminar.Any())
                _context.ConsolidacionesRequisitosExpedientes.RemoveRange(consolidacionesEliminar);
            return Unit.Value;
        }

        protected internal virtual IEnumerable<NivelUsoComportamientoExpediente> GetNivelesUsoComportamientos(List<AsignaturaExpediente> asignaturas)
        {
            var nivelesUsoComportamientos = new List<NivelUsoComportamientoExpediente>();

            var nivelUsoComportamientosTipoAsignatura = asignaturas
                .Join(_context.NivelesUsoComportamientosExpedientes,
                    asignaturaExpediente => new { asignaturaExpediente.IdRefTipoAsignatura, TipoNivelUsoId = TipoNivelUso.TipoAsignatura },
                    nivelUso => new { nivelUso.IdRefTipoAsignatura, nivelUso.TipoNivelUsoId },
                    (asignaturaExpediente, nivelUso) => new { AsignaturaExpediente = asignaturaExpediente, NivelUso = nivelUso })
                .Select(aenu => aenu.NivelUso);

            var nivelUsoComportamientosAsignaturaPlan = asignaturas
                .Join(_context.NivelesUsoComportamientosExpedientes,
                    asignaturaExpediente => new { asignaturaExpediente.IdRefAsignaturaPlan, TipoNivelUsoId = TipoNivelUso.AsignaturaPlan, },
                    nivelUso => new { nivelUso.IdRefAsignaturaPlan, nivelUso.TipoNivelUsoId },
                    (asignaturaExpediente, nivelUso) => new { AsignaturaExpediente = asignaturaExpediente, NivelUso = nivelUso })
                .Select(aenu => aenu.NivelUso);

            nivelesUsoComportamientos.AddRange(nivelUsoComportamientosTipoAsignatura);
            nivelesUsoComportamientos.AddRange(nivelUsoComportamientosAsignaturaPlan);
            return nivelesUsoComportamientos.Distinct().ToList();
        }
    }
}
