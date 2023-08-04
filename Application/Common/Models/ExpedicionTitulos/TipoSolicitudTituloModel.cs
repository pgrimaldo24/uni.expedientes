using Unir.Expedientes.Domain.Entities;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.Common.Models.ExpedicionTitulos
{
    public class TipoSolicitudTituloModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string IdRefUniversidad { get; set; }
        public string RefCodigoTipoSolicitud { get; set; }
        public bool ConFechaPago { get; set; }
    }
}
