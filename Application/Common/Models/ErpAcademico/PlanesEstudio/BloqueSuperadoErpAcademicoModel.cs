using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class BloqueSuperadoErpAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double CreditosMinimos { get; set; }
        public double? CreditosObtenidos { get; set; }
        public List<AsignaturaPlanBloqueErpAcademicoModel> AsignaturasBloqueSuperadas { get; set; }
        public List<SubBloqueErpAcademicoModel> SubBloquesSuperados { get; set; }
    }
}
