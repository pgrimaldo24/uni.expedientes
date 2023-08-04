using MediatR;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Queries.GetCalificacionByNota
{
    public class GetCalificacionByNotaQuery : IRequest<CalificacionListModel>
    {
        public int AsignaturaOfertadaId { get; set; }
        public double Nota { get; set; }
        public GetCalificacionByNotaQuery(int asignaturaOfertadaId, double nota)
        {
            AsignaturaOfertadaId = asignaturaOfertadaId;
            Nota = nota;
        }
    }
}
