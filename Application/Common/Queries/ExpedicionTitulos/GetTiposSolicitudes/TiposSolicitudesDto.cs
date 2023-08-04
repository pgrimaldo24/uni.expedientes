using Unir.Expedientes.Application.Common.Models.ExpedicionTitulos;
using Unir.Framework.ApplicationSuperTypes.Mappings;

namespace Unir.Expedientes.Application.Common.Queries.ExpedicionTitulos.GetTiposSolicitudes
{
    public class TiposSolicitudesDto : IMapFrom<TiposSolicitudesTitulosModel>
    {
        public int Id { get; set; }
        public string RefCodigoTipoSolicitud { get; set; }
        public string Nombre { get; set; }
        public bool ConFechaPago { get; set; }
        public bool DestinoAlumno { get; set; }
        public FormatoTipoSolicitud FormatoTipoSolicitud { get; set; }
        public string DisplayName => $"({RefCodigoTipoSolicitud}) {Nombre}";
    }
}
