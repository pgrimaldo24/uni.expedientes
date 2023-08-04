using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio.AlumnoPuedeTitularse
{
    public class BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel
    {
        public BloqueSuperadoAlumnoPuedeTitularseErpAcademicoModel()
        {
            AsignaturasBloqueSuperadas = new List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>();
            SubBloquesSuperados = new List<SubBloqueAlumnoPuedeTitularseErpAcademicoModel>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double CreditosMinimos { get; set; }
        public double? CreditosObtenidos { get; set; }
        public List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel> AsignaturasBloqueSuperadas { get; set; }
        public List<SubBloqueAlumnoPuedeTitularseErpAcademicoModel> SubBloquesSuperados { get; set; }
    }
}
