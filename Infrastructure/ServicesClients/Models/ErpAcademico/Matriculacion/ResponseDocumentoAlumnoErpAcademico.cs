namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Matriculacion
{
    public class ResponseDocumentoAlumnoErpAcademico
    {
        public int Id { get; set; }
        public string EmailAlumno { get; set; }
        public string DisplayName { get; set; }
        public ResponseDocumentoAcademicoErpAcademico[] DocumentosAcademicos { get; set; }
    }
}
