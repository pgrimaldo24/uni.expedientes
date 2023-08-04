using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseSubBloqueErpAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public double CreditosMinimos { get; set; }
        public double? CreditosObtenidos { get; set; }
        public string Descripcion { get; set; }
        public ResponseTipoSubBloqueErpAcademico TipoSubBloque { get; set; }
        public List<ResponseAsignaturaPlanSubBloqueErpAcademico> AsignaturasSubBloqueSuperadas { get; set; }
    }
}
