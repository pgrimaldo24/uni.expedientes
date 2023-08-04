using System.Collections.Generic;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Domain.Entities;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;

namespace Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente
{
    public class GetAsignaturasAsociadasQueryHandler : IRequestHandler<GetAsignaturasAsociadasQuery, List<AsignaturaExpediente>>
    {
        public async Task<List<AsignaturaExpediente>> Handle(GetAsignaturasAsociadasQuery request, CancellationToken cancellationToken)
        {
            var asignaturasAsociar = new List<AsignaturaExpediente>();
            foreach (var asignaturaOfertada in request.AsignaturasOfertadas)
            {
                var idRefAsignaturaPlan = asignaturaOfertada?.AsignaturaPlan?.Id.ToString();
                if (request.AsignaturasExpedienteExistentes.Any(x => x.IdRefAsignaturaPlan == idRefAsignaturaPlan))
                    continue;

                var asignaturaExpediente = AssignAsignaturaExpediente(idRefAsignaturaPlan, asignaturaOfertada, 
                    request.SituacionAsignaturaId, request.Reconocida.Value);
                asignaturasAsociar.Add(asignaturaExpediente);
            }
            return await Task.FromResult(asignaturasAsociar);
        }

        protected internal virtual AsignaturaExpediente AssignAsignaturaExpediente(string idRefAsignaturaPlan,
            AsignaturaOfertadaModel asignaturaOfertada, int situacionAsignatura, bool reconocida)
        {
            return new AsignaturaExpediente
            {
                IdRefAsignaturaPlan = idRefAsignaturaPlan,
                NombreAsignatura = asignaturaOfertada?.AsignaturaPlan?.Asignatura?.Nombre,
                CodigoAsignatura = asignaturaOfertada?.AsignaturaPlan?.Asignatura?.Codigo,
                OrdenAsignatura = asignaturaOfertada?.AsignaturaPlan?.Orden ?? 0,
                Ects = asignaturaOfertada?.AsignaturaPlan?.Asignatura?.Creditos ?? 0,
                IdRefTipoAsignatura = asignaturaOfertada?.TipoAsignatura?.Id.ToString(),
                SimboloTipoAsignatura = asignaturaOfertada?.TipoAsignatura?.Simbolo,
                OrdenTipoAsignatura = asignaturaOfertada?.TipoAsignatura?.Orden ?? 0,
                NombreTipoAsignatura = asignaturaOfertada?.TipoAsignatura?.Nombre,
                IdRefCurso = asignaturaOfertada?.Curso?.Id.ToString(),
                NumeroCurso = asignaturaOfertada?.Curso?.Numero ?? 0,
                AnyoAcademicoInicio = asignaturaOfertada?.PeriodoLectivo?.PeriodoAcademico?.AnyoAcademico?.AnyoInicio ?? 0,
                AnyoAcademicoFin = asignaturaOfertada?.PeriodoLectivo?.PeriodoAcademico?.AnyoAcademico?.AnyoFin ?? 0,
                PeriodoLectivo = asignaturaOfertada?.PeriodoLectivo?.Nombre,
                DuracionPeriodo = asignaturaOfertada?.PeriodoLectivo?.DuracionPeriodoLectivo?.Nombre,
                SimboloDuracionPeriodo = asignaturaOfertada?.PeriodoLectivo?.DuracionPeriodoLectivo?.Simbolo,
                IdRefIdiomaImparticion = asignaturaOfertada?.AsignaturaPlan?.Asignatura?.IdiomaImparticion?.Id.ToString(),
                SimboloIdiomaImparticion = asignaturaOfertada?.AsignaturaPlan?.Asignatura?.IdiomaImparticion?.Siglas,
                Reconocida = reconocida,
                SituacionAsignaturaId = situacionAsignatura
            };
        }
    }
}
