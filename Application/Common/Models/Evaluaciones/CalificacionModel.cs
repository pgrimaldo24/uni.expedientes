using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.Evaluaciones
{
    public class CalificacionModel
    {
        public bool MatriculaHonor { get; set; }
        public int CupoMatriculaHonor { get; set; }
        public bool NoPresentado { get; set; }
        public bool UmbralSobreNotas { get; set; }
        public double PorcentajeUmbralSobreNotas { get; set; }
        public bool UsarSiglaCalificacion { get; set; }
        public bool UsarNotaNumerica { get; set; }
        public bool AfectaPorcentaje { get; set; }
        public bool AfectaNotaNumerica { get; set; }
        public double NotaMinima { get; set; }
        public double NotaMaxima { get; set; }
        public double NotaMinAprobado { get; set; }
        public bool OcultarNotaNumerica { get; set; }
        public bool NotaFinalEnPorcentaje { get; set; }
        public bool NotasIntermediasEnPorcentaje { get; set; }
        public List<CalificacionListModel> Calificaciones { get; set; }
    }
}
