using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Domain.Entities;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Queries.GetCicloPeriodoLectivo
{
    public class GetCicloPeriodoLectivoQueryHandler : IRequestHandler<GetCicloPeriodoLectivoQuery, string>
    {
        private const int Mensual = 1;
        private const int Bimestral = 2;
        private const int Trimestral = 3;
        private const int Cuatrimestral = 4;
        private const int Semestral = 6;
        private const int Anual = 12;
        public async Task<string> Handle(GetCicloPeriodoLectivoQuery request, CancellationToken cancellationToken)
        {
            var periodoSeleccionado = string.Empty;
            var mesesPeriodo = 
                request.IdDuracionPeriodoLectivo == AsignaturaCalificacion.IdDuracionPeriodoLectivoMensual ? Mensual :
                request.IdDuracionPeriodoLectivo == AsignaturaCalificacion.IdDuracionPeriodoLectivoBimestral ? Bimestral :
                request.IdDuracionPeriodoLectivo == AsignaturaCalificacion.IdDuracionPeriodoLectivoTrimestral ? Trimestral :
                request.IdDuracionPeriodoLectivo == AsignaturaCalificacion.IdDuracionPeriodoLectivoCuatrimestral ? Cuatrimestral :
                request.IdDuracionPeriodoLectivo == AsignaturaCalificacion.IdDuracionPeriodoLectivoSemestral ? Semestral :
                request.IdDuracionPeriodoLectivo == AsignaturaCalificacion.IdDuracionPeriodoLectivoAnual ? Anual : 0;

            if (mesesPeriodo == 0) return null;

            var cantidadPeriodos = Anual / mesesPeriodo;
            int periodoIncrementable = mesesPeriodo;
            for (int i = 1; i <= cantidadPeriodos; i++)
            {
                if (request.FechaInicioPeriodoLectivo.Month <= periodoIncrementable)
                {
                    periodoSeleccionado = $"{request.FechaInicioPeriodoLectivo.Year}-{i}";
                    break;
                }
                periodoIncrementable += mesesPeriodo;
            }
            return periodoSeleccionado;
        }
    }
}
