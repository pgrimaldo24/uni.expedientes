namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Global
{
    public class CentroAcademicoModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string DisplayName { get; set; }
        public string IdRefResponsableCentro { get; set; }
        public UniversidadAcademicoModel Universidad { get; set; }
    }
}
