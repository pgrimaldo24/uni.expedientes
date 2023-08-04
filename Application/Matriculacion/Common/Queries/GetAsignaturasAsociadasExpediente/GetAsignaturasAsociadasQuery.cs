using MediatR;
using System.Collections.Generic;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion;
using Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.Matriculacion.Common.Queries.GetAsignaturasAsociadasExpediente
{
    public class GetAsignaturasAsociadasQuery : IRequest<List<AsignaturaExpediente>>
    {
        public List<AsignaturaExpediente> AsignaturasExpedienteExistentes { get; set; }
        public List<AsignaturaOfertadaModel> AsignaturasOfertadas { get; set; }
        public int SituacionAsignaturaId { get; set; }
        public bool? Reconocida { get; set; }
        
        public GetAsignaturasAsociadasQuery(List<AsignaturaExpediente> asignaturasExpedienteExistentes, List<AsignaturaOfertadaModel> asignaturasOfertadas, int situacionAsignaturaId, bool reconocida = false)
        {
            AsignaturasExpedienteExistentes = asignaturasExpedienteExistentes;
            AsignaturasOfertadas = asignaturasOfertadas;
            SituacionAsignaturaId = situacionAsignaturaId;
            Reconocida = reconocida;
        }
    }
}
