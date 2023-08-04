namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.OfertaEstudio.Migracion
{
    public class AsignaturaMatriculadaMigracionModel
    {
        public int Id { get; set; }
        public string IdRefCurso { get; set; }
        public string IdRefExpedienteAlumno { get; set; }
        public int IdAsignaturaOfertada { get; set; }
        public bool MatriculaActiva { get; set; }
    }
}
