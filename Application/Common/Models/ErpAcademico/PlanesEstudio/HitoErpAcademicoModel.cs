using Unir.Expedientes.Application.Common.Models.ErpAcademico.Titulos;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class HitoErpAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public TituloAcademicoModel Titulo { get; set; }
        public EspecializacionAcademicoModel Especializacion { get; set; }
    }
}
