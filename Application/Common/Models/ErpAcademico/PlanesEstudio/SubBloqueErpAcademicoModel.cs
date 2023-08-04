using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class SubBloqueErpAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double CreditosMinimos { get; set; }
        public double? CreditosObtenidos { get; set; }
        public string Descripcion { get; set; }
        public TipoSubBloqueErpAcademicoModel TipoSubBloque { get; set; }
        public List<AsignaturaPlanSubBloqueErpAcademicoModel> AsignaturasSubBloqueSuperadas { get; set; }
    }
}
