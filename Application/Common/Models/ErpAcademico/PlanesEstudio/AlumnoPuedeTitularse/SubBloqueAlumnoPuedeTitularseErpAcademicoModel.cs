using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio.AlumnoPuedeTitularse
{
    public class SubBloqueAlumnoPuedeTitularseErpAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double CreditosMinimos { get; set; }
        public double? CreditosObtenidos { get; set; }
        public string Descripcion { get; set; }
        public TipoSubBloqueErpAcademicoModel TipoSubBloque { get; set; }
        public List<AsignaturaPlanSubBloqueAlumnoPuedeTitularseErpAcademicoModel> AsignaturasSubBloqueSuperadas { get; set; }
    }
}
