using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion
{
    public class ResponseAlumnoErpAcademico
    {
        public int Id { get; set; }
        public string IdIntegracion { get; set; }
        public string DisplayName { get; set; }
        public ResponsePersonaAcademico Persona { get; set; }
        public ICollection<ResponseMatriculaAcademicoModel> Matriculas { get; set; }
        public ICollection<ResponseDocumentoAlumnoErpAcademicoModel> DocumentosAlumnos { get; set; }
    }

    public class ResponseDocumentoAlumnoErpAcademicoModel
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string DisplayName { get; set; }
    }
}
