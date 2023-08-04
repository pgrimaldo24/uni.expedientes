using MediatR;
using System;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Queries.GetCicloPeriodoLectivo
{
    public class GetCicloPeriodoLectivoQuery : IRequest<string>
    {
        public DateTime FechaInicioPeriodoLectivo { get; set; }
        public int IdDuracionPeriodoLectivo { get; set; }
        public GetCicloPeriodoLectivoQuery(DateTime fechaInicioPeriodoLectivo, int idDuracionPeriodoLectivo)
        {
            FechaInicioPeriodoLectivo = fechaInicioPeriodoLectivo;
            IdDuracionPeriodoLectivo = idDuracionPeriodoLectivo;
        }
    }
}
