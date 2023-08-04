using Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.Global;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ErpAcademico.PlanesEstudio
{
    public class ResponseEstudioErpAcademico
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string CodigoOficial { get; set; }
        public string DisplayName { get; set; }
        public ResponseAreaAcademicaErpAcademico AreaAcademica { get; set; }
        public ResponseTipoEstudioErpAcademico Tipo { get; set; }
    }
}
