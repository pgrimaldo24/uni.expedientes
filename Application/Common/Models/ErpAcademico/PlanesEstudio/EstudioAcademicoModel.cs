using Unir.Expedientes.Application.Common.Models.ErpAcademico.Global;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class EstudioAcademicoModel
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string CodigoOficial { get; set; }
        public string DisplayName { get; set; }
        public AreaAcademicaAcademicoModel AreaAcademica { get; set; }
        public TipoEstudioAcademicoModel Tipo { get; set; }

    }
}
