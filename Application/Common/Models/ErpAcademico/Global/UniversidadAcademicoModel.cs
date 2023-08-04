namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Global
{
    public class UniversidadAcademicoModel
    {
        public int Id { get; set; }
        public string CodigoOficial { get; set; }
        public string Nombre { get; set; }
        public string Acronimo { get; set; }
        public string IdIntegracion { get; set; }
        public string IdIntegracionEmpresa { get; set; }
        public string DisplayName { get; set; }
        public string Web { get; set; }
        public string IdRefTerritorioLocalizacion { get; set; }
        public string Estatus { get; set; }
        public string Telefono { get; set; }
    }
}
