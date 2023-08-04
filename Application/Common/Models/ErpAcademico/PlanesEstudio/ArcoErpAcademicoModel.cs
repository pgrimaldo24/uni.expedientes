using System.Collections.Generic;

namespace Unir.Expedientes.Application.Common.Models.ErpAcademico.PlanesEstudio
{
    public class ArcoErpAcademicoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdNodoOrigen { get; set; }
        public int IdNodoDestino { get; set; }
        public virtual List<int> BloquesSuperados { get; set; }
    }
}
