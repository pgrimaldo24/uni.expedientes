using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;

namespace Unir.Expedientes.Infrastructure.ServicesClients.Models.ExpedicionTitulos
{
    public class ResponseTiposSolicitudesTitulos
    {
        public int Id { get; set; }
        public string RefCodigoTipoSolicitud { get; set; }
        public string Nombre { get; set; }
        public bool ConFechaPago { get; set; }
        public bool DestinoAlumno { get; set; }
        public FormatoTipoSolicitud FormatoTipoSolicitud { get; set; }
    }
}
