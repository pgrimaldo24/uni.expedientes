using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Matriculacion
{
    public class AlumnoAcademicoModel
    {
        public int Id { get; set; }
        public string IdIntegracion { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameIdIntegracion => string.IsNullOrWhiteSpace(DisplayName) ? "-" : $"{IdIntegracion} - {DisplayName}";
        public PersonaErpAcademicoModel Persona { get; set; }
        public ICollection<MatriculaAcademicoModel> Matriculas { get; set; }
        public ICollection<DocumentoAlumnoAcademicoModel> DocumentosAlumnos { get; set; }
    }

    public class DocumentoAlumnoAcademicoModel
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string DisplayName { get; set; }
    }
}
