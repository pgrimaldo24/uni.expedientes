using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Global;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseTipoEstudioErpAcademico
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }
        public string DisplayName { get; set; }
        public ResponseUniversidadErpAcademico Universidad { get; set; }
    }
}