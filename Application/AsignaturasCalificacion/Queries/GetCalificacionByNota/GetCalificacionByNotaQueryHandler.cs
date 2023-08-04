using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unir.Expedientes.Application.Common.Interfaces;
using Unir.Expedientes.Application.Common.Models.Evaluaciones;

namespace Unir.Expedientes.Application.AsignaturasCalificacion.Queries.GetCalificacionByNota
{
    public class GetCalificacionByNotaQueryHandler : IRequestHandler<GetCalificacionByNotaQuery, CalificacionListModel>
    {
        private readonly IEvaluacionesServiceClient _evaluacionesServiceClient;
        public GetCalificacionByNotaQueryHandler(IEvaluacionesServiceClient evaluacionesServiceClient)
        {
            _evaluacionesServiceClient = evaluacionesServiceClient;
        }
        public async Task<CalificacionListModel> Handle(GetCalificacionByNotaQuery request, CancellationToken cancellationToken)
        {
            var configuracionEscalas = await _evaluacionesServiceClient.GetConfiguracionEscalaFromNivelesAsociadosEscalas(request.AsignaturaOfertadaId);
            if (configuracionEscalas is null) return null;

            var calificacionEscala = configuracionEscalas.Configuracion.Calificacion;
            var calificaciones = calificacionEscala.Calificaciones.Where(x => !x.EsNoPresentado).OrderBy(c => c.Orden).ToList();

            calificaciones = calificacionEscala.AfectaNotaNumerica ? calificaciones.OrderBy(c => c.NotaMinima).ToList() :
                calificaciones.OrderBy(c => c.PorcentajeMinimo).ToList();

            CalificacionListModel calificacionSeleccionada = null;
            foreach (var calificacion in calificaciones)
            {
                var notaCalificacion = calificacionEscala.AfectaNotaNumerica ? calificacion.NotaMinima : calificacion.PorcentajeMinimo;
                if (request.Nota >= notaCalificacion)
                {
                    calificacionSeleccionada = calificacion;
                    continue;
                }
                break;
            }
            return calificacionSeleccionada;
        }
    }
}
