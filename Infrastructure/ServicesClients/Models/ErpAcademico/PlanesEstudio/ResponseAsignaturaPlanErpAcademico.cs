namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseAsignaturaPlanErpAcademico
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameNivelUso { get; set; }
        public ResponseAsignaturaErpAcademico Asignatura { get; set; }
        public ResponsePlanErpAcademico Plan { get; set; }
    }
}
