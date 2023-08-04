using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTipoSolicitud
{
    public class TipoSolicitudDto : IMapFrom<TipoSolicitudTituloModel>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string IdRefUniversidad { get; set; }
        public string RefCodigoTipoSolicitud { get; set; }
        public bool ConFechaPago { get; set; }
    }
}
