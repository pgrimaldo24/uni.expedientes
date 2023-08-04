
namespace Unir.Expedientes.Application.Common.Models.Evaluaciones
{
    public class CalificacionListModel
    {
        public int Orden { get; set; }
        public string Nombre { get; set; }
        public string Sigla { get; set; }
        public double? PorcentajeMinimo { get; set; }
        public double? NotaMinima { get; set; }
        public bool Pondera { get; set; }
        public bool EsMatriculaHonor { get; set; }
        public bool EsNoPresentado { get; set; }
    }
}
