namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.Titulos
{
    public class EspecializacionAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public TipoEspecializacionAcademicoModel TipoEspecializacion { get; set; }
    }
}
