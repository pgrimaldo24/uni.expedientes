using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseArcoErpAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int IdNodoOrigen { get; set; }
        public int IdNodoDestino { get; set; }
        public virtual List<int> BloquesSuperados { get; set; }
    }
}
