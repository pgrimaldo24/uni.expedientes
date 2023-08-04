using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio.AlumnoPuedeTitularse
{
    public class BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel
    {
        public BloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel()
        {
            AsignaturasBloqueNoSuperadas = new List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel>();
            SubBloquesNoSuperados = new List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double CreditosMinimos { get; set; }
        public double? CreditosObtenidos { get; set; }

        public List<AsignaturaPlanBloqueAlumnoPuedeTitularseErpAcademicoModel> AsignaturasBloqueNoSuperadas
        {
            get;
            set;
        }

        public List<SubBloqueNoSuperadoAlumnoPuedeTitularseErpAcademicoModel> SubBloquesNoSuperados { get; set; }
    }
}
