namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion
{
    public class DocumentoAlumnoModel
    {
        public int Id { get; set; }
        public string EmailAlumno { get; set; }
        public string DisplayName { get; set; }
        public DocumentoAcademicoModel[] DocumentosAcademicos { get; set; }
    }
}
