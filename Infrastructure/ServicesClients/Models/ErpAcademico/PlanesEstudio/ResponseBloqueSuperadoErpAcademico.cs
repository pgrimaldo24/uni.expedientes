using System.Collections.Generic;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseBloqueSuperadoErpAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public double CreditosMinimos { get; set; }
        public double? CreditosObtenidos { get; set; }
        public List<ResponseAsignaturaPlanBloqueErpAcademico> AsignaturasBloqueSuperadas { get; set; }
        public List<ResponseSubBloqueErpAcademico> SubBloquesSuperados { get; set; }
    }
}
