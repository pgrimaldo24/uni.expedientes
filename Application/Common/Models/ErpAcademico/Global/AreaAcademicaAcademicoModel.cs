namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Global
{
    public class AreaAcademicaAcademicoModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string DisplayName { get; set; }
        public CentroAcademicoModel Centro { get; set; }
    }
}
