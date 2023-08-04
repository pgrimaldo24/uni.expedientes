namespace Unir.Expedientes.Application.Common.Models.Evaluaciones
{
    public class NivelAsociadoEscalaModel
    {
        public int IdUniversidad { get; set; }
        public string Universidad { get; set; }
        public int? IdTipoEstudio { get; set; }
        public string TipoEstudio { get; set; }
        public int? IdEstudio { get; set; }
        public string Estudio { get; set; }
        public int? IdPlan { get; set; }
        public string Plan { get; set; }
        public int? IdTipoAsignatura { get; set; }
        public string TipoAsignatura { get; set; }
        public int? IdAsignaturaPlan { get; set; }
        public int? IdAsignaturaOfertada { get; set; }
        public string Asignatura { get; set; }
    }
}
